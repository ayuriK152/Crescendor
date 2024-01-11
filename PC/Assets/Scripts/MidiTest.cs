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
    public GameObject noteObj;
    public Transform noteInstantiatePoint;

    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public float noteScale = 1.0f;
    public float scrollSpeed = 1.0f;
    public int tempo = 120;
    public float currentDeltaTime = 0.0f;

    static bool[] keyChecks = new bool[88];
    static InputDevice _inputDevice;

    bool _isInputTiming = false;

    List<GameObject> instatiateNotes = new List<GameObject>();
    List<Notes> notes = new List<Notes>();
    Dictionary<int, int> tempNoteData = new Dictionary<int, int>();
    List<int> noteTiming = new List<int>();
    Dictionary<int, List<KeyValuePair<int, bool>>> noteSetBySameTime = new Dictionary<int, List<KeyValuePair<int, bool>>>();
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
                noteSetBySameTime.Add(notes[i].startTime, new List<KeyValuePair<int, bool>>());
            }
            noteSetBySameTime[notes[i].startTime].Add(new KeyValuePair<int, bool>(notes[i].keyNum - 1, false));

            instatiateNotes.Add(Instantiate(noteObj, noteInstantiatePoint));
            instatiateNotes[i].transform.localScale = new Vector3(0.5f, 0.5f, notes[i].deltaTime / (float)song.division * noteScale);
            instatiateNotes[i].transform.localPosition = new Vector3((notes[i].keyNum - DEFAULT_C3_POSITION) * 0.5f, 0, (notes[i].startTime + notes[i].deltaTime / 2.0f) / song.division * noteScale);
        }

        noteTiming.Sort();

        try
        {
            _inputDevice = InputDevice.GetByName("Digital Piano");
            _inputDevice.EventReceived += OnEventReceived;
            _inputDevice.StartEventsListening();
            Debug.Log(_inputDevice.IsListeningForEvents);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
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
            transform.position = new Vector3(0, 0, -currentDeltaTime / song.division * noteScale);
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
            noteSetBySameTime[noteTiming[currentNoteIndex]][i] = new KeyValuePair<int, bool>(noteSetBySameTime[noteTiming[currentNoteIndex]][i].Key, keyChecks[noteSetBySameTime[noteTiming[currentNoteIndex]][i].Key]);
            if (!noteSetBySameTime[noteTiming[currentNoteIndex]][i].Value)
                return;
        }
        IncreaseCurrentNoteIndex();
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

    static void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if (e.Event.EventType != MidiEventType.ActiveSensing)
        {
            NoteEvent noteEvent = e.Event as NoteEvent;
            if (noteEvent.Velocity != 0)
            {
                keyChecks[noteEvent.NoteNumber - 1] = true;
                Debug.Log(keyChecks[noteEvent.NoteNumber - 1]);
            }
            else if (noteEvent.Velocity == 0)
            {
                keyChecks[noteEvent.NoteNumber - 1] = false;
                Debug.Log(keyChecks[noteEvent.NoteNumber - 1]);
            }
        }
    }
}
