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
    public GameObject handmenuPanelObj;
    public GameObject pausedUI;
    
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
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        optionPanelObj = Resources.Load<GameObject>("Prefabs/UI/XR_Popup/UI_Option");
        handmenuPanelObj = GameObject.Find("UI/Canvas");

        _resumeBtn = handmenuPanelObj.transform.Find("ButtonPanel/Buttons/PauseBtn").GetComponent<Button>();
        _optionBtn = handmenuPanelObj.transform.Find("ButtonPanel/Buttons/PauseBtn").GetComponent<Button>();
        _exitBtn = handmenuPanelObj.transform.Find("ButtonPanel/Buttons/PauseBtn").GetComponent<Button>();
        _pauseBtn = handmenuPanelObj.transform.Find("ButtonPanel/Buttons/PauseBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        _optionBtn.onClick.AddListener(OnOptionButtonClick);
        _pauseBtn.onClick.AddListener(TogglePausePanel);
        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

        // Managers.Input.keyAction -= InputKeyEvent;
        // Managers.Input.keyAction += InputKeyEvent;
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

    /*
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
    */
}
