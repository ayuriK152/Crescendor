using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActualModUIController : MonoBehaviour
{
    public TextMeshProUGUI songTitleTMP;
    public TextMeshProUGUI songNoteMountTMP;
    public TextMeshProUGUI songBpmTMP;
    public TextMeshProUGUI songBeatTMP;
    public TextMeshProUGUI accuracyTMP;
    public TextMeshProUGUI introCountTMP;
    public Slider songTimeSlider;
    public GameObject songTimeSliderHandle;
    public GameObject pausePanelObj;

    Button _disconnectBtn;
    Button _resumeBtn;
    Button _optionBtn;
    Button _exitBtn;

    ActualModController _controller;

    public void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/TimeSlider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        songBpmTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
        accuracyTMP = GameObject.Find("MainCanvas/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        introCountTMP = GameObject.Find("MainCanvas/IntroTimeCount").GetComponent<TextMeshProUGUI>();
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        _resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        _optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        _exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        _disconnectBtn = GameObject.Find("MainCanvas/Buttons/DisconnectBtn").GetComponent<Button>();

        _controller = Managers.Ingame.currentController as ActualModController;

        _disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{_controller.passedNote}/{_controller.totalNote}";
    }

    public void UpdateAccuracy()
    {
        accuracyTMP.text = $"{Convert.ToInt32(_controller.currentAcc * 10000.0f) / 100.0f}%";
    }

    public void DisconnectPianoBtn()
    {
        _controller.DisconnectPiano();
    }

    void TogglePausePanel()
    {
        pausePanelObj.SetActive(!pausePanelObj.activeSelf);

        if (pausePanelObj.activeSelf)
        {
            _controller.enabled = false;
        }
        else
        {
            _controller.enabled = true;
        }
    }

    void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.ResultScene);
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }

    void InputKeyEvent(KeyCode keyCode, Define.InputType inputType)
    {
        switch (inputType)
        {
            case Define.InputType.OnKeyDown:
                switch (keyCode)
                {
                    case KeyCode.Escape:
                        TogglePausePanel();
                        break;
                }
                break;
            case Define.InputType.OnKeyUp:
                break;
        }
    }
}
