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
    public TextMeshProUGUI tempoControllTMP;
    public Slider songTimeSlider;
    public Button tempoPlusBtn;
    public Button tempoMinusBtn;
    public GameObject songTimeSliderHandle;
    public GameObject pausePanelObj;
    public GameObject optionPanelObj;
    #endregion

    #region Protected Members
    protected Button _resumeBtn;
    protected Button _optionBtn;
    protected Button _exitBtn;
    protected Button _pauseBtn;
    #endregion

    protected IngameController _controller;

    protected void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/TimeSlider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        songTempoTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
        tempoControllTMP = GameObject.Find("MainCanvas/Informations/TempoSpeed/Value").GetComponent<TextMeshProUGUI>();
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");

        tempoPlusBtn = GameObject.Find("MainCanvas/Informations/TempoSpeed/PlusBtn").GetComponent<Button>();
        tempoMinusBtn = GameObject.Find("MainCanvas/Informations/TempoSpeed/MinusBtn").GetComponent<Button>();
        tempoPlusBtn.onClick.AddListener(IncreaseTempoSpeed);
        tempoMinusBtn.onClick.AddListener(DecreaseTempoSpeed);

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        optionPanelObj = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_Option");
        _resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        _optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        _exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        _pauseBtn = GameObject.Find("MainCanvas/Informations/PauseButton").GetComponent<Button>();
        pausePanelObj.SetActive(false);
        _optionBtn.onClick.AddListener(OnOptionButtonClick);
        _pauseBtn.onClick.AddListener(TogglePausePanel);

        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void IncreaseTempoSpeed()
    {
        if (PlayerPrefs.GetInt("user_TempoSpeed") >= 15)
        {
            return;
        }
        PlayerPrefs.SetInt("user_TempoSpeed", PlayerPrefs.GetInt("user_TempoSpeed") + 1);
        _controller.tempoSpeed = PlayerPrefs.GetInt("user_TempoSpeed") / 10.0f;
        tempoControllTMP.text = $"x{_controller.tempoSpeed}";
    }

    public void DecreaseTempoSpeed()
    {
        if (PlayerPrefs.GetInt("user_TempoSpeed") <= 5)
        {
            return;
        }
        PlayerPrefs.SetInt("user_TempoSpeed", PlayerPrefs.GetInt("user_TempoSpeed") - 1);
        _controller.tempoSpeed = PlayerPrefs.GetInt("user_TempoSpeed") / 10.0f;
        tempoControllTMP.text = $"x{_controller.tempoSpeed}";
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

    // ��Ʈ�ѷ����� ���ư����ϴ� ���� �ٸ��� ������ �������̵�� �������ؼ� ����� ��
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
