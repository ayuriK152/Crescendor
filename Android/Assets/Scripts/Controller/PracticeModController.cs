using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Datas;

public class PracticeModController : IngameController
{
    #region Public Members
    public int currentNoteIndex;
    public int loopStartDeltaTime;
    public int loopEndDeltaTime;
    public int loopStartNoteIndex;
    public int loopStartPassedNote;

    public bool isLoop;
    public bool isPlaying;
    public bool isSongEnd = false;
    #endregion

    #region Private Members
    private bool _isInputTiming = false;
    private bool _isWaitInput = true;
    #endregion

    public void Init()
    {
        base.Init();

        currentNoteIndex = 0;

        isLoop = false;
        isPlaying = true;
        loopStartDeltaTime = -1;
        loopEndDeltaTime = -1;
        loopStartNoteIndex = 0;
        loopStartPassedNote = 0;

        _uiController = Managers.UI.currentUIController as PracticeModUIController;
        (_uiController as PracticeModUIController).BindIngameUI();
        _uiController.songTitleTMP.text = songTitle.Replace("_", " ");
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songTempoTMP.text = $"{Managers.Midi.tempo}";
        _uiController.songBeatTMP.text = $"{Managers.Midi.beat.Key}/{Managers.Midi.beat.Value}";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLengthDelta;

        for (int i = 0; i < Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count; i++)
        {
            _correctNoteKeys.Add(Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key);
        }

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;

        Managers.Input.noteAction -= OnEventReceived;
        Managers.Input.noteAction += OnEventReceived;

        Managers.InitManagerPosition();

        try
        {
            StartCoroutine(sheetController.ShowSheetAtIndex($"SheetDatas/{songTitle}", 0));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void Update()
    {
        StartCoroutine(ToggleKeyHighlight());
        WaitMidiInput();
        Scroll();
    }

    void Scroll()
    {
        if (!isPlaying || isSongEnd)
            return;
        if (isLoop)
        {
            if (currentDeltaTimeF >= loopEndDeltaTime)
            {
                currentNoteIndex = loopStartNoteIndex;
                passedNote = loopStartPassedNote;
                _uiController.UpdatePassedNoteText();
                currentDeltaTimeF = loopStartDeltaTime;
                SyncDeltaTime(false);
                transform.position = new Vector3(0, 0, -currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScaleZ + notePosOffset);
            }
        }
        
        currentDeltaTimeF += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * tempoSpeed * Managers.Midi.song.division * Time.deltaTime;
        
        if (currentNoteIndex < Managers.Midi.noteTiming.Count)
        {
            if (Managers.Midi.noteTiming[currentNoteIndex] <= currentDeltaTimeF && _isWaitInput)
            {
                currentDeltaTime = Managers.Midi.noteTiming[currentNoteIndex];
                if (currentDeltaTime != currentDeltaTimeF)
                    SyncDeltaTime(true);
                transform.position = new Vector3(0, 0, -currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScaleZ + notePosOffset);
                _isInputTiming = true;
                return;
            }
        }

        SyncDeltaTime(false);
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * tempoSpeed * Managers.Midi.noteScaleZ * Time.deltaTime));
    }

