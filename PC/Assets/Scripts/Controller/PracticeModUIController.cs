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

    private GameObject[] _sheetUpper = new GameObject[4];
    private GameObject[] _sheetLower = new GameObject[4];
    private Sprite[] _noteSprites = new Sprite[4];
    #endregion

    public void BindIngameUI()
    {
        base.BindIngameUI();
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

        songEndPanelObj = GameObject.Find("MainCanvas/SongEndPanel");
        songEndPanelObj.SetActive(false);

        /*
        _sheetUpper[0] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/UpperArea/First");
        _sheetUpper[1] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/UpperArea/Second");
        _sheetUpper[2] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/UpperArea/Third");
        _sheetUpper[3] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/UpperArea/Fourth");

        _sheetLower[0] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/LowerArea/First");
        _sheetLower[1] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/LowerArea/Second");
        _sheetLower[2] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/LowerArea/Third");
        _sheetLower[3] = GameObject.Find("MainCanvas/SheetPanel/SheetBG/LowerArea/Fourth");

        _noteSprites[0] = Resources.Load<Sprite>("Textures/FirstNote");
        _noteSprites[1] = Resources.Load<Sprite>("Textures/SecondNote");
        _noteSprites[2] = Resources.Load<Sprite>("Textures/FourthNote");
        _noteSprites[3] = Resources.Load<Sprite>("Textures/EighthNote");
        */

        _controller = Managers.Ingame.currentController as PracticeModController;

        songTimeSlider.onValueChanged.AddListener(UpdateDeltaTimeBySlider);
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;

        _toBeginBtn.onClick.AddListener(OnToBeginBtnClick);
        _playBtn.onClick.AddListener(OnPlayBtnClick);
        _forceProgressBtn.onClick.AddListener(OnForceProgressBtnClick);
        _toEndBtn.onClick.AddListener(OnToEndBtnClick);
        _loopBtn.onClick.AddListener(TurnOffLoop);

        //ShowSheet();
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

    void ShowSheet()
    {
        GameObject notePrefab = Resources.Load("Prefabs/SheetNote") as GameObject;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < Managers.Midi.bars[i].notes.Count; j++)
            {
                switch (Managers.Midi.bars[i].notes[j].noteKind)
                {
                    case Define.NoteKind.First:
                        notePrefab.GetComponent<Image>().sprite = _noteSprites[0];
                        break;
                    case Define.NoteKind.Second:
                        notePrefab.GetComponent<Image>().sprite = _noteSprites[1];
                        break;
                    case Define.NoteKind.Fourth:
                        notePrefab.GetComponent<Image>().sprite = _noteSprites[2];
                        break;
                    case Define.NoteKind.Eighth:
                        notePrefab.GetComponent<Image>().sprite = _noteSprites[3];
                        break;
                }

                if (Managers.Midi.bars[i].notes[j].isShapeFourth)
                    notePrefab.GetComponent<Image>().sprite = _noteSprites[2];

                if (Managers.Midi.bars[i].notes[j].isUpper)
                {
                    Instantiate(notePrefab, _sheetUpper[i].transform).transform.localPosition = Managers.Midi.bars[i].notes[j].position;
                }
                else
                {
                    Instantiate(notePrefab, _sheetLower[i].transform).transform.localPosition = Managers.Midi.bars[i].notes[j].position;
                }
            }
        }
    }
}
