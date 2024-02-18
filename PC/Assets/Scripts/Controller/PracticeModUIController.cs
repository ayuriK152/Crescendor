using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PracticeModUIController : MonoBehaviour
{
    public TextMeshProUGUI songTitleTMP;
    public TextMeshProUGUI songNoteMountTMP;
    public TextMeshProUGUI songBpmTMP;
    public TextMeshProUGUI songBeatTMP;
    public Slider songTimeSlider;
    public GameObject songTimeSliderHandle;
    public GameObject loopStartMarker;
    public Image loopStartMarkerSprite;
    public GameObject loopEndMarker;
    public Image loopEndMarkerSprite;
    public GameObject pausePanelObj;

    Button forceScrollBtn;
    Button disconnectBtn;
    Button autoScrollBtn;
    Button turnOffLoopBtn;
    Button resumeBtn;
    Button optionBtn;
    Button exitBtn;

    PracticeModController practiceModController;

    public void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/TimeSlider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        songBpmTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");
        loopStartMarker = GameObject.Find("MainCanvas/TimeSlider/Slider/LoopStartMarker");
        loopEndMarker = GameObject.Find("MainCanvas/TimeSlider/Slider/LoopEndMarker");
        loopStartMarkerSprite = loopStartMarker.GetComponent<Image>();
        loopEndMarkerSprite = loopEndMarker.GetComponent<Image>();

        forceScrollBtn = GameObject.Find("MainCanvas/Buttons/ForceScrollBtn").GetComponent<Button>();
        disconnectBtn = GameObject.Find("MainCanvas/Buttons/DisconnectBtn").GetComponent<Button>();
        autoScrollBtn = GameObject.Find("MainCanvas/Buttons/AutoScroll").GetComponent<Button>();
        turnOffLoopBtn = GameObject.Find("MainCanvas/Buttons/TurnOffLoop").GetComponent<Button>();

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        practiceModController = Managers.Ingame.controller as PracticeModController;

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        forceScrollBtn.onClick.AddListener(ForceScrollBtn);
        disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        autoScrollBtn.onClick.AddListener(AutoScrollBtn);
        turnOffLoopBtn.onClick.AddListener(TurnOffLoop);
        resumeBtn.onClick.AddListener(TogglePausePanel);
        exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void SetLoopStartMarker()
    {
        loopStartMarker.transform.position = songTimeSliderHandle.transform.position;
        loopStartMarkerSprite.enabled = true;
    }

    public void SetLoopEndMarker()
    {
        loopEndMarker.transform.position = songTimeSliderHandle.transform.position;
        loopEndMarkerSprite.enabled = true;
    }

    public void SwapStartEndMarker()
    {
        Vector3 temp = loopStartMarker.transform.position;
        loopStartMarker.transform.position = loopEndMarker.transform.position;
        loopEndMarker.transform.position = loopStartMarker.transform.position;
    }

    public void TurnOffLoop()
    {
        practiceModController.TurnOffLoop();
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{practiceModController.passedNote}/{practiceModController.totalNote}";
    }

    public void UpdateDeltaTimeBySlider(float sliderValue)
    {
        if (practiceModController.isPlaying)
            practiceModController.isPlaying = false;
        practiceModController.currentDeltaTime = (int)sliderValue;
        practiceModController.SyncDeltaTime(true);
        StartCoroutine(practiceModController.UpdateNotePosByTime());
    }

    public void ForceScrollBtn()
    {
        practiceModController.isPlaying = true;
        practiceModController.IncreaseCurrentNoteIndex();
    }

    public void DisconnectPianoBtn()
    {
        practiceModController.DisconnectPiano();
    }

    public void AutoScrollBtn()
    {
        practiceModController.AutoScroll();
    }

    void OnClickExitBtn()
    {
        Managers.Input.keyAction -= InputKeyEvent;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.ResultScene);
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }

    void TogglePausePanel()
    {
        pausePanelObj.SetActive(!pausePanelObj.activeSelf);

        if (pausePanelObj.activeSelf)
        {
            practiceModController.enabled = false;
        }
        else
        {
            practiceModController.enabled = true;
        }
    }

    void InputKeyEvent(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Escape:
                TogglePausePanel();
                break;
        }
    }
}
