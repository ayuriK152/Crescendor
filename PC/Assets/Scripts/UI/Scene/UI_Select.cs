using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Define;

public class UI_Select : UI_Scene
{
    enum GameObjects
    {
        SongPanel,
        RankPanel,
        CurriculumPanel,
        CurriculumSongPanel,
        SongInfoPanel,
        SongListScrollView,
        CurriListScrollView,
        CurriSongListScrollView,
        RankListScrollView,
    }

    enum Buttons
    {
        MainMenuButton,
        OptionButton,
        ProfileButton,
        SongListButton,
        CurriculumButton
    }

    enum Dropdowns
    {
        RankCategory
    }

    Dictionary<Curriculum, GameObject> _buttonByCurriculums;
    GameObject _rankPanelObj;
    GameObject _noRankSignPanelObj;
    GameObject _songInfoPanel;
    GameObject _songListScrollView;
    GameObject _curriListScrollView;
    GameObject _curriSongListScrollView;
    GameObject _rankListScrollView;
    Button _mainMenuBtn;
    Button _optionBtn;
    Button _profileBtn;
    Button _songListBtn;
    Button _curriListBtn;
    TextMeshProUGUI _profileName;
    TextMeshProUGUI _songInfoName;
    TextMeshProUGUI _songInfoComposser;
    TextMeshProUGUI _songInfoLength;
    TextMeshProUGUI _songInfoTempo;
    TMP_Dropdown _rankListDropdown;
    Define.RankRecordList rankRecords;
    Sprite originalSprite;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Dropdown>(typeof(Dropdowns));
        GameObject songPanel = Get<GameObject>((int)GameObjects.SongPanel);
        GameObject curriculumPanel = Get<GameObject>((int)GameObjects.CurriculumPanel);
        Managers.Song.LoadSongsFromConvertsFolder();
        foreach (Transform child in songPanel.transform)
            Managers.Data.Destroy(child.gameObject);

        _rankPanelObj = Get<GameObject>((int)GameObjects.RankPanel);
        _songInfoPanel = Get<GameObject>((int)GameObjects.SongInfoPanel);
        _songListScrollView = Get<GameObject>((int)GameObjects.SongListScrollView);
        _curriListScrollView = Get<GameObject>((int)GameObjects.CurriListScrollView);
        _curriSongListScrollView = Get<GameObject>((int)GameObjects.CurriSongListScrollView);
        _rankListScrollView = Get<GameObject>((int)GameObjects.RankListScrollView);
        _curriListScrollView.SetActive(false);
        _curriSongListScrollView.SetActive(false);

        _mainMenuBtn = Get<Button>((int)Buttons.MainMenuButton);
        _optionBtn = Get<Button>((int)Buttons.OptionButton);
        _profileBtn = Get<Button>((int)Buttons.ProfileButton);
        _songListBtn = Get<Button>((int)Buttons.SongListButton);
        _curriListBtn = Get<Button>((int)Buttons.CurriculumButton);
        _songListBtn.interactable = false;
        _songListBtn.onClick.AddListener(SwapListView);
        _curriListBtn.onClick.AddListener(SwapListView);

        _rankListDropdown = Get<TMP_Dropdown>((int)Dropdowns.RankCategory);

        _mainMenuBtn.onClick.AddListener(OnMainMenuButtonClick);
        _profileBtn.onClick.AddListener(OnProfileButtonClick);

        _rankListDropdown.onValueChanged.AddListener(OnRankCategoryValueChanged);

        _profileName = _profileBtn.transform.Find("Name").transform.GetComponent<TextMeshProUGUI>();
        _profileName.text = Managers.Data.userId;
        _songInfoName = _songInfoPanel.transform.Find("Detail/SongName").GetComponent<TextMeshProUGUI>();
        _songInfoName.text = "good";
        _songInfoComposser = _songInfoPanel.transform.Find("Detail/ComposerName").GetComponent<TextMeshProUGUI>();
        _songInfoLength = _songInfoPanel.transform.Find("Detail/SongLength/Value").GetComponent<TextMeshProUGUI>();
        _songInfoTempo = _songInfoPanel.transform.Find("Detail/Tempo/Value").GetComponent<TextMeshProUGUI>();

