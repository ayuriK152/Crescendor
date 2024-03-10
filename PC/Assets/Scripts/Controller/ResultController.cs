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
        RankRecord tempRankRecord = new RankRecord(PlayerPrefs.GetString("trans_SongTitle"), "TestUser1", _correctMount / (float)_totalAcc, $"{DateTime.Now.ToString("yyyy-MM-dd")}T{DateTime.Now.ToString("HH:mm:ss")}.000Z", JsonConvert.SerializeObject(Managers.Data.userReplayRecord));
        File.WriteAllText($"{Application.dataPath}/RecordReplay/TestUser1{DateTime.Now.ToString("yyyyMMddHHmmss")}.json", JsonConvert.SerializeObject(Managers.Data.userReplayRecord));
    }

    string ReturnStringForServer(string origin)
    {
        Debug.Log(origin.Replace("\"", "\\\"").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace(":", "\\:").Replace("-", "\\-"));
        return origin.Replace("\"", "\\\"").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace(":", "\\:").Replace("-", "\\-");
    }

    void UpdateBestResult()
    {
        float bestScoreFromServer = Managers.Data.GetBestRankFromServer("test2", PlayerPrefs.GetString("trans_SongTitle"));
        if (bestScoreFromServer == -2)
            return;
        if (bestScoreFromServer == -1)
        {
            Managers.Data.AddBestRankFromServer("test2", PlayerPrefs.GetString("trans_SongTitle"), _correctMount / (float)_totalAcc, ReturnStringForServer(JsonConvert.SerializeObject(Managers.Data.userReplayRecord)));
        }
        else if(bestScoreFromServer < _correctMount / (float)_totalAcc)
        {
            Managers.Data.SetBestRankFromServer("test2", PlayerPrefs.GetString("trans_SongTitle"), _correctMount / (float)_totalAcc, ReturnStringForServer(JsonConvert.SerializeObject(Managers.Data.userReplayRecord)));
        }
        else
        {
            Debug.Log("점수가 낮다!");
        }
    }

    IEnumerator UnityWebRequestPatchTest(string url, string json)
    {

        // PUT 방식
        UnityWebRequest www = UnityWebRequest.Put(url, json);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();  // 응답이 올때까지 대기한다.

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("error");
        }
    }
}
