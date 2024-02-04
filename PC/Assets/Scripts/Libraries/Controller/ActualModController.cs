using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using static Define;
using static Datas;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ActualModController : MonoBehaviour
{
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public int tempo = 120;
    public float scrollSpeed = 1.0f;
    public float notePosOffset = 0.0f;
    public float noteScale = 1.0f;
    public float widthValue = 1.0f;
    public string songTitle;

    public int passedNote;
    public int totalNote;
    public int currentFail;
    public int totalAcc;
    public float currentAcc = 1;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    int[] lastInputTiming = new int[88];
    ActualModUIController _uiController;

    Dictionary<int, KeyValuePair<int, int>> noteRecords;

    public void Init()
    {
        passedNote = 0;
        totalNote = 0;
        currentFail = 0;

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi(songTitle, gameObject);

        totalNote = Managers.Midi.notes.Count;
        totalAcc = Managers.Midi.totalDeltaTime;
        tempo = Managers.Midi.tempo;

        _uiController = Managers.UI.currentUIController as ActualModUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = songTitle;
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songBpmTMP.text = $"{tempo}";
        _uiController.songBeatTMP.text = $"4/4";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLength;

        noteRecords = new Dictionary<int, KeyValuePair<int, int>>();

        // Managers.Input.keyAction -= InputKeyEvent;
        // Managers.Input.keyAction += InputKeyEvent;

        if (Managers.Input.inputDevice != null)
        {
            Managers.Input.inputDevice.EventReceived -= OnEventReceived;
            Managers.Input.inputDevice.EventReceived += OnEventReceived;
        }

        Managers.Input.keyChecks[59] = true;
    }

    void Update()
    {
        Scroll();
        StartCoroutine(CheckNotesStatus());
        if (currentDeltaTime > Managers.Midi.songLength && flag)
            TempSwapScene();
    }

    bool flag = true;

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

        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.ResultScene);
        flag = false;
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

                if (!Managers.Input.keyChecks[keyNum])
                {
                    currentFail += currentDeltaTime - lastInputTiming[keyNum];
                }
                lastInputTiming[keyNum] = currentDeltaTime;
                currentAcc = (totalAcc - currentFail) / (float)totalAcc;
            }
        }

        if (Managers.Midi.songLength < currentDeltaTime)
            Debug.Log($"{Convert.ToInt32(currentAcc * 10000) / 100.0f}");

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
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1] = true;
                if (!noteRecords.ContainsKey(noteEvent.NoteNumber - 1))
                    noteRecords.Add(noteEvent.NoteNumber - 1, new KeyValuePair<int, int>(currentDeltaTime, -1));
                else
                    noteRecords[noteEvent.NoteNumber - 1] = new KeyValuePair<int, int>(currentDeltaTime, -1);
                Debug.Log(noteEvent);
            }
            // 노트 입력 종료
            else if (noteEvent.Velocity == 0)
            {
                noteRecords[noteEvent.NoteNumber - 1] = new KeyValuePair<int, int>(noteRecords[noteEvent.NoteNumber - 1].Key, currentDeltaTime);
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1] = false;
            }
        }
    }

    IEnumerator JudgeInput(int keyNum)
    {
        yield return null;
    }
}
