using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using static Define;
using static Datas;
using System.IO;
using Newtonsoft.Json;

public class ActualModController : IngameController
{
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public int tempo = 120;
    public float scrollSpeed = 1.0f;
    public float notePosOffset = -2.625f;
    public float noteScale = 1.5f;
    public float widthValue = 1.5f;
    public string songTitle;

    public int passedNote;
    public int totalNote;
    public int currentFail;
    public int totalAcc;
    public float currentAcc = 1;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    int[] _initInputTiming = new int[88];
    int[] lastInputTiming = new int[88];
    ActualModUIController _uiController;

    Dictionary<int, List<KeyValuePair<int, int>>> noteRecords;

    public void Init()
    {
        songTitle = PlayerPrefs.GetString("trans_SongTitle");

        passedNote = 0;
        totalNote = 0;
        currentFail = 0;

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        for (int i = 0; i < 88; i++)
        {
            _initInputTiming[i] = -1;
        }

        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi(songTitle);

        totalNote = Managers.Midi.notes.Count;
        totalAcc = Managers.Midi.totalDeltaTime;
        tempo = Managers.Midi.tempo;

        _uiController = Managers.UI.currentUIController as ActualModUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = songTitle.Replace("_", " ");
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songBpmTMP.text = $"{tempo}";
        _uiController.songBeatTMP.text = $"4/4";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLengthDelta;

        noteRecords = new Dictionary<int, List<KeyValuePair<int, int>>>();

        // Managers.Input.keyAction -= InputKeyEvent;
        // Managers.Input.keyAction += InputKeyEvent;

        if (Managers.Input.inputDevice != null)
        {
            Managers.Input.inputDevice.EventReceived -= OnEventReceived;
            Managers.Input.inputDevice.EventReceived += OnEventReceived;
        }

        base.Init();

        Managers.InitManagerPosition();
    }

    void Update()
    {
        Scroll();
        StartCoroutine(CheckNotesStatus());
        if (currentDeltaTime > Managers.Midi.songLengthDelta && !isSceneOnSwap)
            TempSwapScene();
        StartCoroutine(ToggleKeyHighlight());
    }

    bool isSceneOnSwap = false;

    void TempSwapScene()
    {
        if (!PlayerPrefs.HasKey("trans_SongTitle"))
            PlayerPrefs.SetString("trans_SongTitle", "");
        PlayerPrefs.SetString("trans_SongTitle", songTitle);

        if (!PlayerPrefs.HasKey("trans_FailMount"))
            PlayerPrefs.SetInt("trans_FailMount", 0);
        PlayerPrefs.SetInt("trans_FailMount", currentFail);
        if (!PlayerPrefs.HasKey("trans_OutlinerMount"))
            PlayerPrefs.SetInt("trans_OutlinerMount", 0);
        PlayerPrefs.SetInt("trans_OutlinerMount", 0);

        Managers.Data.userReplayRecord = new Define.UserReplayRecord(noteRecords, tempo, songTitle, currentAcc);
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.ResultScene);
        isSceneOnSwap = true;
    }

    void Scroll()
    {
        currentDeltaTimeF += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.song.division * Time.deltaTime;
        SyncDeltaTime(false);
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.noteScale * Time.deltaTime));
    }

    public void DisconnectPiano()
    {
        Managers.Input.inputDevice.StopEventsListening();
    }

    public void SyncDeltaTime(bool isIntToFloat)
    {
        if (isIntToFloat)
        {
            currentDeltaTimeF = currentDeltaTime;
        }
        else
        {
            currentDeltaTime = currentDeltaTimeF - (int)currentDeltaTimeF < 0.5f ? (int)currentDeltaTimeF : (int)currentDeltaTimeF + 1;
        }
        _uiController.songTimeSlider.SetValueWithoutNotify(currentDeltaTime);
    }

    IEnumerator CheckNotesStatus()
    {
        for (int i = 0; i < 88; i++)
        {
            if (Managers.Midi.noteSetByKey.ContainsKey(i))
            {
                if (Managers.Midi.noteSetByKey[i].Count > 0 && Managers.Midi.noteSetByKey[i].Count > Managers.Midi.nextKeyIndex[i])
                {
                    StartCoroutine(CheckNotesStatus(i));
                }
            }
        }

        yield return null;
    }

    IEnumerator CheckNotesStatus(int keyNum)
    {
        if (lastInputTiming[keyNum] == 0 && Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key != 0)
            lastInputTiming[keyNum] = Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key;

        if (Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value - currentDeltaTime < 0)
        {
            if (lastInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value)
            {
                if (!Managers.Input.keyChecks[keyNum])
                {
                    currentFail += Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value - lastInputTiming[keyNum];
                    currentAcc = (totalAcc - currentFail) / (float)totalAcc;
                }
            }
            Managers.Midi.nextKeyIndex[keyNum]++;
            // Debug.Log(Managers.Midi.nextKeyIndex[keyNum]);
        }

        if (Managers.Midi.noteSetByKey[keyNum].Count > Managers.Midi.nextKeyIndex[keyNum])
        {
            if (Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key - currentDeltaTime < 0)
            {
                if (lastInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key)
                    lastInputTiming[keyNum] = Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key;

                if (!Managers.Input.keyChecks[keyNum] || _initInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key)
                {
                    currentFail += currentDeltaTime - lastInputTiming[keyNum];
                }

                lastInputTiming[keyNum] = currentDeltaTime;
                currentAcc = (totalAcc - currentFail) / (float)totalAcc;
            }
        }

        _uiController.UpdateAccuracy();

        yield return null;
    }

    void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if (e.Event.EventType != MidiEventType.ActiveSensing)
        {
            NoteEvent noteEvent = e.Event as NoteEvent;

            // 노트 입력 시작
            if (noteEvent.Velocity != 0)
            {
                _initInputTiming[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET] = currentDeltaTime;
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET] = true;
                if (!noteRecords.ContainsKey(noteEvent.NoteNumber - DEFAULT_KEY_NUM_OFFSET))
                    noteRecords.Add(noteEvent.NoteNumber - DEFAULT_KEY_NUM_OFFSET, new List<KeyValuePair<int, int>>());
                Debug.Log(noteEvent);
            }
            // 노트 입력 종료
            else if (noteEvent.Velocity == 0)
            {
                noteRecords[noteEvent.NoteNumber - DEFAULT_KEY_NUM_OFFSET].Add(new KeyValuePair<int, int>(_initInputTiming[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET], currentDeltaTime));
                _initInputTiming[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET] = -1;
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET] = false;
            }
        }
    }

    IEnumerator JudgeInput(int keyNum)
    {
        yield return null;
    }
}
