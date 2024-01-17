using SmfLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;
using static Datas;
using Melanchall.DryWetMidi.Multimedia;
using System;
using Melanchall.DryWetMidi.Core;

public class MidiTest : MonoBehaviour
{
    public TextAsset sourceFile;
    public MidiFileContainer song;
    public GameObject whiteNoteObj;
    public GameObject blackNoteObj;
    public GameObject keyTextObj;
    public Transform noteInstantiatePoint;

    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;
    public Material whiteKeyOne;
    public Material whiteKeyTwo;
    public Material blackKeyOne;
    public Material blackKeyTwo;

    public int tempo = 120;
    public float noteScale = 1.0f;
    public float scrollSpeed = 1.0f;
    public float currentDeltaTime = 0.0f;
    public float notePosOffset = 0.0f;

    bool _isInputTiming = false;

    List<GameObject> instatiateNotes = new List<GameObject>();
    List<Notes> notes = new List<Notes>();
    Dictionary<int, int> tempNoteData = new Dictionary<int, int>();
    List<int> noteTiming = new List<int>();
    Dictionary<int, List<KeyValuePair<int, bool>>> noteSetBySameTime = new Dictionary<int, List<KeyValuePair<int, bool>>>();
    int currentNoteIndex = 0;

    void Awake()
    {
        sourceFile = Resources.Load<TextAsset>("Converts/for_elise");
    }

