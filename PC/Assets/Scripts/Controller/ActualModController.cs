using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using static Datas;
using System;

public class ActualModController : IngameController
{
    #region Public Members
    public int currentCorrect;
    public int currentFail;
    public int totalAcc;
    public int currentMetronomeIdx = -1;
    public float currentAcc = 1;
    #endregion

    #region Private Members
    private int[] _lastInputTiming = new int[88];
    private int noteCheckCoroutineCnt = 0;

    private bool _isSceneOnSwap = false;
    private bool _isIntro = true;

    private Dictionary<int, List<KeyValuePair<int, int>>> _noteRecords;
    #endregion

    public void Init()
    {
        base.Init();

        currentCorrect = 0;
        currentFail = 0;

        totalAcc = Managers.Midi.totalDeltaTime;
        tempo = Managers.Midi.tempo;

        _uiController = Managers.UI.currentUIController as ActualModUIController;
        (_uiController as ActualModUIController).BindIngameUI();
        _uiController.songTitleTMP.text = songTitle.Replace("_", " ");
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songTempoTMP.text = $"{tempo}";
        _uiController.songBeatTMP.text = $"{Managers.Midi.beat.Key}/{Managers.Midi.beat.Value}";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLengthDelta;

        _noteRecords = new Dictionary<int, List<KeyValuePair<int, int>>>();

        if (Managers.Input.inputDevice != null)
        {
            Managers.Input.inputDevice.EventReceived -= OnEventReceived;
            Managers.Input.inputDevice.EventReceived += OnEventReceived;
        }

        try
        {
            StartCoroutine(sheetController.ShowSheetAtIndex($"SheetDatas/{songTitle}", 0));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        Managers.InitManagerPosition();
        StartCoroutine(DelayForSeconds(3));
    }

    void Update()
    {
        Scroll();
        CheckNotesStatus();
        if (currentDeltaTime > Managers.Midi.songLengthDelta && !_isSceneOnSwap)
            SwapScene();
        StartCoroutine(ToggleKeyHighlight());
    }

    //처음 게임 시작 전에 딜레이
    public IEnumerator DelayForSeconds(float seconds)
    {
        for (int i = (int)seconds / 1; i > 0; i--)
        {
            (_uiController as ActualModUIController).introCountTMP.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        (_uiController as ActualModUIController).introCountTMP.gameObject.SetActive(false);
        _isIntro = false;
    }

    //Result 씬으로 넘어갈때 반드시 거치는 메소드
    void SwapScene()
    {
        Debug.Log(noteCheckCoroutineCnt);
        if (noteCheckCoroutineCnt > 0)
            return;

        Managers.Input.keyAction = null;
        if (!PlayerPrefs.HasKey("trans_SongTitle"))
            PlayerPrefs.SetString("trans_SongTitle", "");
        PlayerPrefs.SetString("trans_SongTitle", songTitle);

        if (!PlayerPrefs.HasKey("trans_FailMount"))
            PlayerPrefs.SetInt("trans_FailMount", 0);
        PlayerPrefs.SetInt("trans_FailMount", currentFail);
        if (!PlayerPrefs.HasKey("trans_OutlinerMount"))
            PlayerPrefs.SetInt("trans_OutlinerMount", 0);
        PlayerPrefs.SetInt("trans_OutlinerMount", 0);

        Managers.Data.userReplayRecord = new Define.UserReplayRecord(_noteRecords, tempo, songTitle, currentAcc);
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.ResultScene);
        _isSceneOnSwap = true;
    }

    void Scroll()
    {
        if (_isIntro)
            return;
        // 메트로놈
        if (currentMetronomeIdx < (currentDeltaTime + (Managers.Midi.song.division / 10) * Managers.Sound.metronomeOffset) / Managers.Midi.song.division)
        {
            if ((Managers.Sound.metronomeOffset != 0 && Managers.Midi.barTiming[_currentBarIdx] <= currentDeltaTime + (Managers.Midi.song.division / 10) * Managers.Sound.metronomeOffset) ||
                currentMetronomeIdx == -1)
            {
                Managers.Sound.metronomeAction.Invoke(true);
            }
            else
            {
                Managers.Sound.metronomeAction.Invoke(false);
            }
            currentMetronomeIdx++;
        }

        //템포에 따라 스크롤 되는 부분
        currentDeltaTimeF += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * tempoSpeed * Managers.Midi.song.division * Time.deltaTime;
        SyncDeltaTime(false);
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * tempoSpeed * Managers.Midi.noteScaleZ * Time.deltaTime));
        UpdateTempo();
        UpdateBeat();
        UpdateBar();
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

