using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Define;
using static Datas;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System.Collections;

public class ReplayModController : IngameController
{
    #region Public Members
    public int currentNoteIndex;

    public bool isLoop;
    public bool isPlaying;
    public int loopStartDeltaTime;
    public int loopEndDeltaTime;
    public int loopStartNoteIndex;
    public int loopStartPassedNote;
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

        _uiController = Managers.UI.currentUIController as ReplayModUIController;
        (_uiController as ReplayModUIController).BindIngameUI();
        _uiController.songTitleTMP.text = songTitle.Replace("_", " ");
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songTempoTMP.text = $"{Managers.Midi.tempo}";
        _uiController.songBeatTMP.text = $"{Managers.Midi.beat.Key}/{Managers.Midi.beat.Value}";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLengthDelta;

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;

        Managers.InitManagerPosition();

        Managers.Midi.LoadAndInstantiateReplay(Managers.Data.rankRecord.midi, false);
    }

    void Update()
    {
        WaitMidiInput();
        Scroll();
    }
    void Scroll()
    {
        if (!isPlaying)
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
        if (!_isInputTiming || currentNoteIndex >= Managers.Midi.noteTiming.Count)
            return;
        for (int i = 0; i < Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count; i++)
        {
            Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i] = new KeyValuePair<int, bool>(Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key, Managers.Input.keyChecks[Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key]);
            if (!Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Value)
            {
                if (Managers.Input.selectedInputDevice != null && _initInputTiming[Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key] < currentDeltaTime)
                    return;
                _isInputTiming = false;
                return;
            }
        }
        UpdatePassedNote();
    }

    public void UpdatePassedNote()
    {
        if (Managers.Midi.noteTiming[currentNoteIndex] - currentDeltaTime > 0)
            return;
        passedNote += Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
        _uiController.UpdatePassedNoteText();
        currentNoteIndex += 1;
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

    public IEnumerator UpdateNotePosByTime()
    {
        transform.position = new Vector3(0, 0, -currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScaleZ + notePosOffset);
        while (true)
        {
            if (currentNoteIndex > 0)
            {
                if (Managers.Midi.noteTiming[currentNoteIndex - 1] >= currentDeltaTime)
                {
                    currentNoteIndex--;
                    passedNote -= Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
                    _uiController.UpdatePassedNoteText();
                    continue;
                }
            }
            if (Managers.Midi.noteTiming[currentNoteIndex] < currentDeltaTime)
            {
                passedNote += Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
                currentNoteIndex++;
                _uiController.UpdatePassedNoteText();
                continue;
            }
            break;
        }
        yield return null;
    }

    void InputKeyEvent(KeyCode keyCode, Define.InputType inputType)
    {
        switch (inputType)
        {
            case Define.InputType.OnKeyDown:
                switch (keyCode)
                {
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

    void SetStartDeltaTime()
    {
        loopStartDeltaTime = currentDeltaTime;
        loopStartNoteIndex = currentNoteIndex;
        loopStartPassedNote = passedNote;
        (_uiController as ReplayModUIController).SetLoopStartMarker();
        if (loopEndDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            (_uiController as ReplayModUIController).SwapStartEndMarker();
        }
        Debug.Log($"Loop Start Delta Time Set to {loopStartDeltaTime}");
        if (loopEndDeltaTime >= 0)
        {
            isLoop = true;
            (_uiController as ReplayModUIController).ActiveLoopBtn();
        }
    }

    void SetEndDeltaTime()
    {
        if (loopStartDeltaTime < 0)
            return;
        loopEndDeltaTime = currentDeltaTime;
        (_uiController as ReplayModUIController).SetLoopEndMarker();
        if (loopStartDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            (_uiController as ReplayModUIController).SwapStartEndMarker();
        }
        Debug.Log($"Loop End Delta Time Set to {loopEndDeltaTime}");
        if (loopStartDeltaTime >= 0)
        {
            isLoop = true;
            (_uiController as ReplayModUIController).ActiveLoopBtn();
        }
    }
}
