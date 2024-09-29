/* 미디 매니저
 * 작성 - 이원섭
 * 미디 파일의 입출력을 위해 사용하는 객체 */

using SmfLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using static Define;
using static Datas;

public class MidiManager
{
    GameObject noteObj;
    GameObject keyTextObj;
    GameObject barLineObj;
    Material whiteKeyOne;
    Material whiteKeyTwo;
    Material whiteKeyThree;
    Material blackKeyOne;
    Material blackKeyTwo;
    Material blackKeyThree;
    Transform noteInstantiatePoint;
    Transform replayInstantiatePoint;

    public int tempo = 120;
    public List<int> barTiming = new List<int>();
    public KeyValuePair<int, int> beat = new KeyValuePair<int, int>(4, 4);
    public int songLengthDelta = 0;
    public float songLengthSecond = 0;
    public int totalDeltaTime = 0;
    public float noteScaleZ = 1.0f;
    public float blackNoteWidth = 0.13125f;
    public float whiteNoteWidth = 0.225f;
    public float notePositionOffset = -10.5f;
    public float virtualPianoWidth = 1.575f;
    public float widthValue = 4.0f;
    public MidiFileContainer song;

    public List<Notes> notes = new List<Notes>();
    public List<int> noteTiming = new List<int>();
    public List<GameObject> instantiateNotes = new List<GameObject>();

    // 같은 시간대에 동시에 쳐야하는 노트들 모음
    public Dictionary<int, List<KeyValuePair<int, bool>>> noteSetBySameTime = new Dictionary<int, List<KeyValuePair<int, bool>>>();
    // 각 건반별 쳐야하는 노트들 모음
    public Dictionary<int, List<KeyValuePair<int, int>>> noteSetByKey = new Dictionary<int, List<KeyValuePair<int, int>>>();
    public int[] nextKeyIndex = new int[88];

    public List<Notes> replayNotes = new List<Notes>();
    public List<int> replayNoteTiming = new List<int>();
    public List<GameObject> instantiateReplayNotes = new List<GameObject>();

    // 미디 파싱 로직에서 잠깐 씀. 실사용X
    Dictionary<int, int> _tempNoteData = new Dictionary<int, int>();
    public void Init()
    {
        noteObj = Resources.Load<GameObject>("Prefabs/Note");
        keyTextObj = Resources.Load<GameObject>("Prefabs/KeyText");
        barLineObj = Resources.Load<GameObject>("Prefabs/BarLine");

        whiteKeyOne = Resources.Load<Material>("Materials/WhiteChannel1");
        whiteKeyTwo = Resources.Load<Material>("Materials/WhiteChannel2");
        whiteKeyThree = Resources.Load<Material>("Materials/WhiteChannel3");
        blackKeyOne = Resources.Load<Material>("Materials/BlackChannel1");
        blackKeyTwo = Resources.Load<Material>("Materials/BlackChannel2");
        blackKeyThree = Resources.Load<Material>("Materials/BlackChannel3");

        if (!PlayerPrefs.HasKey("user_ScrollSpeed"))
        {
            PlayerPrefs.SetFloat("user_ScrollSpeed", 1.0f);
        }
        noteScaleZ = PlayerPrefs.GetFloat("user_ScrollSpeed");
    }

    void CleanPrevDatas()
    {
        totalDeltaTime = 0;
        songLengthDelta = 0;
        songLengthSecond = 0;

        notes.Clear();
        noteTiming.Clear();
        instantiateNotes.Clear();
        noteSetBySameTime.Clear();
        noteSetByKey.Clear();
        _tempNoteData.Clear();

        replayNotes.Clear();
        replayNoteTiming.Clear();
        instantiateReplayNotes.Clear();
        barTiming.Clear();

        nextKeyIndex = new int[88];
    }

