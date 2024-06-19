using Newtonsoft.Json;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ResultController : MonoBehaviour
{
    string _songTitle;
    string _songComposer;
    int _songLength;

    string _username;
    int _totalAcc;
    int _failMount;
    int _correctMount;

    ResultUIController _uiController;
    RankRecordList rankRecords;
    GameObject _rankPanelObj;

    public void Init()
    {
        _songTitle = PlayerPrefs.GetString("trans_SongTitle").Split('-')[0].Replace("_", " ");
        _songComposer = PlayerPrefs.GetString("trans_SongTitle").Split('-')[1].Replace("_", " ");
        _songLength = Managers.Midi.songLengthDelta;

        if (Managers.Data.isUserLoggedIn)
            _username = Managers.Data.userId;
        else
            _username = "Guest";

        _totalAcc = Managers.Midi.totalDeltaTime;
        _failMount = PlayerPrefs.GetInt("trans_FailMount");
        _correctMount = _totalAcc - _failMount;

        _uiController = Managers.UI.currentUIController as ResultUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = $"{_songTitle}";
        _uiController.songComposerTMP.text = $"{_songComposer}";
        _uiController.correctMountTMP.text = $"{_correctMount}";
        _uiController.failMountTMP.text = $"{_failMount}";
        _uiController.songLengthTMP.text = $"{(int)(Managers.Midi.songLengthSecond / 60)}:{(int)(Managers.Midi.songLengthSecond % 60)}";
        _uiController.correctGraphImage.fillAmount = _correctMount / (float)_totalAcc;
        _uiController.failGraphImage.fillAmount = _failMount / (float)_failMount;
        _uiController.accuracyTMP.text = $"{Convert.ToInt32((_correctMount / (float)_totalAcc) * 10000.0f) / 100.0f}%";

        _rankPanelObj = GameObject.Find("MainCanvas/TopRanks/RankListScrollView/Viewport/RankPanel");

        SaveResultToJson();
        if (Managers.Data.isUserLoggedIn)
        {
            if (!Managers.Song.isModCurriculum)
            {
                UpdateBestResult();
            }
            else
            {
                UpdateCurriculumProgress();
            }
        }
        try
        {
            UpdateHighScores();
        }
        catch (Exception ex)
        {

        }
        if (Managers.Data.isUserLoggedIn && !Managers.Song.isModCurriculum)
        {
            for (int i = 0; i < rankRecords.records.Count; i++)
            {
                if (rankRecords.records[i].user_id == Managers.Data.userId)
                {
                    _uiController.playerRankTMP.text = $"My Rank: # {i + 1}";
                    break;
                }
            }
        }
        else
        {
            _uiController.playerRankTMP.text = $"My Rank: # --";
        }
    }

    void SaveResultToJson()
    {
        if (!Directory.Exists($"{Application.persistentDataPath}/RecordReplay"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/RecordReplay");
        }
        File.WriteAllText($"{Application.persistentDataPath}/RecordReplay/{_songTitle}-{_username}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.json", JsonConvert.SerializeObject(Managers.Data.userReplayRecord));
    }

    void UpdateBestResult()
    {
        float bestScoreFromServer = Managers.Data.GetBestRankFromServer($"{_username}", PlayerPrefs.GetString("trans_SongTitle"));
        if (bestScoreFromServer == -2)
            return;
        if (bestScoreFromServer == -1)
        {
            Managers.Data.AddBestRankToServer($"{_username}", PlayerPrefs.GetString("trans_SongTitle"), _correctMount / (float)_totalAcc, Managers.Data.userReplayRecord);
        }
        else if(bestScoreFromServer < _correctMount / (float)_totalAcc)
        {
            Managers.Data.SetBestRankToServer($"{_username}", PlayerPrefs.GetString("trans_SongTitle"), _correctMount / (float)_totalAcc, Managers.Data.userReplayRecord);
        }
        else
        {
            Debug.Log("Á¡¼ö°¡ ³·´Ù!");
        }
    }

    void UpdateCurriculumProgress()
    {
        int playedIdx = -1;
        for (int i = 0; i < Managers.Song.curriculumIdx.Count; i++)
        {
            if (Managers.Song.curriculumIdx[i].Key == _songTitle)
            {
                playedIdx = i;
                break;
            }
        }

        if (playedIdx == Managers.Data.userCurriculumProgress)
        {
            Managers.Data.userCurriculumProgress += 1;
            Managers.Data.SetUserCurriculumProgress(Managers.Data.userCurriculumProgress);
        }
    }

    void UpdateHighScores()
    {
        string songFileName = PlayerPrefs.GetString("trans_SongTitle");
        Managers.Data.GetRankListFromServer(songFileName);

        if (Managers.Data.isServerConnectionComplete)
        {
            rankRecords = JsonUtility.FromJson<Define.RankRecordList>(Managers.Data.jsonDataFromServer);
            Managers.Data.jsonDataFromServer = "init data";

            for (int i = 0; i < rankRecords.records.Count && i < 5; i++)
            {
                GameObject rankButtonInstance = Managers.Data.Instantiate($"UI/Sub/RankButton", _rankPanelObj.transform);
                rankButtonInstance.name = $"{i}";
                if (rankButtonInstance != null)
                {
                    Button button = rankButtonInstance.GetComponent<Button>();
                    button.transform.Find("ReplayButton").gameObject.SetActive(false);

                    if (button != null)
                    {
                        button.transform.Find("Ranking").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
                        button.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{rankRecords.records[i].user_id}";
                        button.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = $"Accuracy: {Math.Truncate(rankRecords.records[i].score * 10000) / 100}%";
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load RankButton prefab");
                }
            }
        }
    }
}
