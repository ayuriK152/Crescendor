using SmfLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;
using static Datas;

public class ActualModController : MonoBehaviour
{
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public int tempo = 120;
    public float scrollSpeed = 1.0f;
    public float currentDeltaTime = 0.0f;
    public float notePosOffset = 0.0f;
    public float noteScale = 1.0f;
    public float widthValue = 1.0f;

    bool _isInputTiming = false;

    int currentNoteIndex = 0;

    void Start()
    {
        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi("for_elise", gameObject);
    }

    void Update()
    {
        Scroll();
    }

    void Scroll()
    {
        if (Managers.Midi.noteTiming[currentNoteIndex] <= currentDeltaTime)
        {
            currentDeltaTime = Managers.Midi.noteTiming[currentNoteIndex];
            transform.position = new Vector3(-1, 0, -currentDeltaTime / Managers.Midi.song.division * Managers.Midi.noteScale + notePosOffset);
            _isInputTiming = true;
            return;
        }
        currentDeltaTime += 2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.song.division * Time.deltaTime;
        transform.Translate(new Vector3(0, 0, -2 * Datas.DEFAULT_QUARTER_NOTE_MILLISEC / Managers.Midi.song.tempoMap[0].milliSecond * Managers.Midi.noteScale * Time.deltaTime));
    }

    public void IncreaseCurrentNoteIndex()
    {
        currentNoteIndex += 1;
    }

    public void DisconnectPiano()
    {
        Managers.Input.inputDevice.StopEventsListening();
    }
}
