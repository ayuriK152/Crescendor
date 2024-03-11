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

    Button forceScrollBtn;
    Button disconnectBtn;
    Button autoScrollBtn;
    Button loopBtn;
    Button resumeBtn;
    Button optionBtn;
    Button exitBtn;
    Button toBeginBtn;
    Button palyBtn;
    Button forceProgressBtn;
    Button toEndBtn;

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

        toBeginBtn = GameObject.Find("MainCanvas/TimeSlider/ToBeginBtn").GetComponent<Button>();
        palyBtn = GameObject.Find("MainCanvas/TimeSlider/PlayBtn").GetComponent<Button>();
        forceProgressBtn = GameObject.Find("MainCanvas/TimeSlider/ForceProgressBtn").GetComponent<Button>();
        toEndBtn = GameObject.Find("MainCanvas/TimeSlider/ToEndBtn").GetComponent<Button>();
        loopBtn = GameObject.Find("MainCanvas/TimeSlider/LoopBtn").GetComponent<Button>();
        loopBtn.interactable = false;

        forceScrollBtn = GameObject.Find("MainCanvas/Buttons/ForceScrollBtn").GetComponent<Button>();
        disconnectBtn = GameObject.Find("MainCanvas/Buttons/DisconnectBtn").GetComponent<Button>();
        autoScrollBtn = GameObject.Find("MainCanvas/Buttons/AutoScroll").GetComponent<Button>();

// 런타임에서의 디버그용 기능 버튼 비활성화
#if UNITY_EDITOR
        forceScrollBtn.onClick.AddListener(ForceScrollBtn);
        disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        autoScrollBtn.onClick.AddListener(AutoScrollBtn);
#else
        forceScrollBtn.gameObject.SetActive(false);
        disconnectBtn.gameObject.SetActive(false);
        autoScrollBtn.gameObject.SetActive(false);
#endif

        pausePanelObj = GameObject.Find("MainCanvas/PausePanel");
        resumeBtn = pausePanelObj.transform.Find("Buttons/ResumeBtn").GetComponent<Button>();
        optionBtn = pausePanelObj.transform.Find("Buttons/OptionBtn").GetComponent<Button>();
        exitBtn = pausePanelObj.transform.Find("Buttons/ExitBtn").GetComponent<Button>();
        pausePanelObj.SetActive(false);

        songEndPanelObj = GameObject.Find("MainCanvas/SongEndPanel");
        songEndPanelObj.SetActive(false);

        practiceModController = Managers.Ingame.controller as PracticeModController;

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        toBeginBtn.onClick.AddListener(OnToBeginBtnClick);
        palyBtn.onClick.AddListener(OnPlayBtnClick);
        forceProgressBtn.onClick.AddListener(OnForceProgressBtnClick);
        toEndBtn.onClick.AddListener(OnToEndBtnClick);
        loopBtn.onClick.AddListener(TurnOffLoop);
        resumeBtn.onClick.AddListener(TogglePausePanel);
        exitBtn.onClick.AddListener(OnClickExitBtn);
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
        loopBtn.interactable = true;
    }

    void TurnOffLoop()
    {
        practiceModController.TurnOffLoop();
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
        loopBtn.interactable = false;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{practiceModController.passedNote}/{practiceModController.totalNote}";
    }

    void UpdateDeltaTimeBySlider(float sliderValue)
    {
        if (practiceModController.isSongEnd)
        {
            songEndPanelObj.SetActive(false);
            practiceModController.isSongEnd = false;
        }
        if (practiceModController.isPlaying)
            practiceModController.isPlaying = false;
        practiceModController.currentDeltaTime = (int)sliderValue;
        practiceModController.SyncDeltaTime(true);
        StartCoroutine(practiceModController.UpdateNotePosByTime());
    }

    void ForceScrollBtn()
    {
        practiceModController.isPlaying = true;
        practiceModController.IncreaseCurrentNoteIndex();
    }

    void DisconnectPianoBtn()
    {
        practiceModController.DisconnectPiano();
    }

    void AutoScrollBtn()
    {
        practiceModController.AutoScroll();
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
            practiceModController.enabled = false;
        }
        else
        {
            practiceModController.enabled = true;
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
        practiceModController.isPlaying = true;
    }

    void OnForceProgressBtnClick()
    {
        practiceModController.IncreaseCurrentNoteIndex();
    }

    void OnToEndBtnClick()
    {
        UpdateDeltaTimeBySlider(Managers.Midi.songLengthDelta);
        practiceModController.IncreaseCurrentNoteIndex();
    }
}
