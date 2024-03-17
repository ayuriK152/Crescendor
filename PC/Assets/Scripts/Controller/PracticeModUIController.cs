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
    public GameObject songEndPanelObj;

    Button _forceScrollBtn;
    Button _disconnectBtn;
    Button _autoScrollBtn;
    Button _loopBtn;
    Button _resumeBtn;
    Button _optionBtn;
    Button _exitBtn;
    Button _toBeginBtn;
    Button _playBtn;
    Button _forceProgressBtn;
    Button _toEndBtn;

    PracticeModController _controller;

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

        _toBeginBtn = GameObject.Find("MainCanvas/TimeSlider/ToBeginBtn").GetComponent<Button>();
        _playBtn = GameObject.Find("MainCanvas/TimeSlider/PlayBtn").GetComponent<Button>();
        _forceProgressBtn = GameObject.Find("MainCanvas/TimeSlider/ForceProgressBtn").GetComponent<Button>();
        _toEndBtn = GameObject.Find("MainCanvas/TimeSlider/ToEndBtn").GetComponent<Button>();
        _loopBtn = GameObject.Find("MainCanvas/TimeSlider/LoopBtn").GetComponent<Button>();
        _loopBtn.interactable = false;

        _forceScrollBtn = GameObject.Find("MainCanvas/Buttons/ForceScrollBtn").GetComponent<Button>();
        _disconnectBtn = GameObject.Find("MainCanvas/Buttons/DisconnectBtn").GetComponent<Button>();
        _autoScrollBtn = GameObject.Find("MainCanvas/Buttons/AutoScroll").GetComponent<Button>();

// 런타임에서의 디버그용 기능 버튼 비활성화
#if UNITY_EDITOR
        _forceScrollBtn.onClick.AddListener(ForceScrollBtn);
        _disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        _autoScrollBtn.onClick.AddListener(AutoScrollBtn);
#else
        _forceScrollBtn.gameObject.SetActive(false);
        _disconnectBtn.gameObject.SetActive(false);
        _autoScrollBtn.gameObject.SetActive(false);
#endif

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        _resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        _optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        _exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        songEndPanelObj = GameObject.Find("MainCanvas/SongEndPanel");
        songEndPanelObj.SetActive(false);

        _controller = Managers.Ingame.currentController as PracticeModController;

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        _toBeginBtn.onClick.AddListener(OnToBeginBtnClick);
        _playBtn.onClick.AddListener(OnPlayBtnClick);
        _forceProgressBtn.onClick.AddListener(OnForceProgressBtnClick);
        _toEndBtn.onClick.AddListener(OnToEndBtnClick);
        _loopBtn.onClick.AddListener(TurnOffLoop);
        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);
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

    public void ActiveLoopBtn()
    {
        _loopBtn.interactable = true;
    }

    void TurnOffLoop()
    {
        _controller.TurnOffLoop();
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
        _loopBtn.interactable = false;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{_controller.passedNote}/{_controller.totalNote}";
    }

    void UpdateDeltaTimeBySlider(float sliderValue)
    {
        if (_controller.isSongEnd)
        {
            songEndPanelObj.SetActive(false);
            _controller.isSongEnd = false;
        }
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

    void DisconnectPianoBtn()
    {
        _controller.DisconnectPiano();
    }

    void AutoScrollBtn()
    {
        _controller.AutoScroll();
    }

    void OnClickExitBtn()
    {
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
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
        _controller.isPlaying = true;
    }

    void OnForceProgressBtnClick()
    {
        _controller.IncreaseCurrentNoteIndex();
    }

    void OnToEndBtnClick()
    {
        UpdateDeltaTimeBySlider(Managers.Midi.songLengthDelta);
        _controller.IncreaseCurrentNoteIndex();
    }
}