        _rankListDropdown = transform.Find("RankListScrollView/RankCategory").GetComponent<TMP_Dropdown>();

        _noRankSignPanelObj = _rankPanelObj.transform.parent.Find("NoRankExists").gameObject;

        // SongManager의 곡 정보를 이용하여 버튼 생성
        for (int i = 0; i < Managers.Song.songs.Count; i++)
        {
            // SongButton 프리팹 로드
            GameObject songButtonPrefab = Managers.Data.Instantiate($"UI/Sub/SongButton", songPanel.transform);
            // SongButton 생성
            if (songButtonPrefab != null)
            {
                Button button = songButtonPrefab.GetComponent<Button>();

                // Song 정보를 버튼에 표시
                if (button != null)
                {
                    // 예시로 Song의 songTitle을 버튼에 표시
                    button.gameObject.name = $"{i}";
                    button.transform.Find("Title/Value").GetComponent<TextMeshProUGUI>().text = Managers.Song.songs[i].songTitle;
                    button.transform.Find("Composer/Value").GetComponent<TextMeshProUGUI>().text = Managers.Song.songs[i].songComposer;
                    button.onClick.AddListener(() => OnSongButtonClick(Convert.ToInt32(button.gameObject.name)));
                }
            }
            else
            {
                Debug.LogError($"Failed to load SongButton prefab");
            }
        }

        _buttonByCurriculums = new Dictionary<Curriculum, GameObject>();

        // 커리큘럼 버튼 리스트 생성
        foreach (Curriculum curriculum in Enum.GetValues(typeof(Curriculum)))
        {
            if (curriculum == Curriculum.None)
                continue;

            GameObject curriculumButtonPrefab = Managers.Data.Instantiate($"UI/Sub/CurriculumButton", curriculumPanel.transform);
            if (curriculumButtonPrefab != null)
            {
                _buttonByCurriculums.Add(curriculum, curriculumButtonPrefab);
                Button button = curriculumButtonPrefab.GetComponent<Button>();

                if (button != null)
                {
                    button.gameObject.name = $"{curriculum}";
                    button.transform.Find("Title/Value").GetComponent<TextMeshProUGUI>().text = curriculum.ToString();
                }
            }
            else
            {
                Debug.LogError($"Failed to load CurriculumButton prefab");
            }
        }

        PlayerPrefs.SetString("trans_SongTitle", $"{Managers.Song.songs[0].songTitle.Replace(" ", "_")}-{Managers.Song.songs[0].songComposer.Replace(" ", "_")}");
        Managers.Song.selectedSong = Managers.Song.songs[0];
        Managers.Song.selectedCurriculum = Curriculum.Hanon;
        Managers.Song.isModCurriculum = false;

        UpdateRankList();
        UpdateSongInfo();
        UpdateCurriculumSongList();

        if (Managers.Data.isUserLoggedIn)
        {
            int curriculumMount = 0;
            foreach (Curriculum curriculum in Enum.GetValues(typeof(Curriculum)))
            {
                if (curriculum == Curriculum.None)
                    continue;
                curriculumMount += Managers.Song.curriculumSongMounts[curriculum];
                if (Managers.Data.userCurriculumProgress < curriculumMount)
                {
                    _buttonByCurriculums[curriculum].transform.Find("Progress/ProgressBar/Value").GetComponent<TextMeshProUGUI>().text = $"{Math.Truncate((Managers.Data.userCurriculumProgress - curriculumMount + Managers.Song.curriculumSongMounts[curriculum]) / (float)Managers.Song.curriculumSongMounts[curriculum] * 1000) / 10}%";
                    _buttonByCurriculums[curriculum].transform.Find("Progress/ProgressBar/ProgressSlider").GetComponent<Slider>().value = (Managers.Data.userCurriculumProgress - curriculumMount + Managers.Song.curriculumSongMounts[curriculum]) / (float)Managers.Song.curriculumSongMounts[curriculum] * 100;
                }
                else
                {
                    _buttonByCurriculums[curriculum].transform.Find("Progress/ProgressBar/Value").GetComponent<TextMeshProUGUI>().text = "100%";
                    _buttonByCurriculums[curriculum].transform.Find("Progress/ProgressBar/ProgressSlider").GetComponent<Slider>().value = 100f;
                }
            }
        }

