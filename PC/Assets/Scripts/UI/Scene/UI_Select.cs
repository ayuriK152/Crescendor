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

    GameObject rankPanelObj;
    GameObject noRankSignPanelObj;
    GameObject songInfoPanel;
    Button mainMenuBtn;
    Button optionBtn;
    Button profileBtn;
    TextMeshProUGUI songInfoName;
    TextMeshProUGUI songInfoComposser;
    TextMeshProUGUI songInfoLength;
    TextMeshProUGUI songInfoTempo;
    TMP_Dropdown rankListDropdown;
    TMP_Dropdown songListDropdown;
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

        rankPanelObj = Get<GameObject>((int)GameObjects.RankPanel);
        songInfoPanel = Get<GameObject>((int)GameObjects.SongInfoPanel);

        mainMenuBtn = Get<Button>((int)Buttons.MainMenuButton);
        optionBtn = Get<Button>((int)Buttons.OptionButton);
        profileBtn = Get<Button>((int)Buttons.ProfileButton);

        rankListDropdown = Get<TMP_Dropdown>((int)Dropdowns.RankCategory);
        songListDropdown = Get<TMP_Dropdown>((int)Dropdowns.SongCategory);

        mainMenuBtn.onClick.AddListener(OnMainMenuButtonClick);
        profileBtn.onClick.AddListener(OnProfileButtonClick);

        rankListDropdown.onValueChanged.AddListener(OnRankCategoryValueChanged);

        songInfoName = songInfoPanel.transform.Find("Detail/SongName").GetComponent<TextMeshProUGUI>();
        songInfoName.text = "good";
        songInfoComposser = songInfoPanel.transform.Find("Detail/ComposerName").GetComponent<TextMeshProUGUI>();
        songInfoLength = songInfoPanel.transform.Find("Detail/SongLength/Value").GetComponent<TextMeshProUGUI>();
        songInfoTempo = songInfoPanel.transform.Find("Detail/Tempo/Value").GetComponent<TextMeshProUGUI>();

        rankListDropdown = transform.Find("RankListScrollView/RankCategory").GetComponent<TMP_Dropdown>();
        songListDropdown = transform.Find("SongListScrollView/SongCategory").GetComponent<TMP_Dropdown>();

        noRankSignPanelObj = rankPanelObj.transform.parent.Find("NoRankExists").gameObject;

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
        (Managers.UI.currentUIController as OutGameUIController).ShowPopupUI<UI_RankPopUp>();
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
            noRankSignPanelObj.SetActive(true);
            return;
        }
        foreach (Transform child in rankPanelObj.transform)
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
                noRankSignPanelObj.SetActive(true);
            }

            else
            {
                noRankSignPanelObj.SetActive(false);
            }

            for (int i = 0; i < rankRecords.records.Count; i++)
            {
                GameObject rankButtonPrefab = Managers.Data.Instantiate($"UI/Sub/RankButton", rankPanelObj.transform);
                if (rankButtonPrefab != null)
                {
                    Button button = rankButtonPrefab.GetComponent<Button>();
                    button.gameObject.BindEvent(OnRankButtonClick);

                    if (button != null)
                    {
                        button.transform.Find("Ranking").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
                        button.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{rankRecords.records[i].user_id}";
                        button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"Accuracy: {rankRecords.records[i].score}";
                        //button.onClick.AddListener(() => OnRankButtonClick(button.GetComponentInChildren<TextMeshProUGUI>().text));
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
            noRankSignPanelObj.SetActive(false);
        }
    }

    void UpdateLocalRankList()
    {
        foreach (Transform child in rankPanelObj.transform)
        {
            Destroy(child.gameObject);
        }

        string songFileName = PlayerPrefs.GetString("trans_SongTitle");
        rankRecords = Managers.Data.GetRankListFromLocal(songFileName);

        if (rankRecords.records.Count == 0)
        {
            noRankSignPanelObj.SetActive(true);
        }

        else
        {
            noRankSignPanelObj.SetActive(false);
        }

        for (int i = 0; i < rankRecords.records.Count; i++)
        {
            GameObject rankButtonPrefab = Managers.Data.Instantiate($"UI/Sub/RankButton", rankPanelObj.transform);
            if (rankButtonPrefab != null)
            {
                Button button = rankButtonPrefab.GetComponent<Button>();
                button.gameObject.BindEvent(OnRankButtonClick);

                if (button != null)
                {
                    button.transform.Find("Ranking").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
                    button.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{rankRecords.records[i].user_id}";
                    button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"Accuracy: {rankRecords.records[i].score}";
                    //button.onClick.AddListener(() => OnRankButtonClick(button.GetComponentInChildren<TextMeshProUGUI>().text));
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
            songInfoName.text = "";
            songInfoComposser.text = "";
            songInfoTempo.text = "";
            return;
        }
        Managers.Midi.LoadMidi(PlayerPrefs.GetString("trans_SongTitle"));
        songInfoName.text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[0].Replace("_", " ");
        songInfoComposser.text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[1].Replace("_", " ");
        songInfoTempo.text = Managers.Midi.tempo.ToString();
    }
}

