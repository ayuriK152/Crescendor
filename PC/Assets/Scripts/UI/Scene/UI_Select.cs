using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Select : UI_Scene
{
    enum GameObjects
    {
        SongPanel,
        RankPanel,
        SongInfoPanel,
    }

    enum Buttons
    {
        RankButton,
        MainMenuButton,
        OptionButton,
        ProfileButton,
    }

    enum Dropdowns
    {
        RankCategory,
        SongCategory,
    }

    GameObject _rankPanelObj;
    GameObject _noRankSignPanelObj;
    GameObject _songInfoPanel;
    Button _mainMenuBtn;
    Button _optionBtn;
    Button _profileBtn;
    TextMeshProUGUI _songInfoName;
    TextMeshProUGUI _songInfoComposser;
    TextMeshProUGUI _songInfoLength;
    TextMeshProUGUI _songInfoTempo;
    TMP_Dropdown _rankListDropdown;
    TMP_Dropdown _songListDropdown;
    Define.RankRecordList rankRecords;

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
        Managers.Song.LoadSongsFromConvertsFolder();
        foreach (Transform child in songPanel.transform)
            Managers.Data.Destroy(child.gameObject);

        _rankPanelObj = Get<GameObject>((int)GameObjects.RankPanel);
        _songInfoPanel = Get<GameObject>((int)GameObjects.SongInfoPanel);

        _mainMenuBtn = Get<Button>((int)Buttons.MainMenuButton);
        _optionBtn = Get<Button>((int)Buttons.OptionButton);
        _profileBtn = Get<Button>((int)Buttons.ProfileButton);

        _rankListDropdown = Get<TMP_Dropdown>((int)Dropdowns.RankCategory);
        _songListDropdown = Get<TMP_Dropdown>((int)Dropdowns.SongCategory);

        _mainMenuBtn.onClick.AddListener(OnMainMenuButtonClick);
        _profileBtn.onClick.AddListener(OnProfileButtonClick);

        _rankListDropdown.onValueChanged.AddListener(OnRankCategoryValueChanged);

        _songInfoName = _songInfoPanel.transform.Find("Detail/SongName").GetComponent<TextMeshProUGUI>();
        _songInfoName.text = "good";
        _songInfoComposser = _songInfoPanel.transform.Find("Detail/ComposerName").GetComponent<TextMeshProUGUI>();
        _songInfoLength = _songInfoPanel.transform.Find("Detail/SongLength/Value").GetComponent<TextMeshProUGUI>();
        _songInfoTempo = _songInfoPanel.transform.Find("Detail/Tempo/Value").GetComponent<TextMeshProUGUI>();

        _rankListDropdown = transform.Find("RankListScrollView/RankCategory").GetComponent<TMP_Dropdown>();
        _songListDropdown = transform.Find("SongListScrollView/SongCategory").GetComponent<TMP_Dropdown>();

        _noRankSignPanelObj = _rankPanelObj.transform.parent.Find("NoRankExists").gameObject;

        PlayerPrefs.SetString("trans_SongTitle", "CanCan-Jacques_Offenbach");

        UpdateRankList();
        UpdateSongInfo();

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
                    button.transform.Find("Title/Value").GetComponent<TextMeshProUGUI>().text = Managers.Song.songs[i].songTitle;
                    button.transform.Find("Composer/Value").GetComponent<TextMeshProUGUI>().text = Managers.Song.songs[i].songComposer;
                    button.onClick.AddListener(() => OnSongButtonClick($"{button.transform.Find("Title/Value").GetComponent<TextMeshProUGUI>().text.Replace(" ", "_")}-{button.transform.Find("Composer/Value").GetComponent<TextMeshProUGUI>().text.Replace(" ", "_")}"));
                }
            }
            else
            {
                Debug.LogError($"Failed to load SongButton prefab");
            }
        }
    }

    public void OnSongButtonClick(string songName)
    {
        if (!PlayerPrefs.HasKey("trans_SongTitle"))
            PlayerPrefs.SetString("trans_SongTitle", "");
        string currentSongTitle = PlayerPrefs.GetString("trans_SongTitle");
        if (currentSongTitle != songName)
        {
            PlayerPrefs.SetString("trans_SongTitle", songName);
            UpdateRankList();
            UpdateSongInfo();
        }
        else if (currentSongTitle == songName)
        {
            (Managers.UI.currentUIController as OutGameUIController).ShowPopupUI<UI_SongPopup>();
        }
    }

    public void OnRankButtonClick(PointerEventData data)
    {
        int recordIdx = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.name.Split("-")[0]);
        Managers.Data.rankRecord = rankRecords.records[recordIdx];
        if (_rankListDropdown.itemText.text == "Online Rank")
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
        (Managers.UI.currentUIController as OutGameUIController).ShowPopupUI<UI_RankPopUp>().gameObject.name = recordIdx.ToString();
    }

    public void OnInstantReplayButtonClick(PointerEventData data)
    {
        int recordIdx = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.transform.parent.name.Split("-")[0]);
        Managers.Data.rankRecord = rankRecords.records[recordIdx];
        if (_rankListDropdown.options[_rankListDropdown.value].text == "Online Rank")
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
                        button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"Accuracy: {rankRecords.records[i].score}";
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
                    button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"Accuracy: {rankRecords.records[i].score}";
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
}