    public void LoadMidi(string fileName)
    {
        CleanPrevDatas();

        TextAsset sourceFile;
        if (Managers.Song.selectedSong.curriculum == Curriculum.None)
        {
            sourceFile = Resources.Load<TextAsset>($"Converts/{fileName}");
        }
        else
        {
            sourceFile = Resources.Load<TextAsset>($"Converts/{Managers.Song.selectedSong.curriculum}/{fileName}");
        }
        song = MidiFileLoader.Load(sourceFile.bytes);
        while (song.tracks == null)     // 혹시 모를 비동기 로드 상황에 대비
        {
            Debug.Log("Now MIDI data on loading..");
        }
        Debug.Log("MIDI data loaded!");

        song.tempoMap.Sort((Tempo a, Tempo b) => { return a.deltaTime - b.deltaTime; });
        song.beatMap.Sort((Beat a, Beat b) => { return a.deltaTime - b.deltaTime; });

        tempo = CalcTempoWithRatio(Datas.DEFAULT_QUARTER_NOTE_MILLISEC / song.tempoMap[0].milliSecond);
        if (song.beatMap.Count == 0)
        {
            beat = new KeyValuePair<int, int>(4, 4);
            song.beatMap.Add(new Beat(0, 4, 4, 24, 8));
        }
        else
            beat = new KeyValuePair<int, int>(song.beatMap[0].numerator, song.beatMap[0].denominator);
    }

