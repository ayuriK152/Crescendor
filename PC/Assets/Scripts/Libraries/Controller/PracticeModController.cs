using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Define;
using static Datas;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

public class PracticeModController : MonoBehaviour
{
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public int tempo = 120;
    public float scrollSpeed = 1.0f;
    public float notePosOffset = -2.625f;
    public float noteScale = 3.0f;
    public float widthValue = 1.5f;
    public string songTitle;

    public int passedNote;
    public int totalNote;
    public int currentNoteIndex;

    public bool isLoop;
    public int loopStartDeltaTime;
    public int loopEndDeltaTime;
    public int loopStartNoteIndex;
    public int loopStartPassedNote;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    bool _isInputTiming = false;
    bool _isWaitInput = true;

    PracticeModUIController _uiController;

    public void Init()
    {
        songTitle = PlayerPrefs.GetString("trans_SongTitle");

        passedNote = 0;
        totalNote = 0;
        currentNoteIndex = 0;

        isLoop = false;
        loopStartDeltaTime = -1;
        loopEndDeltaTime = -1;
        loopStartNoteIndex = 0;
        loopStartPassedNote = 0;

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi(songTitle, gameObject);
        totalNote = Managers.Midi.notes.Count;

        _uiController = Managers.UI.currentUIController as PracticeModUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = songTitle;
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songBpmTMP.text = $"{Managers.Midi.tempo}";
        _uiController.songBeatTMP.text = $"4/4";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLength;

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;

        if (Managers.Input.inputDevice != null)
        {
            Managers.Input.inputDevice.EventReceived -= OnEventReceived;
            Managers.Input.inputDevice.EventReceived += OnEventReceived;
        }

        Managers.InitManagerPosition();
    }

    void Update()
    {
        WaitMidiInput();
        Scroll();
    }
    void Scroll()
    {
        if (isLoop)
        {
            if (currentDeltaTimeF >= loopEndDeltaTime)
            {
                currentNoteIndex = loopStartNoteIndex;
                passedNote = loopStartPassedNote;
                _uiController.UpdatePassedNote();
                currentDeltaTimeF = loopStartDeltaTime;
                SyncDeltaTime(false);
                transform.position = new Vector3(-1, 0, -currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScale + notePosOffset);
            }
        }
        if (Managers.Midi.noteTiming[currentNoteIndex] <= currentDeltaTimeF && _isWaitInput)
        {
            currentDeltaTime = Managers.Midi.noteTiming[currentNoteIndex];
            SyncDeltaTime(true);
            transform.position = new Vector3(-1, 0, -currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScale + notePosOffset);
            _isInputTiming = true;
            return;
        }
        currentDeltaTimeF += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.song.division * Time.deltaTime;
        SyncDeltaTime(false);
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.noteScale * Time.deltaTime));
    }

    void WaitMidiInput()
    {
        if (!_isInputTiming)
            return;
        for (int i = 0; i < Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count; i++)
        {
            Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i] = new KeyValuePair<int, bool>(Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key, Managers.Input.keyChecks[Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Key]);
            if (!Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]][i].Value)
                return;
        }
        IncreaseCurrentNoteIndex();
    }

    public void IncreaseCurrentNoteIndex()
    {
        passedNote += Managers.Midi.noteSetBySameTime[Managers.Midi.noteTiming[currentNoteIndex]].Count;
        _uiController.UpdatePassedNote();
        currentNoteIndex += 1;
    }

    public void DisconnectPiano()
    {
        Managers.Input.inputDevice.StopEventsListening();
    }

    public void AutoScroll()
    {
        _isWaitInput = !_isWaitInput;
    }

    public void TurnOffLoop()
    {
        _uiController.TurnOffLoop();
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

    void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if (e.Event.EventType != MidiEventType.ActiveSensing)
        {
            NoteEvent noteEvent = e.Event as NoteEvent;

            // ��Ʈ �Է� ����
            if (noteEvent.Velocity != 0)
            {
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1] = true;
                Debug.Log(noteEvent);
            }
            // ��Ʈ �Է� ����
            else if (noteEvent.Velocity == 0)
            {
                Managers.Input.keyChecks[noteEvent.NoteNumber - 1] = false;
            }
        }
    }

    void InputKeyEvent(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.LeftBracket:
                SetStartDeltaTime();
                break;
            case KeyCode.RightBracket:
                SetEndDeltaTime();
                break;
        }
    }

    void SetStartDeltaTime()
    {
        loopStartDeltaTime = currentDeltaTime;
        loopStartNoteIndex = currentNoteIndex;
        loopStartPassedNote = passedNote;
        _uiController.SetLoopStartMarker();
        if (loopEndDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            _uiController.SwapStartEndMarker();
        }
        Debug.Log($"Loop Start Delta Time Set to {loopStartDeltaTime}");
        if (loopEndDeltaTime >= 0)
            isLoop = true;
    }

    void SetEndDeltaTime()
    {
        if (loopStartDeltaTime < 0)
            return;
        loopEndDeltaTime = currentDeltaTime;
        _uiController.SetLoopEndMarker();
        if (loopStartDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            _uiController.SwapStartEndMarker();
        }
        Debug.Log($"Loop End Delta Time Set to {loopEndDeltaTime}");
        if (loopStartDeltaTime >= 0)
            isLoop = true;
    }
}