    void CheckNotesStatus()
    {
        for (int i = 0; i < 88; i++)
        {
            if (Managers.Midi.noteSetByKey.ContainsKey(i))
            {
                if (Managers.Midi.noteSetByKey[i].Count > 0 && Managers.Midi.noteSetByKey[i].Count > Managers.Midi.nextKeyIndex[i])
                {
                    CheckNotesStatus(i);
                }
            }
        }
    }

    void CheckNotesStatus(int keyNum)
    {
        noteCheckCoroutineCnt += 1;
        if (_lastInputTiming[keyNum] == 0 && Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key != 0)
            _lastInputTiming[keyNum] = Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key;

        if (Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value - currentDeltaTime < 0)
        {
            if (_lastInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value)
            {
                if (!Managers.Input.keyChecks[keyNum])
                {
                    currentFail += Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value - _lastInputTiming[keyNum];
                    currentAcc = (totalAcc - currentFail) / (float)totalAcc;
                }
                else
                {

                }
            }
            Managers.Midi.nextKeyIndex[keyNum]++;
        }

        // 일반적인 노트 정확도 검사 부분, 인덱스 오류를 막기위한 조건문
        if (Managers.Midi.noteSetByKey[keyNum].Count > Managers.Midi.nextKeyIndex[keyNum])
        {
            if (Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key - currentDeltaTime < 0)
            {
                if (_lastInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key)
                    _lastInputTiming[keyNum] = Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key;

                // 피아노 입력중인지, 지나친 선입력은 아닌지 체크
                if ((!Managers.Input.keyChecks[keyNum] && _lastInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value) ||
                    (!Managers.Input.keyChecks[keyNum] && _initInputTiming[keyNum] > Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key && _lastInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Value - Managers.Midi.song.division / 10f) ||
                    _initInputTiming[keyNum] < Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key - Managers.Midi.song.division / 10f)
                {
                    currentFail += currentDeltaTime - _lastInputTiming[keyNum];
                }
                else if (Managers.Input.keyChecks[keyNum] && _lastInputTiming[keyNum] >= Managers.Midi.noteSetByKey[keyNum][Managers.Midi.nextKeyIndex[keyNum]].Key)
                {
                    currentCorrect += currentDeltaTime - _lastInputTiming[keyNum];
                }

                _lastInputTiming[keyNum] = currentDeltaTime;
                currentAcc = (totalAcc - currentFail) / (float)totalAcc;
            }
        }

        (_uiController as ActualModUIController).UpdateAccuracy();

        noteCheckCoroutineCnt -= 1;
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
                if (!_noteRecords.ContainsKey(noteEvent.NoteNumber - DEFAULT_KEY_NUM_OFFSET))
                    _noteRecords.Add(noteEvent.NoteNumber - DEFAULT_KEY_NUM_OFFSET, new List<KeyValuePair<int, int>>());
                Debug.Log(noteEvent);
            }
            // 노트 입력 종료
            else if (noteEvent.Velocity == 0)
            {
                _noteRecords[noteEvent.NoteNumber - DEFAULT_KEY_NUM_OFFSET].Add(new KeyValuePair<int, int>(_initInputTiming[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET], currentDeltaTime));
                _initInputTiming[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET] = -1;
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1 - DEFAULT_KEY_NUM_OFFSET] = false;
            }
        }
    }
}