        // 프로필 이미지 로드
        if (Managers.Data.isUserLoggedIn)
        {
            LoadImage();
        }
    }

    public void OnSongButtonClick(int songIdx)
    {
        string selectedSongTitle = $"{Managers.Song.songs[songIdx].songTitle.Replace(" ", "_")}-{Managers.Song.songs[songIdx].songComposer.Replace(" ", "_")}";
        string currentSongTitle = PlayerPrefs.GetString("trans_SongTitle");
        if (currentSongTitle != selectedSongTitle && !_songListBtn.interactable)
        {
            PlayerPrefs.SetString("trans_SongTitle", selectedSongTitle);
            Managers.Song.selectedSong = Managers.Song.songs[songIdx];
            UpdateRankList();
            UpdateSongInfo();
        }
        else
        {
            if (currentSongTitle != selectedSongTitle)
            {
                PlayerPrefs.SetString("trans_SongTitle", selectedSongTitle);
                Managers.Song.selectedSong = Managers.Song.songs[songIdx];
                UpdateRankList();
                UpdateSongInfo();
            }

            (Managers.UI.currentUIController as BaseUIController).ShowPopupUI<UI_SongPopup>();
        }
    }

    public void OnRankButtonClick(PointerEventData data)
    {
        int recordIdx = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.name.Split("-")[0]);
        Managers.Data.rankRecord = rankRecords.records[recordIdx];
        if (_rankListDropdown.itemText.text == "온라인 랭크")
        {
            for (int i = 0; i < Managers.Data.rankRecord.midi.Length; i++)
            {
                if (Managers.Data.rankRecord.midi[i] == '[')
                {
                    Managers.Data.rankRecord.midi = Managers.Data.rankRecord.midi.Remove(i, 1);
                    break;
                }
            }
            for (int i = Managers.Data.rankRecord.midi.Length - 1; i >= 0; i--)
            {
                if (Managers.Data.rankRecord.midi[i] == ']')
                {
                    Managers.Data.rankRecord.midi = Managers.Data.rankRecord.midi.Remove(i, 1);
                    break;
                }
            }
        }
        (Managers.UI.currentUIController as BaseUIController).ShowPopupUI<UI_RankPopUp>().gameObject.name = recordIdx.ToString();
    }

    public void OnInstantReplayButtonClick(PointerEventData data)
    {
        int recordIdx = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.transform.parent.name.Split("-")[0]);
        Managers.Data.rankRecord = rankRecords.records[recordIdx];
        if (_rankListDropdown.options[_rankListDropdown.value].text == "온라인 랭크")
        {
            for (int i = 0; i < Managers.Data.rankRecord.midi.Length; i++)
            {
                if (Managers.Data.rankRecord.midi[i] == '[')
                {
                    Managers.Data.rankRecord.midi = Managers.Data.rankRecord.midi.Remove(i, 1);
                    break;
                }
            }
            for (int i = Managers.Data.rankRecord.midi.Length - 1; i >= 0; i--)
            {
                if (Managers.Data.rankRecord.midi[i] == ']')
                {
                    Managers.Data.rankRecord.midi = Managers.Data.rankRecord.midi.Remove(i, 1);
                    break;
                }
            }
        }
        Managers.Scene.LoadScene(Define.Scene.ReplayModScene);
    }

    public void OnMainMenuButtonClick()
    {
        Managers.Scene.LoadScene(Define.Scene.MainMenuScene);
    }

    public void OnOptionButtonClick()
    {

    }

    public void OnProfileButtonClick()
    {
        if (Managers.Data.isUserLoggedIn)
        {
            Managers.Scene.LoadScene(Define.Scene.MyPageScene);
        }
        else
        {
            Debug.Log("로그인 상태가 아닙니다!");
        }
    }

    void OnRankCategoryValueChanged(int value)
    {
        switch (value)
        {
            case 0:
                UpdateRankList();
                break;
            case 1:
                UpdateLocalRankList();
                break;
        }
    }

    void UpdateRankList()
    {
        if (PlayerPrefs.GetString("trans_SongTitle") == "")
        {
            _noRankSignPanelObj.SetActive(true);
            return;
        }
        foreach (Transform child in _rankPanelObj.transform)
        {
            Destroy(child.gameObject);
        }

        string songFileName = PlayerPrefs.GetString("trans_SongTitle");
        Managers.Data.GetRankListFromServer(songFileName);

        if (Managers.Data.isServerConnectionComplete)
        {
            rankRecords = JsonUtility.FromJson<Define.RankRecordList>(Managers.Data.jsonDataFromServer);
            Managers.Data.jsonDataFromServer = "init data";

            if (rankRecords.records.Count == 0)
            {
                _noRankSignPanelObj.SetActive(true);
            }

            else
            {
                _noRankSignPanelObj.SetActive(false);
            }

            for (int i = 0; i < rankRecords.records.Count; i++)
            {
                GameObject rankButtonInstance = Managers.Data.Instantiate($"UI/Sub/RankButton", _rankPanelObj.transform);
                rankButtonInstance.name = $"{i}";
                if (rankButtonInstance != null)
                {
                    Button button = rankButtonInstance.GetComponent<Button>();
                    button.gameObject.BindEvent(OnRankButtonClick);
                    button.transform.Find("ReplayButton").gameObject.BindEvent(OnInstantReplayButtonClick);

                    if (button != null)
                    {
                        button.transform.Find("Ranking").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
                        button.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{rankRecords.records[i].user_id}";
                        button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"정확도: {Math.Truncate(rankRecords.records[i].score * 10000) / 100}%";
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load RankButton prefab");
                }
            }
        }

        else
        {
            _noRankSignPanelObj.SetActive(false);
        }
    }

    void UpdateLocalRankList()
    {
        foreach (Transform child in _rankPanelObj.transform)
        {
            Destroy(child.gameObject);
        }

        string songFileName = PlayerPrefs.GetString("trans_SongTitle");
        rankRecords = Managers.Data.GetRankListFromLocal(songFileName);
        // 점수 내림차순 정렬, 람다식 사용함. 동점인 경우 날짜순으로 정렬되도록 수정 필요.(날짜가 지금 문자열이라 넘겨둠)
        rankRecords.records.Sort((Define.RankRecord a, Define.RankRecord b) => {
            if (a.score > b.score)
                return -1;
            else if (a.score == b.score)
                return 0;
            else
                return 1;
        });

        if (rankRecords.records.Count == 0)
        {
            _noRankSignPanelObj.SetActive(true);
        }

        else
        {
            _noRankSignPanelObj.SetActive(false);
        }

        for (int i = 0; i < rankRecords.records.Count; i++)
        {
            GameObject rankButtonInstance = Managers.Data.Instantiate($"UI/Sub/RankButton", _rankPanelObj.transform);
            rankButtonInstance.name = $"{i}";
            if (rankButtonInstance != null)
            {
                Button button = rankButtonInstance.GetComponent<Button>();
                button.gameObject.BindEvent(OnRankButtonClick);
                button.transform.Find("ReplayButton").gameObject.BindEvent(OnInstantReplayButtonClick);

                if (button != null)
                {
                    button.transform.Find("Ranking").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
                    button.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{rankRecords.records[i].user_id}";
                    button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"정확도: {Math.Truncate(rankRecords.records[i].score * 10000) / 100}%";
                }
            }
            else
            {
                Debug.LogError($"Failed to load RankButton prefab");
            }
        }
    }

    void UpdateSongInfo()
    {
        if (PlayerPrefs.GetString("trans_SongTitle") == "")
        {
            _songInfoName.text = "";
            _songInfoComposser.text = "";
            _songInfoTempo.text = "";
            return;
        }
        Managers.Midi.LoadMidi(PlayerPrefs.GetString("trans_SongTitle"));
        _songInfoName.text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[0].Replace("_", " ");
        _songInfoComposser.text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[1].Replace("_", " ");
        _songInfoTempo.text = Managers.Midi.tempo.ToString();
    }

    void UpdateCurriculumSongList()
    {
        GameObject curriculumSongPanel = Get<GameObject>((int)GameObjects.CurriculumSongPanel);
        Managers.Song.curriculumSongMounts.Clear();

        for (int i = 0; i < Managers.Song.songs.Count; i++)
        {
            if (Managers.Song.songs[i].curriculum != Managers.Song.selectedCurriculum)
                continue;
            if (!Managers.Song.curriculumSongMounts.ContainsKey(Managers.Song.songs[i].curriculum))
            {
                Managers.Song.curriculumSongMounts.Add(Managers.Song.songs[i].curriculum, 0);
            }
            Managers.Song.curriculumSongMounts[Managers.Song.songs[i].curriculum]++;
            GameObject songButtonPrefab = Managers.Data.Instantiate($"UI/Sub/SongButton", curriculumSongPanel.transform);
            Button button = songButtonPrefab.GetComponent<Button>();

            button.gameObject.name = $"{i}";
            button.transform.Find("Title/Value").GetComponent<TextMeshProUGUI>().text = Managers.Song.songs[i].songTitle;
            button.transform.Find("Composer/Value").GetComponent<TextMeshProUGUI>().text = Managers.Song.songs[i].songComposer;
            button.onClick.AddListener(() => OnSongButtonClick(Convert.ToInt32(button.gameObject.name)));
        }
    }

    void SwapListView()
    {
        Managers.Song.isModCurriculum = !Managers.Song.isModCurriculum;
        _songListBtn.interactable = !_songListBtn.interactable;
        _curriListBtn.interactable = !_curriListBtn.interactable;
        _songListScrollView.SetActive(!_songListScrollView.active);
        _curriListScrollView.SetActive(!_curriListScrollView.active);
        _curriSongListScrollView.SetActive(!_curriSongListScrollView.active);
        _rankListScrollView.SetActive(!_rankListScrollView.active);
        _songInfoPanel.SetActive(!_songInfoPanel.active);
    }

    #region Image Settings
    public void LoadImage()
    {
        StartCoroutine(SetProfileImage(Managers.Data.userProfileURL));
    }

    private IEnumerator SetProfileImage(string imageURL)
    {
        using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                // 텍스처 다운로드 및 이미지 UI에 설정
                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                originalSprite = _profileBtn.GetComponent<UnityEngine.UI.Image>().sprite; // 원래 이미지를 저장
                _profileBtn.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }


    private string GetProfileURL(string json)
    {
        // JSON 문자열을 파싱하여 profile 정보 추출
        string profileURL = "";

        // JSON 문자열을 파싱하고 원하는 정보를 추출하는 로직을 작성
        // 여기서는 간단하게 문자열을 찾아내는 방식으로 작성하였습니다.
        int startIndex = json.IndexOf("\"profile\":") + "\"profile\":".Length + 1;
        int endIndex = json.IndexOf("\"", startIndex + 1);
        profileURL = json.Substring(startIndex, endIndex - startIndex);

        return profileURL;
    }

    #endregion Image Settings 
}

