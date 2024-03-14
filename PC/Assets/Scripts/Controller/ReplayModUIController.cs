using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayModUIController : MonoBehaviour
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

    Button _forceScrollBtn;
    Button _autoScrollBtn;
    Button _turnOffLoopBtn;
    Button _resumeBtn;
    Button _optionBtn;
    Button _exitBtn;

    ReplayModController _controller;

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

        _forceScrollBtn = GameObject.Find("MainCanvas/Buttons/ForceScrollBtn").GetComponent<Button>();
        _autoScrollBtn = GameObject.Find("MainCanvas/Buttons/AutoScroll").GetComponent<Button>();
        _turnOffLoopBtn = GameObject.Find("MainCanvas/Buttons/TurnOffLoop").GetComponent<Button>();

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        _resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        _optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        _exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        _controller = Managers.Ingame.currentController as ReplayModController;

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        _forceScrollBtn.onClick.AddListener(ForceScrollBtn);
        _autoScrollBtn.onClick.AddListener(AutoScrollBtn);
        _turnOffLoopBtn.onClick.AddListener(TurnOffLoop);
        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

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

    void TurnOffLoop()
    {
        _controller.TurnOffLoop();
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{_controller.passedNote}/{_controller.totalNote}";
    }

    void UpdateDeltaTimeBySlider(float sliderValue)
    {
        if (_controller.isPlaying)
            _controller.isPlaying = false;
        _controller.currentDeltaTime = (int)sliderValue;
        _controller.SyncDeltaTime(true);
        StartCoroutine(_controller.UpdateNotePosByTime());
    }

    void ForceScrollBtn()
    {
        _controller.isPlaying = true;
        _controller.IncreaseCurrentNoteIndex();
    }

    void AutoScrollBtn()
    {
        _controller.AutoScroll();
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
            _controller.enabled = false;
        }
        else
        {
            _controller.enabled = true;
        }
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
