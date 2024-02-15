/* �̵� �Ŵ���
 * �ۼ� - �̿���
 * �̵� ������ ������� ���� ����ϴ� ��ü */

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
    Transform noteInstantiatePoint;

    public int tempo = 120;
    public int songLength = 0;
    public int totalDeltaTime = 0;
    public float noteScale = 1.0f;
    public float blackNoteWidth = 0.13125f;
    public float whiteNoteWidth = 0.225f;
    public float virtualPianoWidth = 1.575f;
    public float widthValue = 1.0f;
    public MidiFileContainer song;

    public List<Notes> notes = new List<Notes>();
    public List<int> noteTiming = new List<int>();
    public List<GameObject> instantiateNotes = new List<GameObject>();

    // ���� �ð��뿡 ���ÿ� �ľ��ϴ� ��Ʈ�� ����
    public Dictionary<int, List<KeyValuePair<int, bool>>> noteSetBySameTime = new Dictionary<int, List<KeyValuePair<int, bool>>>();
    // �� �ǹݺ� �ľ��ϴ� ��Ʈ�� ����
    public Dictionary<int, List<KeyValuePair<int, int>>> noteSetByKey = new Dictionary<int, List<KeyValuePair<int, int>>>();
    public int[] nextKeyIndex = new int[88];

    // �̵� �Ľ� �������� ��� ��. �ǻ��X
    Dictionary<int, int> _tempNoteData = new Dictionary<int, int>();
    public void Init()
    {
        noteObj = Resources.Load<GameObject>("Prefabs/Note");
        keyTextObj = Resources.Load<GameObject>("Prefabs/KeyText");

        whiteKeyOne = Resources.Load<Material>("Materials/WhiteChannel1");
        whiteKeyTwo = Resources.Load<Material>("Materials/WhiteChannel2");
        blackKeyOne = Resources.Load<Material>("Materials/BlackChannel1");
        blackKeyTwo = Resources.Load<Material>("Materials/BlackChannel2");
    }

    void CleanPrevDatas()
    {
        totalDeltaTime = 0;

        notes.Clear();
        noteTiming.Clear();
        instantiateNotes.Clear();
        noteSetBySameTime.Clear();
        noteSetByKey.Clear();
        _tempNoteData.Clear();

        nextKeyIndex = new int[88];
    }

    public void LoadAndInstantiateMidi(string fileName, GameObject obj)
    {
        CleanPrevDatas();

        GameObject tempNoteInstantiatePoint = new GameObject("Notes");
        tempNoteInstantiatePoint.transform.parent = Managers.ManagerInstance.transform;
        tempNoteInstantiatePoint.transform.localPosition = new Vector3(-1, 0, 0);
        noteInstantiatePoint = tempNoteInstantiatePoint.transform;

        TextAsset sourceFile = Resources.Load<TextAsset>($"Converts/{fileName}");
        song = MidiFileLoader.Load(sourceFile.bytes);
        while (song.tracks == null)     // Ȥ�� �� �񵿱� �ε� ��Ȳ�� ���
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
                eventStartTime += eventStartTime == -1 ? track.sequence[j].delta + 1 : track.sequence[j].delta;
                if (track.sequence[j].midiEvent.status == 144 && track.sequence[j].midiEvent.data2 > 0)
                {
                    if (_tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET))
                    {
                        notes.Add(new Notes(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET, _tempNoteData[track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET], eventStartTime, trackNum));
                        _tempNoteData.Remove(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET);
                    }
                    _tempNoteData.Add(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET, eventStartTime);
                }
                else if (track.sequence[j].midiEvent.status == 128 || (track.sequence[j].midiEvent.status == 144 && track.sequence[j].midiEvent.data2 == 0))
                {
                    if (!_tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET))
                        continue;
                    songLength = songLength < eventStartTime ? eventStartTime : songLength;
                    notes.Add(new Notes(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET, _tempNoteData[track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET], eventStartTime, trackNum));
                    totalDeltaTime += notes[notes.Count - 1].deltaTime;
                    _tempNoteData.Remove(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET);
                }
            }
            if (notes.Count > 0 && trackNum == 0)
                trackNum++;
        }

        notes.Sort();

        /*  �ٷ� ���� ������ �ڵ�, ���� �ʿ�
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

            if (!noteSetByKey.ContainsKey(notes[i].keyNum - 1))
            {
                noteSetByKey.Add(notes[i].keyNum - 1, new List<KeyValuePair<int, int>>());
            }
            noteSetByKey[notes[i].keyNum - 1].Add(new KeyValuePair<int, int>(notes[i].startTime, notes[i].endTime));

            string noteKeyStr = GetKeyFromKeynum(notes[i].keyNum);
            if (BlackKeyJudge(notes[i].keyNum))
            {
                int keyPos = NoteKeyPosOrder(notes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instantiateNotes.Add(GameObject.Instantiate(noteObj, noteInstantiatePoint));
                instantiateNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue, blackNoteWidth * widthValue, notes[i].deltaTime / (float)song.division * noteScale);
                instantiateNotes[i].transform.localPosition = new Vector3((blackNoteWidth / 2 + keyPos * blackNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0.2f, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
                if (notes[i].channel == 0)
                {
                    instantiateNotes[i].GetComponent<Renderer>().material = blackKeyOne;
                }
                else
                {
                    instantiateNotes[i].GetComponent<Renderer>().material = blackKeyTwo;
                }
            }
            else
            {
                int keyPos = NoteKeyPosOrder(notes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instantiateNotes.Add(GameObject.Instantiate(noteObj, noteInstantiatePoint));
                instantiateNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue, blackNoteWidth * widthValue, notes[i].deltaTime / (float)song.division * noteScale);
                instantiateNotes[i].transform.localPosition = new Vector3((whiteNoteWidth / 2 + keyPos * whiteNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
                if (notes[i].channel == 0)
                {
                    instantiateNotes[i].GetComponent<Renderer>().material = whiteKeyOne;
                }
                else
                {
                    instantiateNotes[i].GetComponent<Renderer>().material = whiteKeyTwo;
                }
            }

            GameObject tempKeyObject = GameObject.Instantiate(keyTextObj, noteInstantiatePoint);
            tempKeyObject.transform.parent = instantiateNotes[i].transform;
            tempKeyObject.transform.localPosition = new Vector3(0, 0.55f, -0.5f);
            tempKeyObject.transform.position = new Vector3(tempKeyObject.transform.position.x, tempKeyObject.transform.position.y, tempKeyObject.transform.position.z + 0.1f);
            tempKeyObject.GetComponent<TextMeshPro>().text = GetKeyFromKeynum(notes[i].keyNum);
        }

        noteTiming.Sort();
    }

    // �и��� �����͸� �̿��� �� ���� ���
    int CalcTempoWithRatio(float ratio)
    {
        int resultTempo = -1;
        float temp = Datas.DEFAULT_TEMPO * ratio;
        resultTempo = temp - (int)temp < 0.5f ? (int)temp : (int)temp + 1;
        return resultTempo;
    }

    string GetKeyFromKeynum(int keyNum)
    {
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
