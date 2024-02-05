using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResultController : MonoBehaviour
{
    string _songTitle;
    int _songLength;

    int _totalAcc;
    int _failMount;
    int _correctMount;
    int _outlinerMount;

    ResultUIController _uiController;

    public void Init()
    {
        _songTitle = PlayerPrefs.GetString("trans_SongTitle");
        _songLength = Managers.Midi.songLength;

        _totalAcc = Managers.Midi.totalDeltaTime;
        _failMount = PlayerPrefs.GetInt("trans_FailMount");
        _correctMount = _totalAcc - _failMount;
        _outlinerMount = PlayerPrefs.GetInt("trans_OutlinerMount");

        _uiController = Managers.UI.currentUIController as ResultUIController;
        _uiController.BindIngameUI();
        _uiController.songTitleTMP.text = $"{_songTitle}";
        _uiController.correctMountTMP.text = $"{_correctMount}";
        _uiController.failMountTMP.text = $"{_failMount}";
        _uiController.outlinerMountTMP.text = $"{_outlinerMount}";
        _uiController.correctGraphImage.fillAmount = _correctMount / (float)(_totalAcc + _outlinerMount);
        _uiController.failGraphImage.fillAmount = _failMount / (float)(_failMount + _outlinerMount);
        _uiController.accuracyTMP.text = $"{Convert.ToInt32((_correctMount / (float)_totalAcc) * 10000.0f) / 100.0f}%";
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
