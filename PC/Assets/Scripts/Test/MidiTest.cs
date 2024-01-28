using SmfLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;
using static Datas;

public class MidiTest : MonoBehaviour
{
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public int tempo = 120;
    public float scrollSpeed = 1.0f;
    public float notePosOffset = 0.0f;
    public float noteScale = 1.0f;
    public float widthValue = 1.0f;
    public string songTitle;

    bool _isInputTiming = false;
    bool _isWaitInput = true;

    int currentNoteIndex = 0;

    void Start()
    {
        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi(songTitle, gameObject);
        Managers.UI.BindIngameUI();
        Managers.UI.songTitleTMP.text = songTitle;
        Managers.UI.songNoteMountTMP.text = $"0/{Managers.Midi.notes.Count}";
        Managers.UI.songBpmTMP.text = $"{Managers.Midi.tempo}";
        Managers.UI.songBeatTMP.text = $"4/4";
        Managers.UI.songTimeSlider.maxValue = Managers.Midi.songLength;
    }

    void Update()
    {
        WaitMidiInput();
        Scroll();
    }

    void Scroll()
    {
        if (Managers.Ingame.isLoop)
        {
            if (Managers.Ingame.currentDeltaTimeF >= Managers.Ingame.loopEndDeltaTime)
            {
                Managers.Ingame.currentDeltaTimeF = Managers.Ingame.loopStartDeltaTime;
                Managers.Ingame.SyncDeltaTime(false);
                transform.position = new Vector3(-1, 0, -Managers.Ingame.currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScale + notePosOffset);
            }
        }
        if (Managers.Midi.noteTiming[currentNoteIndex] <= Managers.Ingame.currentDeltaTimeF && _isWaitInput)
        {
            Managers.Ingame.currentDeltaTime = Managers.Midi.noteTiming[currentNoteIndex];
            Managers.Ingame.SyncDeltaTime(true);
            transform.position = new Vector3(-1, 0, -Managers.Ingame.currentDeltaTimeF / Managers.Midi.song.division * Managers.Midi.noteScale + notePosOffset);
            _isInputTiming = true;
            return;
        }
        Managers.Ingame.currentDeltaTimeF += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.song.division * Time.deltaTime;
        Managers.Ingame.SyncDeltaTime(false);
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
        currentNoteIndex += 1;
    }

    public void DisconnectPiano()
    {
        Managers.Input.inputDevice.StopEventsListening();
    }

    public void AutoScroll()
    {
        _isWaitInput = false;
    }
}