    public void LoadAndInstantiateMidi(string fileName)
    {
        SwapNoteAlphaValue(Managers.Scene.currentScene == Scene.ReplayModScene);
        LoadMidi(fileName);

        GameObject tempNoteInstantiatePoint = new GameObject("Notes");
        tempNoteInstantiatePoint.transform.parent = Managers.ManagerInstance.transform;
        tempNoteInstantiatePoint.transform.localPosition = new Vector3(-4, 0, 0);
        noteInstantiatePoint = tempNoteInstantiatePoint.transform;

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

                // 16진수 8n은 음표 끝, 9n은 음표 시작
                if (track.sequence[j].midiEvent.status >= 0x90 && track.sequence[j].midiEvent.status <= 0x9f && track.sequence[j].midiEvent.data2 > 0)
                {
                    if (_tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET))
                    {
                        notes.Add(new Notes(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET, _tempNoteData[track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET], eventStartTime, trackNum));
                        _tempNoteData.Remove(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET);
                    }
                    _tempNoteData.Add(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET, eventStartTime);
                }
                else if (track.sequence[j].midiEvent.status >= 0x80 && track.sequence[j].midiEvent.status <= 0x8f || (track.sequence[j].midiEvent.status >= 0x90 && track.sequence[j].midiEvent.status <= 0x9f && track.sequence[j].midiEvent.data2 == 0))
                {
                    if (!_tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET))
                        continue;
                    songLengthDelta = songLengthDelta < eventStartTime ? eventStartTime : songLengthDelta;
                    notes.Add(new Notes(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET, _tempNoteData[track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET], eventStartTime, trackNum));
                    totalDeltaTime += notes[notes.Count - 1].deltaTime;
                    _tempNoteData.Remove(track.sequence[j].midiEvent.data1 - DEFAULT_KEY_NUM_OFFSET);
                }
            }
            if (notes.Count > 0 && trackNum == 0)
                trackNum++;
        }

        songLengthSecond = (songLengthDelta / (float)song.division) / (tempo / 60.0f);

        notes.Sort();

        int beatMapIndex = 0;
        for (int i = 0; i < songLengthDelta;)
        {
            if (song.beatMap.Count - 1 > beatMapIndex)
            {
                if (song.beatMap[beatMapIndex + 1].deltaTime <= i)
                {
                    beatMapIndex += 1;
                }
            }

            barTiming.Add(i + song.division * (song.beatMap[beatMapIndex].numerator * 4 / song.beatMap[beatMapIndex].denominator));
            i += (int)(song.division * (song.beatMap[beatMapIndex].numerator * 4.0f / song.beatMap[beatMapIndex].denominator));
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
                instantiateNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue * 0.1f , blackNoteWidth * widthValue, notes[i].deltaTime / (float)song.division * noteScaleZ * 0.1f);
                instantiateNotes[i].transform.localPosition = new Vector3((blackNoteWidth / 2 + keyPos * blackNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0.1f, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScaleZ);
                if (notes[i].channel == 0)
                {
                    instantiateNotes[i].transform.GetChild(0).GetComponent<Renderer>().material = blackKeyOne;
                }
                else
                {
                    instantiateNotes[i].transform.GetChild(0).GetComponent<Renderer>().material = blackKeyTwo;
                }
            }
            else
            {
                int keyPos = NoteKeyPosOrder(notes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instantiateNotes.Add(GameObject.Instantiate(noteObj, noteInstantiatePoint));
                instantiateNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue * 0.1f, blackNoteWidth * widthValue, notes[i].deltaTime / (float)song.division * noteScaleZ * 0.1f);
                instantiateNotes[i].transform.localPosition = new Vector3((whiteNoteWidth / 2 + keyPos * whiteNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScaleZ);
                if (notes[i].channel == 0)
                {
                    instantiateNotes[i].transform.GetChild(0).GetComponent<Renderer>().material = whiteKeyOne;
                }
                else
                {
                    instantiateNotes[i].transform.GetChild(0).GetComponent<Renderer>().material = whiteKeyTwo;
                }
            }

            GameObject tempKeyObject = GameObject.Instantiate(keyTextObj, noteInstantiatePoint);
            tempKeyObject.transform.parent = instantiateNotes[i].transform;
            tempKeyObject.transform.localPosition = new Vector3(0, 0.1f, -5.0f);
            tempKeyObject.transform.position = new Vector3(tempKeyObject.transform.position.x, tempKeyObject.transform.position.y, tempKeyObject.transform.position.z + 0.1f);
            tempKeyObject.GetComponent<TextMeshPro>().text = GetKeyFromKeynum(notes[i].keyNum);
        }

        noteTiming.Sort();

        for (int i = 0; i < barTiming.Count; i++)
        {
            GameObject.Instantiate(barLineObj, noteInstantiatePoint).transform.localPosition = new Vector3(0, 0, barTiming[i] / song.division * noteScaleZ);
        }
    }

    public void LoadAndInstantiateReplay(string replayFile, bool isPath)
    {
        GameObject tempNoteInstantiatePoint = new GameObject("ReplayNotes");
        tempNoteInstantiatePoint.transform.parent = Managers.ManagerInstance.transform;
        tempNoteInstantiatePoint.transform.localPosition = new Vector3(-1, -2, 0);
        replayInstantiatePoint = tempNoteInstantiatePoint.transform;
        Define.UserReplayRecord userReplayRecord = null;

        if (isPath)
        {
            userReplayRecord = JsonConvert.DeserializeObject<Define.UserReplayRecord>(File.ReadAllText($"{Application.dataPath}/RecordReplay/{replayFile}.json"));
        }
        else
        {
            userReplayRecord = JsonConvert.DeserializeObject<Define.UserReplayRecordForParse>(replayFile).ParseToOrigin(Managers.Data.rankRecord.score);
        }

        List<int> keyNums = userReplayRecord.noteRecords.Keys.ToList<int>();
        keyNums.Sort();

        foreach(int i in keyNums)
        {
            for (int j = 0; j < userReplayRecord.noteRecords[i].Count; j++)
            {
                replayNotes.Add(new Notes(i, userReplayRecord.noteRecords[i][j].Key, userReplayRecord.noteRecords[i][j].Value, 1));
            }
        }

        for (int i = 0; i < replayNotes.Count; i++)
        {
            string noteKeyStr = GetKeyFromKeynum(replayNotes[i].keyNum);
            if (BlackKeyJudge(replayNotes[i].keyNum))
            {
                int keyPos = NoteKeyPosOrder(replayNotes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instantiateReplayNotes.Add(GameObject.Instantiate(noteObj, replayInstantiatePoint));
                instantiateReplayNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue * 0.1f, blackNoteWidth * widthValue, replayNotes[i].deltaTime / (float)song.division * noteScaleZ * 0.1f);
                instantiateReplayNotes[i].transform.localPosition = new Vector3((blackNoteWidth / 2 + keyPos * blackNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0.2f, (replayNotes[i].startTime + replayNotes[i].deltaTime / 2.0f) / song.division * noteScaleZ);
                instantiateReplayNotes[i].GetComponent<Renderer>().material = blackKeyThree;
            }
            else
            {
                int keyPos = NoteKeyPosOrder(replayNotes[i].keyNum) - 1;
                int keyOffset = int.Parse(noteKeyStr[noteKeyStr.Length - 1].ToString()) - 3;
                instantiateReplayNotes.Add(GameObject.Instantiate(noteObj, replayInstantiatePoint));
                instantiateReplayNotes[i].transform.localScale = new Vector3(blackNoteWidth * widthValue * 0.1f, blackNoteWidth * widthValue, replayNotes[i].deltaTime / (float)song.division * noteScaleZ * 0.1f);
                instantiateReplayNotes[i].transform.localPosition = new Vector3((whiteNoteWidth / 2 + keyPos * whiteNoteWidth + keyOffset * virtualPianoWidth) * widthValue, 0, (replayNotes[i].startTime + replayNotes[i].deltaTime / 2.0f) / song.division * noteScaleZ);
                instantiateReplayNotes[i].GetComponent<Renderer>().material = whiteKeyThree;
            }

            GameObject tempKeyObject = GameObject.Instantiate(keyTextObj, replayInstantiatePoint);
            tempKeyObject.transform.parent = instantiateReplayNotes[i].transform;
            tempKeyObject.transform.localPosition = new Vector3(0, 0.55f, -0.5f);
            tempKeyObject.transform.position = new Vector3(tempKeyObject.transform.position.x, tempKeyObject.transform.position.y, tempKeyObject.transform.position.z + 0.1f);
            tempKeyObject.GetComponent<TextMeshPro>().text = GetKeyFromKeynum(replayNotes[i].keyNum);
        }
    }

    void SwapNoteAlphaValue(bool isReplay)
    {
        if (isReplay)
        {
            whiteKeyOne.color = new Color(whiteKeyOne.color.r, whiteKeyOne.color.g, whiteKeyOne.color.b, 0.5f);
            whiteKeyTwo.color = new Color(whiteKeyTwo.color.r, whiteKeyTwo.color.g, whiteKeyTwo.color.b, 0.5f);
            blackKeyOne.color = new Color(blackKeyOne.color.r, blackKeyOne.color.g, blackKeyOne.color.b, 0.5f);
            blackKeyTwo.color = new Color(blackKeyTwo.color.r, blackKeyTwo.color.g, blackKeyTwo.color.b, 0.5f);
        }
        else
        {
            whiteKeyOne.color = new Color(whiteKeyOne.color.r, whiteKeyOne.color.g, whiteKeyOne.color.b, 1);
            whiteKeyTwo.color = new Color(whiteKeyTwo.color.r, whiteKeyTwo.color.g, whiteKeyTwo.color.b, 1);
            blackKeyOne.color = new Color(blackKeyOne.color.r, blackKeyOne.color.g, blackKeyOne.color.b, 1);
            blackKeyTwo.color = new Color(blackKeyTwo.color.r, blackKeyTwo.color.g, blackKeyTwo.color.b, 1);
        }
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

    public bool BlackKeyJudge(int keyNum)
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

    public int NoteKeyPosOrder(int keyNum)
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
