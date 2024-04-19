using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayModUIController : IngameUIController
{
    public GameObject loopStartMarker;
    public Image loopStartMarkerSprite;
    public GameObject loopEndMarker;
    public Image loopEndMarkerSprite;

    Button _resumeBtn;
    Button _optionBtn;
    Button _exitBtn;
    Button _toBeginBtn;
    Button _playBtn;
    Button _forceProgressBtn;
    Button _toEndBtn;
    Button _loopBtn;
    Toggle _originalNotesToggle;
    GameObject _originalNotes;

    public void BindIngameUI()
    {
        loopStartMarker = GameObject.Find("MainCanvas/TimeSlider/Slider/LoopStartMarker");
        loopEndMarker = GameObject.Find("MainCanvas/TimeSlider/Slider/LoopEndMarker");
        loopStartMarkerSprite = loopStartMarker.GetComponent<Image>();
        loopEndMarkerSprite = loopEndMarker.GetComponent<Image>();

        _toBeginBtn = GameObject.Find("MainCanvas/TimeSlider/ToBeginBtn").GetComponent<Button>();
        _playBtn = GameObject.Find("MainCanvas/TimeSlider/PlayBtn").GetComponent<Button>();
        _forceProgressBtn = GameObject.Find("MainCanvas/TimeSlider/ForceProgressBtn").GetComponent<Button>();
        _toEndBtn = GameObject.Find("MainCanvas/TimeSlider/ToEndBtn").GetComponent<Button>();
        _loopBtn = GameObject.Find("MainCanvas/TimeSlider/LoopBtn").GetComponent<Button>();
        _loopBtn.interactable = false;
        _originalNotes = GameObject.Find("@Manager/Notes");

        _originalNotesToggle = GameObject.Find("MainCanvas/TimeSlider/OriginNoteToggle").GetComponent<Toggle>();

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);
        _toBeginBtn.onClick.AddListener(OnToBeginBtnClick);
        _playBtn.onClick.AddListener(OnPlayBtnClick);
        _forceProgressBtn.onClick.AddListener(OnForceProgressBtnClick);
        _toEndBtn.onClick.AddListener(OnToEndBtnClick);
        _loopBtn.onClick.AddListener(TurnOffLoop);
        _originalNotesToggle.onValueChanged.AddListener(ToggleOriginalNote);
        _originalNotesToggle.isOn = false;

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;

        _controller = Managers.Ingame.currentController as ReplayModController;
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
        loopStartMarker.transform.position = loopEndMarker.transform.position;
        loopEndMarker.transform.position = loopStartMarker.transform.position;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{_controller.passedNote}/{_controller.totalNote}";
    }

    void UpdateDeltaTimeBySlider(float sliderValue)
    {
        if ((_controller as ReplayModController).isPlaying)
            (_controller as ReplayModController).isPlaying = false;
        _controller.currentDeltaTime = (int)sliderValue;
        (_controller as ReplayModController).SyncDeltaTime(true);
        StartCoroutine((_controller as ReplayModController).UpdateNotePosByTime());
    }

    void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
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

    void OnToBeginBtnClick()
    {
        UpdateDeltaTimeBySlider(0);
    }

    void OnPlayBtnClick()
    {
        (_controller as ReplayModController).isPlaying = true;
    }

    void OnForceProgressBtnClick()
    {
        (_controller as ReplayModController).IncreaseCurrentNoteIndex();
    }

    void OnToEndBtnClick()
    {
        UpdateDeltaTimeBySlider(Managers.Midi.songLengthDelta);
        (_controller as ReplayModController).IncreaseCurrentNoteIndex();
    }

    public void ActiveLoopBtn()
    {
        _loopBtn.interactable = true;
    }

    void TurnOffLoop()
    {
        (_controller as ReplayModController).TurnOffLoop();
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
        _loopBtn.interactable = false;
    }

    void ToggleOriginalNote(bool value)
    {
        _originalNotes.SetActive(value);
    }
}
