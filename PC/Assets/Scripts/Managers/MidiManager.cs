/* 미디 매니저
 * 작성 - 이원섭
 * 미디 파일의 입출력을 위해 사용하는 객체 */

using SmfLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;
using static Datas;

public class MidiManager
{
    GameObject noteObj;
    GameObject keyTextObj;
    Material whiteKeyOne;
    Material whiteKeyTwo;
    Material blackKeyOne;
    Material blackKeyTwo;

    public int tempo = 120;
    public int songLength = 0;
    public float noteScale = 1.0f;
    public float blackNoteWidth = 0.13125f;
    public float whiteNoteWidth = 0.225f;
    public float virtualPianoWidth = 1.575f;
    public float widthValue = 1.0f;
    public MidiFileContainer song;

    public List<Notes> notes = new List<Notes>();
    public List<int> noteTiming = new List<int>();
    public List<GameObject> instatiateNotes = new List<GameObject>();

    public Dictionary<int, int> tempNoteData = new Dictionary<int, int>();
    public Dictionary<int, List<KeyValuePair<int, bool>>> noteSetBySameTime = new Dictionary<int, List<KeyValuePair<int, bool>>>();
    public void Init()
    {
        noteObj = Resources.Load<GameObject>("Prefabs/Note");
        keyTextObj = Resources.Load<GameObject>("Prefabs/KeyText");

        whiteKeyOne = Resources.Load<Material>("Materials/WhiteChannel1");
        whiteKeyTwo = Resources.Load<Material>("Materials/WhiteChannel2");
        blackKeyOne = Resources.Load<Material>("Materials/BlackChannel1");
        blackKeyTwo = Resources.Load<Material>("Materials/BlackChannel2");
    }

    public void LoadAndInstantiateMidi(string fileName, GameObject obj)
    {
        TextAsset sourceFile = Resources.Load<TextAsset>($"Converts/{fileName}");
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
                    songLength = songLength < eventStartTime ? eventStartTime : songLength;
                    notes.Add(new Notes(track.sequence[j].midiEvent.data1, tempNoteData[track.sequence[j].midiEvent.data1], eventStartTime, trackNum));
                    tempNoteData.Remove(track.sequence[j].midiEvent.data1);
                }
            }
            if (notes.Count > 0 && trackNum == 0)
                trackNum++;
        }
        /*  바로 시작 방지용 코드, 검증 필요
        noteTiming.Add(0);
        noteSetBySameTime.Add(0, new List<KeyValuePair<int, bool>>());
        noteSetBySameTime[0].Add(new KeyValuePair<int, bool>(1, false));
        */
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
                instatiateNotes.Add(GameObject.Instantiate(noteObj, obj.transform));
                instatiateNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue, blackNoteWidth * widthValue, notes[i].deltaTime / (float)song.division * noteScale);
                instatiateNotes[i].transform.localPosition = new Vector3((blackNoteWidth / 2 + keyPos * blackNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0.2f, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
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
                instatiateNotes.Add(GameObject.Instantiate(noteObj, obj.transform));
                instatiateNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue, blackNoteWidth * widthValue, notes[i].deltaTime / (float)song.division * noteScale);
                instatiateNotes[i].transform.localPosition = new Vector3((whiteNoteWidth / 2 + keyPos * whiteNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
                if (notes[i].channel == 0)
                {
                    instatiateNotes[i].GetComponent<Renderer>().material = whiteKeyOne;
                }
                else
                {
                    instatiateNotes[i].GetComponent<Renderer>().material = whiteKeyTwo;
                }
            }

            GameObject tempKeyObject = GameObject.Instantiate(keyTextObj, obj.transform);
            tempKeyObject.transform.parent = instatiateNotes[i].transform;
            tempKeyObject.transform.localPosition = new Vector3(0, 0.55f, -0.5f);
            tempKeyObject.transform.position = new Vector3(tempKeyObject.transform.position.x, tempKeyObject.transform.position.y, tempKeyObject.transform.position.z + 0.1f);
            tempKeyObject.GetComponent<TextMeshPro>().text = GetKeyFromKeynum(notes[i].keyNum);
        }

        noteTiming.Sort();
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
