using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIController : BaseUIController
{
    #region Public Members
    public TextMeshProUGUI songTitleTMP;
    public TextMeshProUGUI songNoteMountTMP;
    public TextMeshProUGUI songTempoTMP;
    public TextMeshProUGUI songBeatTMP;
    public Slider songTimeSlider;
    public GameObject songTimeSliderHandle;
    public GameObject pausePanelObj;
    public GameObject optionPanelObj;
    #endregion

    #region Protected Members
    protected Button _disconnectBtn;
    protected Button _resumeBtn;
    protected Button _optionBtn;
    protected Button _exitBtn;
    #endregion

    protected IngameController _controller;

    protected void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/TimeSlider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        songTempoTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        optionPanelObj = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_Option");
        _resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        _optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        _exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);
        _optionBtn.onClick.AddListener(OnOptionButtonClick);

        try
        {
            _disconnectBtn = GameObject.Find("MainCanvas/Buttons/DisconnectBtn").GetComponent<Button>();
            _disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        }
        catch
        {
            Debug.Log("No Object");
        }

        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void UpdatePassedNoteText()
    {
        songNoteMountTMP.text = $"{_controller.passedNote}/{_controller.totalNote}";
    }

    public void UpdateTempoText()
    {
        songTempoTMP.text = $"{Managers.Midi.tempo}";
    }

    public void UpdateBeatText()
    {
        songBeatTMP.text = $"{Managers.Midi.beat.Key}/{Managers.Midi.beat.Value}";
    }

    public void OnOptionButtonClick()
    {
        ShowPopupUI<UI_Option>();
    }

    public void DisconnectPianoBtn()
    {
        _controller.DisconnectPiano();
    }

    public void TogglePausePanel()
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

    // 컨트롤러마다 돌아가야하는 씬이 다르기 때문에 오버라이드로 재정의해서 사용할 것
    protected virtual void OnClickExitBtn() { }

    protected virtual void InputKeyEvent(KeyCode keyCode, Define.InputType inputType)
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