    void Start()
    {
        song = MidiFileLoader.Load(sourceFile.bytes);
        while (song.tracks == null)     // 혹시 모를 비동기 로드 상황에 대비
        {
            Debug.Log("Now MIDI data on loading..");
        }
        Debug.Log("MIDI data loaded!");

        tempo = CalcTempoWithRatio(Datas.DEFAULT_QUARTER_NOTE_MILLISEC / song.tempoMap[0].milliSecond);

        int trackNum = 0;
        for (int i = 0; i < song.tracks.Count; i++)
        {
            MidiTrack track = song.tracks[i];
            int eventStartTime = -1;
            int deltaTime = 0;
            for (int j = 0; j < track.sequence.Count; j++)
            {
                deltaTime = track.sequence[j].delta != 0 ? track.sequence[j].delta : deltaTime;
                //eventStartTime += eventStartTime == -1 ? 1 : track.sequence[j].delta;
                eventStartTime += track.sequence[j].delta;
                if (track.sequence[j].midiEvent.status == 144 && track.sequence[j].midiEvent.data2 > 0)
                {
                    if (tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1))
                    {
                        notes.Add(new Notes(track.sequence[j].midiEvent.data1, tempNoteData[track.sequence[j].midiEvent.data1], eventStartTime, trackNum));
                        tempNoteData.Remove(track.sequence[j].midiEvent.data1);
                    }
                    tempNoteData.Add(track.sequence[j].midiEvent.data1, eventStartTime);
                }
                else if (track.sequence[j].midiEvent.status == 128 || (track.sequence[j].midiEvent.status == 144 && track.sequence[j].midiEvent.data2 == 0))
                {
                    if (!tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1))
                        continue;
                    notes.Add(new Notes(track.sequence[j].midiEvent.data1, tempNoteData[track.sequence[j].midiEvent.data1], eventStartTime, trackNum));
                    tempNoteData.Remove(track.sequence[j].midiEvent.data1);
                }
            }
            if (notes.Count > 0 && trackNum == 0)
                trackNum++;
        }
        for (int i = 0; i < notes.Count; i++)
        {
            if (!noteSetBySameTime.ContainsKey(notes[i].startTime))
            {
                noteTiming.Add(notes[i].startTime);
                noteSetBySameTime.Add(notes[i].startTime, new List<KeyValuePair<int, bool>>());
            }
            noteSetBySameTime[notes[i].startTime].Add(new KeyValuePair<int, bool>(notes[i].keyNum - 1, false));

            string noteKeyStr = GetKeyFromKeynum(notes[i].keyNum);
            if (BlackKeyJudge(notes[i].keyNum))
            {
                int keyPos = NoteKeyPosOrder(notes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instatiateNotes.Add(Instantiate(whiteNoteObj, noteInstantiatePoint));
                instatiateNotes[i].transform.localScale = new Vector3(0.13125f, 0.13125f, notes[i].deltaTime / (float)song.division * noteScale);
                instatiateNotes[i].transform.localPosition = new Vector3(0.065625f + keyPos * 0.13125f + keyOffset * 1.575f, 0.2f, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
                if (notes[i].channel == 0)
                {
                    instatiateNotes[i].GetComponent<Renderer>().material = blackKeyOne;
                }
                else
                {
                    instatiateNotes[i].GetComponent<Renderer>().material = blackKeyTwo;
                }
            }
            else
            {
                int keyPos = NoteKeyPosOrder(notes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instatiateNotes.Add(Instantiate(whiteNoteObj, noteInstantiatePoint));
                instatiateNotes[i].transform.localScale = new Vector3(0.225f, 0.225f, notes[i].deltaTime / (float)song.division * noteScale);
                instatiateNotes[i].transform.localPosition = new Vector3(0.1125f + keyPos * 0.225f + keyOffset * 1.575f, 0, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
                if (notes[i].channel == 0)
                {
                    instatiateNotes[i].GetComponent<Renderer>().material = whiteKeyOne;
                }
                else
                {
                    instatiateNotes[i].GetComponent<Renderer>().material = whiteKeyTwo;
                }
            }
            
            GameObject tempKeyObject = Instantiate(keyTextObj, noteInstantiatePoint);
            tempKeyObject.transform.parent = instatiateNotes[i].transform;
            tempKeyObject.transform.localPosition = new Vector3(0, 0.55f, -0.5f);
            tempKeyObject.transform.position = new Vector3(tempKeyObject.transform.position.x, tempKeyObject.transform.position.y, tempKeyObject.transform.position.z + 0.1f);
            tempKeyObject.GetComponent<TextMeshPro>().text = GetKeyFromKeynum(notes[i].keyNum);
        }

        noteTiming.Sort();
    }

    void Update()
    {
        WaitMidiInput();
        Scroll();
    }

    void Scroll()
    {
        if (noteTiming[currentNoteIndex] <= currentDeltaTime)
        {
            currentDeltaTime = noteTiming[currentNoteIndex];
            transform.position = new Vector3(0, 0, -currentDeltaTime / song.division * noteScale + notePosOffset);
            _isInputTiming = true;
            return;
        }
        currentDeltaTime += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / song.tempoMap[0].milliSecond * song.division * Time.deltaTime;
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / song.tempoMap[0].milliSecond * noteScale * Time.deltaTime));
    }

    void WaitMidiInput()
    {
        if (!_isInputTiming)
            return;
        for (int i = 0; i < noteSetBySameTime[noteTiming[currentNoteIndex]].Count; i++)
        {
            noteSetBySameTime[noteTiming[currentNoteIndex]][i] = new KeyValuePair<int, bool>(noteSetBySameTime[noteTiming[currentNoteIndex]][i].Key, Managers.Input.keyChecks[noteSetBySameTime[noteTiming[currentNoteIndex]][i].Key]);
            if (!noteSetBySameTime[noteTiming[currentNoteIndex]][i].Value)
                return;
        }
        IncreaseCurrentNoteIndex();
    }

    public void IncreaseCurrentNoteIndex()
    {
        currentNoteIndex += 1;
    }

    public void DisconnectPiano()
    {
        Managers.Input.inputDevice.StopEventsListening();
    }

    // 밀리초 데이터를 이용한 곡 템포 계산
    int CalcTempoWithRatio(float ratio)
    {
        int resultTempo = -1;
        float temp = Datas.DEFAULT_TEMPO * ratio;
        resultTempo = temp - (int)temp < 0.5f ? (int)temp : (int)temp + 1;
        return resultTempo;
    }

    string GetKeyFromKeynum(int keyNum)
    {
        keyNum -= 20;
        string key = "";
        string pos = "";
        int offset = -1;
        if (keyNum > 3)
        {
            keyNum -= 3;
            offset = keyNum / 12;
            if (keyNum % 12 == 0)
                offset--;
        }
        else
            keyNum += 9;

        switch (keyNum % 12)
        {
            case 1:
                pos = "C";
                break;
            case 2:
                pos = "C#";
                break;
            case 3:
                pos = "D";
                break;
            case 4:
                pos = "D#";
                break;
            case 5:
                pos = "E";
                break;
            case 6:
                pos = "F";
                break;
            case 7:
                pos = "F#";
                break;
            case 8:
                pos = "G";
                break;
            case 9:
                pos = "G#";
                break;
            case 10:
                pos = "A";
                break;
            case 11:
                pos = "A#";
                break;
            case 0:
                pos = "B";
                break;
        }
        key = $"{pos}{offset}";
        return key;
    }

    bool BlackKeyJudge(int keyNum)
    {
        bool flag = false;
        keyNum -= 20;
        if (keyNum > 3)
            keyNum -= 3;
        else
            keyNum += 9;
        switch (keyNum % 12)
        {
            case 1:
                flag = false;
                break;
            case 2:
                flag = true;
                break;
            case 3:
                flag = false;
                break;
            case 4:
                flag = true;
                break;
            case 5:
                flag = false;
                break;
            case 6:
                flag = false;
                break;
            case 7:
                flag = true;
                break;
            case 8:
                flag = false;
                break;
            case 9:
                flag = true;
                break;
            case 10:
                flag = false;
                break;
            case 11:
                flag = true;
                break;
            case 0:
                flag = false;
                break;
        }
        return flag;
    }

    int NoteKeyPosOrder(int keyNum)
    {
        int keyPos = 0;
        keyNum -= 20;
        if (keyNum > 3)
            keyNum -= 3;
        else
            keyNum += 9;
        switch (keyNum % 12)
        {
            case 1:
                keyPos = 1;
                break;
            case 2:
                keyPos = 2;
                break;
            case 3:
                keyPos = 2;
                break;
            case 4:
                keyPos = 4;
                break;
            case 5:
                keyPos = 3;
                break;
            case 6:
                keyPos = 4;
                break;
            case 7:
                keyPos = 7;
                break;
            case 8:
                keyPos = 5;
                break;
            case 9:
                keyPos = 9;
                break;
            case 10:
                keyPos = 6;
                break;
            case 11:
                keyPos = 11;
                break;
            case 0:
                keyPos = 7;
                break;
        }
        return keyPos;
    }
}
