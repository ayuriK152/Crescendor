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
    public Slider songTimeSlider;
    public GameObject songTimeSliderHandle;
    public GameObject pausePanelObj;

    Button disconnectBtn;
    Button resumeBtn;
    Button optionBtn;
    Button exitBtn;

    ActualModController actualModController;

    public void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/TimeSlider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        songBpmTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
        accuracyTMP = GameObject.Find("MainCanvas/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        disconnectBtn = GameObject.Find("MainCanvas/Buttons/DisconnectBtn").GetComponent<Button>();

        actualModController = Managers.Ingame.controller as ActualModController;

        disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        resumeBtn.onClick.AddListener(TogglePausePanel);
        exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{actualModController.passedNote}/{actualModController.totalNote}";
    }

    public void UpdateAccuracy()
    {
        accuracyTMP.text = $"{Convert.ToInt32(actualModController.currentAcc * 10000.0f) / 100.0f}%";
    }

    public void DisconnectPianoBtn()
    {
        actualModController.DisconnectPiano();
    }

    void TogglePausePanel()
    {
        pausePanelObj.SetActive(!pausePanelObj.activeSelf);

        if (pausePanelObj.activeSelf)
        {
            actualModController.enabled = false;
        }
        else
        {
            actualModController.enabled = true;
        }
    }

    void OnClickExitBtn()
    {
        Managers.Input.keyAction -= InputKeyEvent;
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