    void WaitMidiInput()
    {
        UpdateBar();
        if (currentNoteIndex >= Managers.Midi.noteTiming.Count)
        {
            if (currentDeltaTime >= Managers.Midi.songLengthDelta)
            {
                if (!isSongEnd)
                {
                    (_uiController as PracticeModUIController).ToggleSongEndPanel();
                }
                isSongEnd = true;
            }
        }
        if (!_isInputTiming || isSongEnd)
            return;
        if (currentNoteIndex < Managers.Midi.noteTiming.Count)
        {
            for (int i = 0; i < Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count; i++)
            {
                Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i] = new KeyValuePair<int, bool>(Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key, Managers.Input.keyChecks[Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key]);
                if (!Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Value)
                {
                    if (Managers.Input.isPianoConnected && _initInputTiming[Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key] < currentDeltaTime)
                        return;
                    _isInputTiming = false;
                    return;
                }
            }
            UpdatePassedNote();
            UpdateTempo();
            UpdateBeat();
            UpdateBar();
        }
    }

    public void UpdatePassedNote()
    {
        if (currentNoteIndex == Managers.Midi.noteTiming.Count)
            return;
        if (Managers.Midi.noteTiming[currentNoteIndex] - currentDeltaTime > 0)
            return;
        _correctNoteKeys.Clear();
        passedNote += Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
        _uiController.UpdatePassedNoteText();
        currentNoteIndex += 1;

        if (currentNoteIndex == Managers.Midi.noteTiming.Count)
            return;

        for (int i = 0; i < Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count; i++)
        {
            _correctNoteKeys.Add(Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key);
        }
    }

    public void AutoScroll()
    {
        _isWaitInput = !_isWaitInput;
    }

    public void TurnOffLoop()
    {
        isLoop = false;
        loopStartDeltaTime = -1;
        loopEndDeltaTime = -1;
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

    // 타임 슬라이더 조작 또는 데이터 임의 조작으로 인한 deltaTime 변화로 현재 진행중인 곡의 정보가 바뀐 경우
    public IEnumerator ForceUpdateNote()
    {
        transform.position = new Vector3(0, 0, -currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScaleZ + notePosOffset);
        while (true)
        {
            // 노트 개수, 딕셔너리 데이터 관리
            bool isNoteIdxChanged = false;
            if (currentNoteIndex > 0)
            {
                if (Managers.Midi.noteTiming[currentNoteIndex - 1] >= currentDeltaTime)
                {
                    currentNoteIndex--;
                    passedNote -= Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
                    _uiController.UpdatePassedNoteText();
                    isNoteIdxChanged = true;
                }
            }
            if (currentNoteIndex < Managers.Midi.noteTiming.Count - 1 && !isNoteIdxChanged)
            {
                if (Managers.Midi.noteTiming[currentNoteIndex] < currentDeltaTime)
                {
                    passedNote += Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
                    currentNoteIndex++;
                    _uiController.UpdatePassedNoteText();
                    isNoteIdxChanged = true;
                }
            }

            // 현재 템포 인덱스 관리
            bool isTempoMapIdxChanged = false;
            if (_tempoMapIdx > 0)
            {
                if (Managers.Midi.song.tempoMap[_tempoMapIdx - 1].deltaTime >= currentDeltaTime)
                {
                    _tempoMapIdx--;
                    tempo = Managers.Midi.song.tempoMap[_tempoMapIdx].tempo;
                    _uiController.UpdateTempoText();
                    isTempoMapIdxChanged = true;
                }
            }
            if (_tempoMapIdx < Managers.Midi.song.tempoMap.Count - 1 && !isTempoMapIdxChanged)
            {
                if (Managers.Midi.song.tempoMap[_tempoMapIdx].deltaTime < currentDeltaTime)
                {
                    _tempoMapIdx++;
                    tempo = Managers.Midi.song.tempoMap[_tempoMapIdx].tempo;
                    _uiController.UpdateTempoText();
                    isTempoMapIdxChanged = true;
                }
            }

            // 현재 박자 인덱스 관리
            bool isBeatMapIdxChanged = false;
            if (_beatMapIdx > 0)
            {
                if (Managers.Midi.song.beatMap[_beatMapIdx - 1].deltaTime >= currentDeltaTime)
                {
                    _beatMapIdx--;
                    Managers.Midi.beat = new KeyValuePair<int, int>(Managers.Midi.song.beatMap[_beatMapIdx].numerator, Managers.Midi.song.beatMap[_beatMapIdx].denominator);
                    _uiController.UpdateBeatText();
                    isBeatMapIdxChanged = true;
                }
            }
            if (_beatMapIdx < Managers.Midi.song.beatMap.Count - 1 && !isBeatMapIdxChanged)
            {
                if (Managers.Midi.song.beatMap[_beatMapIdx].deltaTime < currentDeltaTime)
                {
                    _beatMapIdx++;
                    Managers.Midi.beat = new KeyValuePair<int, int>(Managers.Midi.song.beatMap[_beatMapIdx].numerator, Managers.Midi.song.beatMap[_beatMapIdx].denominator);
                    _uiController.UpdateBeatText();
                    isBeatMapIdxChanged = true;
                }
            }

            bool isCurrentBarIdxChanged = false;
            if (_currentBarIdx > 0)
            {
                if (Managers.Midi.barTiming[_currentBarIdx - 1] >= currentDeltaTime)
                {
                    _currentBarIdx--;
                    StartCoroutine(sheetController.ShowSheetAtIndex($"SheetDatas/{songTitle}", _currentBarIdx / 4));
                    isCurrentBarIdxChanged = true;
                }
            }
            if (_currentBarIdx < Managers.Midi.barTiming.Count - 1 && !isCurrentBarIdxChanged)
            {
                if (Managers.Midi.barTiming[_currentBarIdx] < currentDeltaTime)
                {
                    _currentBarIdx++;
                    StartCoroutine(sheetController.ShowSheetAtIndex($"SheetDatas/{songTitle}", _currentBarIdx / 4));
                    isCurrentBarIdxChanged = true;
                }
            }

            if (isNoteIdxChanged || isTempoMapIdxChanged || isBeatMapIdxChanged || isCurrentBarIdxChanged)
                continue;
            break;
        }
        yield return null;
    }

    void OnEventReceived(int noteNum, int velocity)
    {
        if (isSongEnd && velocity != 0)
        {
            isSongEnd = false;
            currentDeltaTime = 0;
            SyncDeltaTime(true);
            StartCoroutine(ForceUpdateNote());
            (_uiController as PracticeModUIController).ToggleSongEndPanel();
        }
        // 노트 입력 시작
        if (velocity != 0)
        {
            _initInputTiming[noteNum - 1 - DEFAULT_KEY_NUM_OFFSET] = currentDeltaTime;
            Managers.Input.keyChecks[noteNum - 1 - DEFAULT_KEY_NUM_OFFSET] = true;
            Debug.Log(noteNum);
        }
        // 노트 입력 종료
        else if (velocity == 0)
        {
            _isPlayingEffect[noteNum - 1 - DEFAULT_KEY_NUM_OFFSET] = false;
            _initInputTiming[noteNum - 1 - DEFAULT_KEY_NUM_OFFSET] = -1;
            Managers.Input.keyChecks[noteNum - 1 - DEFAULT_KEY_NUM_OFFSET] = false;
            Debug.Log(noteNum);
        }
    }

    void InputKeyEvent(KeyCode keyCode, Define.InputType inputType)
    {
        if (isSongEnd && keyCode != KeyCode.Escape)
        {
            isSongEnd = false;
            currentDeltaTime = 0;
            SyncDeltaTime(true);
            StartCoroutine(ForceUpdateNote());
            (_uiController as PracticeModUIController).ToggleSongEndPanel();
            return;
        }
        switch (inputType)
        {
            case Define.InputType.OnKeyDown:
                switch (keyCode)
                {
                    case KeyCode.Escape:
                        if (isSongEnd)
                        {
                            Managers.CleanManagerChilds();
                            Managers.Input.keyAction = null;
                            Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
                        }
                        break;

                    case KeyCode.LeftBracket:
                        SetStartDeltaTime();
                        break;

                    case KeyCode.RightBracket:
                        SetEndDeltaTime();
                        break;
                }
                break;
            case Define.InputType.OnKeyUp:
                break;
        }
    }

    void SetStartDeltaTime()
    {
        loopStartDeltaTime = currentDeltaTime;
        loopStartNoteIndex = currentNoteIndex;
        loopStartPassedNote = passedNote;
        (_uiController as PracticeModUIController).SetLoopStartMarker();
        if (loopEndDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            (_uiController as PracticeModUIController).SwapStartEndMarker();
        }
        Debug.Log($"Loop Start Delta Time Set to {loopStartDeltaTime}");
        if (loopEndDeltaTime >= 0)
        {
            isLoop = true;
            (_uiController as PracticeModUIController).ActiveLoopBtn();
        }
    }

    void SetEndDeltaTime()
    {
        if (loopStartDeltaTime < 0)
            return;
        loopEndDeltaTime = currentDeltaTime;
        (_uiController as PracticeModUIController).SetLoopEndMarker();
        if (loopStartDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            (_uiController as PracticeModUIController).SwapStartEndMarker();
        }
        Debug.Log($"Loop End Delta Time Set to {loopEndDeltaTime}");
        if (loopStartDeltaTime >= 0)
        {
            isLoop = true;
            (_uiController as PracticeModUIController).ActiveLoopBtn();
        }
    }
}
