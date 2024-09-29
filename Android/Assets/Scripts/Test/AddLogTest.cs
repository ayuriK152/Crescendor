using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AddLogTest : MonoBehaviour
{
    private string baseURL = "http://15.164.2.49:3000/log/addlog/";

    public string userID;

    private void Start()
    {
        StartCoroutine(AddLog(userID));
    }

    private IEnumerator AddLog(string userID)
    {
        string url = baseURL + userID;

        // 현재 시간사용
        string logData = "{\"date\":\"" + System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\"}";

        // POST 요청
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(logData);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            // 요청 결과 확인
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Log added successfully!");
            }
            else
            {
                Debug.LogError("Error adding log: " + www.error);
            }
        }
    }
}
