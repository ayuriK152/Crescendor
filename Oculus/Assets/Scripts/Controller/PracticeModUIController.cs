using UnityEngine;
using UnityEngine.UI;

public class PracticeModUIController : IngameUIController
{
    #region Public Members
    public GameObject loopStartMarker;
    public Image loopStartMarkerSprite;
    public GameObject loopEndMarker;
    public Image loopEndMarkerSprite;
    public GameObject songEndPanelObj;
    #endregion

    #region Private Members
    private Button _forceScrollBtn;
    private Button _autoScrollBtn;
    private Button _loopBtn;
    private Button _toBeginBtn;
    private Button _playBtn;
    private Button _forceProgressBtn;
    private Button _toEndBtn;
    #endregion

    public void BindIngameUI()
    {
        base.BindIngameUI();
        loopStartMarker = GameObject.Find("UI/Canvas/TimeSlider/Slider/LoopStartMarker");
        loopEndMarker = GameObject.Find("UI/Canvas/TimeSlider/Slider/LoopEndMarker");
        loopStartMarkerSprite = loopStartMarker.GetComponent<Image>();
        loopEndMarkerSprite = loopEndMarker.GetComponent<Image>();

        _toBeginBtn = GameObject.Find("UI/Canvas/TimeSlider/ToBeginBtn").GetComponent<Button>();
        _playBtn = GameObject.Find("UI/Canvas/TimeSlider/PlayBtn").GetComponent<Button>();
        _forceProgressBtn = GameObject.Find("UI/Canvas/TimeSlider/ForceProgressBtn").GetComponent<Button>();
        _toEndBtn = GameObject.Find("UI/Canvas/TimeSlider/ToEndBtn").GetComponent<Button>();
        _loopBtn = GameObject.Find("UI/Canvas/TimeSlider/LoopBtn").GetComponent<Button>();
        _loopBtn.interactable = false;

        // songEndPanelObj = GameObject.Find("MainCanvas/SongEndPanel");
        // songEndPanelObj.SetActive(false);

        _controller = Managers.Ingame.currentController as PracticeModController;

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        _toBeginBtn.onClick.AddListener(OnToBeginBtnClick);
        _playBtn.onClick.AddListener(OnPlayBtnClick);
        _forceProgressBtn.onClick.AddListener(OnForceProgressBtnClick);
        _toEndBtn.onClick.AddListener(OnToEndBtnClick);
        _loopBtn.onClick.AddListener(TurnOffLoop);
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

    public void ActiveLoopBtn()
    {
        _loopBtn.interactable = true;
    }

    void TurnOffLoop()
    {
        (_controller as PracticeModController).TurnOffLoop();
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
        _loopBtn.interactable = false;
    }

    void UpdateDeltaTimeBySlider(float sliderValue)
    {
        if ((_controller as PracticeModController).isSongEnd)
        {
            songEndPanelObj.SetActive(false);
            (_controller as PracticeModController).isSongEnd = false;
        }
        if ((_controller as PracticeModController).isPlaying)
            (_controller as PracticeModController).isPlaying = false;
        (_controller as PracticeModController).currentDeltaTime = (int)sliderValue;
        (_controller as PracticeModController).SyncDeltaTime(true);
        StartCoroutine((_controller as PracticeModController).ForceUpdateNote());
    }

    void ForceScrollBtn()
    {
        (_controller as PracticeModController).isPlaying = true;
        (_controller as PracticeModController).UpdatePassedNote();
        (_controller as PracticeModController).UpdateTempo();
        (_controller as PracticeModController).UpdateBeat();
    }

    void AutoScrollBtn()
    {
        (_controller as PracticeModController).AutoScroll();
    }

    protected override void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }

    public void ToggleSongEndPanel()
    {
        songEndPanelObj.SetActive(!songEndPanelObj.activeSelf);
    }

    void OnToBeginBtnClick()
    {
        UpdateDeltaTimeBySlider(0);
    }

    void OnPlayBtnClick()
    {
        (_controller as PracticeModController).isPlaying = true;
    }

    void OnForceProgressBtnClick()
    {
        (_controller as PracticeModController).UpdatePassedNote();
        (_controller as PracticeModController).UpdateTempo();
        (_controller as PracticeModController).UpdateBeat();
    }

    void OnToEndBtnClick()
    {
        (_controller as PracticeModController).UpdatePassedNote();
        (_controller as PracticeModController).UpdateTempo();
        (_controller as PracticeModController).UpdateBeat();
        (_controller as PracticeModController).isSongEnd = true;
        UpdateDeltaTimeBySlider(Managers.Midi.songLengthDelta);
    }
}
