using SmfLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;
using static Datas;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class MidiTest : MonoBehaviour
{
    public TextAsset sourceFile;
    public MidiFileContainer song;
    public GameObject noteObj;
    public Transform noteInstantiatePoint;

    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public float noteScale = 1.0f;
    public float scrollSpeed = 1.0f;
    public int tempo = 120;
    public float currentDeltaTime = 0.0f;

    List<GameObject> instatiateNotes = new List<GameObject>();
    List<Notes> notes = new List<Notes>();
    Dictionary<int, int> tempNoteData = new Dictionary<int, int>();
    List<int> noteTiming = new List<int>();
    Dictionary<int, List<Notes>> noteSetBySameTime = new Dictionary<int, List<Notes>>();
    int currentNoteIndex = 0;

    void Awake()
    {
        sourceFile = Resources.Load<TextAsset>("Converts/b9IsAwesome");
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
        for (int i = 0; i < song.tracks.Count; i++)
        {
            MidiTrack track = song.tracks[i];
            int eventStartTime = -1;
            int deltaTime = 0;
            for (int j = 0; j < track.sequence.Count; j++)
            {
                deltaTime = track.sequence[j].delta != 0 ? track.sequence[j].delta : deltaTime;
                eventStartTime += eventStartTime == -1 ? 1 : track.sequence[j].delta;
                if (track.sequence[j].midiEvent.status == 144 && track.sequence[j].midiEvent.data2 > 0)
                {
                    if (tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1))
                    {
                        notes.Add(new Notes(track.sequence[j].midiEvent.data1, tempNoteData[track.sequence[j].midiEvent.data1], eventStartTime));
                        tempNoteData.Remove(track.sequence[j].midiEvent.data1);
                    }
                    tempNoteData.Add(track.sequence[j].midiEvent.data1, eventStartTime);
                }
                else if (track.sequence[j].midiEvent.status == 128 || (track.sequence[j].midiEvent.status == 144 && track.sequence[j].midiEvent.data2 == 0))
                {
                    if (!tempNoteData.ContainsKey(track.sequence[j].midiEvent.data1))
                        continue;
                    notes.Add(new Notes(track.sequence[j].midiEvent.data1, tempNoteData[track.sequence[j].midiEvent.data1], eventStartTime));
                    tempNoteData.Remove(track.sequence[j].midiEvent.data1);
                }
            }
        }
        for (int i = 0; i < notes.Count; i++)
        {
            if (!noteSetBySameTime.ContainsKey(notes[i].startTime))
            {
                noteTiming.Add(notes[i].startTime);
                noteSetBySameTime.Add(notes[i].startTime, new List<Notes>());
            }
            noteSetBySameTime[notes[i].startTime].Add(notes[i]);

            instatiateNotes.Add(Instantiate(noteObj, noteInstantiatePoint));
            instatiateNotes[i].transform.localScale = new Vector3(0.5f, 0.5f, notes[i].deltaTime / (float)song.division * noteScale);
            instatiateNotes[i].transform.localPosition = new Vector3((notes[i].keyNum - DEFAULT_C3_POSITION) * 0.5f, 0, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
        }

        noteTiming.Sort();
    }

    void Update()
    {
        Scroll();
    }

    void Scroll()
    {
        if (noteTiming[currentNoteIndex] <= currentDeltaTime)
        {
            currentDeltaTime = noteTiming[currentNoteIndex];
            transform.position = new Vector3(0, 0, -currentDeltaTime / song.division * noteScale);
            return;
        }
        currentDeltaTime += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / song.tempoMap[0].milliSecond * song.division * Time.deltaTime;
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / song.tempoMap[0].milliSecond * noteScale * Time.deltaTime));
    }

    public void IncreaseCurrentNoteIndex()
    {
        currentNoteIndex += 1;
    }

    // 밀리초 데이터를 이용한 곡 템포 계산
    int CalcTempoWithRatio(float ratio)
    {
        int resultTempo = -1;
        float temp = Datas.DEFAULT_TEMPO * ratio;
        resultTempo = temp - (int)temp < 0.5f ? (int)temp : (int)temp + 1;
        return resultTempo;
    }
}
