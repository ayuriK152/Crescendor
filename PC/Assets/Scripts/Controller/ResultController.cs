using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static Define;

public class ResultController : MonoBehaviour
{
    string _songTitle;
    string _songComposer;
    int _songLength;

    string _username = Managers.Data.userId;
    int _totalAcc;
    int _failMount;
    int _correctMount;
    int _outlinerMount;

    ResultUIController _uiController;

    public void Init()
    {
        _songTitle = PlayerPrefs.GetString("trans_SongTitle").Split('-')[0].Replace("_", " ");
        _songComposer = PlayerPrefs.GetString("trans_SongTitle").Split('-')[1].Replace("_", " ");
        _songLength = Managers.Midi.songLengthDelta;

        _totalAcc = Managers.Midi.totalDeltaTime;
        _failMount = PlayerPrefs.GetInt("trans_FailMount");
        _correctMount = _totalAcc - _failMount;
        _outlinerMount = PlayerPrefs.GetInt("trans_OutlinerMount");

        _uiController = Managers.UI.currentUIController as ResultUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = $"{_songTitle}";
        _uiController.songComposerTMP.text = $"{_songComposer}";
        _uiController.correctMountTMP.text = $"{_correctMount}";
        _uiController.failMountTMP.text = $"{_failMount}";
        _uiController.outlinerMountTMP.text = $"{_outlinerMount}";
        _uiController.songLengthTMP.text = $"{(int)(Managers.Midi.songLengthSecond / 60)}:{(int)(Managers.Midi.songLengthSecond % 60)}";
        _uiController.correctGraphImage.fillAmount = _correctMount / (float)(_totalAcc + _outlinerMount);
        _uiController.failGraphImage.fillAmount = _failMount / (float)(_failMount + _outlinerMount);
        _uiController.accuracyTMP.text = $"{Convert.ToInt32((_correctMount / (float)_totalAcc) * 10000.0f) / 100.0f}%";

        SaveResultToJson();
        UpdateBestResult();
    }

    void SaveResultToJson()
    {
        RankRecord tempRankRecord = new RankRecord(PlayerPrefs.GetString("trans_SongTitle"), $"{_username}", _correctMount / (float)_totalAcc, $"{DateTime.Now.ToString("yyyy-MM-dd")}T{DateTime.Now.ToString("HH:mm:ss")}.000Z", JsonConvert.SerializeObject(Managers.Data.userReplayRecord));
        File.WriteAllText($"{Application.dataPath}/RecordReplay/{_songTitle}-{_username}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.json", JsonConvert.SerializeObject(Managers.Data.userReplayRecord));
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
}
