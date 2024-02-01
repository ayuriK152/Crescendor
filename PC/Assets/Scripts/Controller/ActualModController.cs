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
    public float notePosOffset = 0.0f;
    public float noteScale = 1.0f;
    public float widthValue = 1.0f;
    public string songTitle;

    public int passedNote;
    public int totalNote;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    ActualModUIController _uiController;

    public void Init()
    {
        passedNote = 0;
        totalNote = 0;

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi(songTitle, gameObject);
        totalNote = Managers.Midi.notes.Count;

        _uiController = Managers.UI.currentUIController as ActualModUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = songTitle;
        _uiController.songNoteMountTMP.text = $"0/{totalNote}";
        _uiController.songBpmTMP.text = $"{Managers.Midi.tempo}";
        _uiController.songBeatTMP.text = $"4/4";
        _uiController.songTimeSlider.maxValue = Managers.Midi.songLength;

        // Managers.Input.keyAction -= InputKeyEvent;
        // Managers.Input.keyAction += InputKeyEvent;
    }

    void Update()
    {
        Scroll();
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
}
