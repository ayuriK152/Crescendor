/* 데이터 매니저
 * 작성 - 이원섭
 * 데이터의 입출력, 서버와의 통신과 관련된 기능을 수행하는 매니저
 * 유저의 로그인과 관련된 전반적인 기능 또한 맡아서 처리함.
 * 필요한 경우 서버 통신과 관련된 모든 기능을 다른 매니저에 구현하는 것도 생각해볼 수 있음 */

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Datas;
using static Define;

public class DataManager
{
    public string jsonDataFromServer;
    public string userId = "";
    public int userCurriculumProgress = 0;
    public string userProfileURL = "";
    public int[] logCounts = new int[12 * 4];
    public bool isServerConnectionComplete = false;
    public bool isUserLoggedIn = false;
    public Define.RankRecord rankRecord;
    public Define.UserReplayRecord userReplayRecord;
    public List<DateTime> userLogDates = new List<DateTime>();

    public void Init()
    {
        jsonDataFromServer = "init data";
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go = UnityEngine.Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        UnityEngine.Object.Destroy(go);
    }

    public Define.RankRecordList GetRankListFromLocal(string songFileName)
    {
        if (!System.IO.Directory.Exists($"{Application.dataPath}/RecordReplay/"))
            System.IO.Directory.CreateDirectory($"{Application.dataPath}/RecordReplay/");
        Define.RankRecordList rankRecordList = new Define.RankRecordList();
        DirectoryInfo replayDirInfo = new DirectoryInfo($"{Application.dataPath}/RecordReplay");
        foreach (FileInfo file in replayDirInfo.GetFiles())
        {
            string songTitle = file.Name.Split("-")[0];
            if (songTitle == songFileName.Split("-")[0] && file.Name.Split(".")[file.Name.Split(".").Length - 1] == "json")
            {
                string dataStr = System.IO.File.ReadAllText($"{Application.dataPath}/RecordReplay/{file.Name}");
                Define.UserReplayRecord userReplayRecord = JsonConvert.DeserializeObject<Define.UserReplayRecord>(dataStr);
                rankRecordList.records.Add(new Define.RankRecord(songTitle, file.Name.Split("-")[1], userReplayRecord.accuracy, file.Name.Split("-")[2], dataStr));
            }
        }

        return rankRecordList;
    }

    public void GetRankListFromServer(string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{DEFAULT_SERVER_IP_PORT}/ranking/{songFileName}");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Get Data Success");
            jsonDataFromServer = $"{{\"records\":{www.downloadHandler.text}}}";
        }
        else
        {
            isServerConnectionComplete = false;
            Debug.LogError("Error to Get Data");
        }
    }

    public float GetBestRankFromServer(string userId, string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{DEFAULT_SERVER_IP_PORT}/record/getscore/{userId}/{songFileName}");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Get Data Success");
            
            if (www.downloadHandler.text.Length == 2)
            {
                return -1;
            }
            else
            {
                Debug.Log(www.downloadHandler.text.Substring(10, www.downloadHandler.text.Length - 12));
                return float.Parse(www.downloadHandler.text.Substring(10, www.downloadHandler.text.Length - 12));
            }
        }
        else
        {
            isServerConnectionComplete = false;
            Debug.LogError("Error to Get Data");
            return -2;
        }
    }

    public void SetBestRankToServer(string userId, string songFileName, float score, Define.UserReplayRecord replayData)
    {
        UnityWebRequest www = new UnityWebRequest($"http://{DEFAULT_SERVER_IP_PORT}/record/setscore/{userId}/{songFileName}", "PUT");
        string jsonData = $"{{\"score\" : {score}, \"midi\" : {{\"tempo\" : {replayData.tempo}, \"noteRecords\" : \"[{ParseToServer(JsonConvert.SerializeObject(replayData.noteRecords))}]\", \"originFileName\" : \"{replayData.originFileName}\"}}}}";
        Debug.Log(jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Set Data Success");
        }
        else
        {
            isServerConnectionComplete = false;
            Debug.LogError("Error to Set Data");
        }
    }

    public void AddBestRankToServer(string userId, string songFileName, float score, Define.UserReplayRecord replayData)
    {
        UnityWebRequest www = new UnityWebRequest($"http://{DEFAULT_SERVER_IP_PORT}/record/addscore/{userId}/{songFileName}", "POST");
        string jsonData = $"{{\"score\" : {score}, \"midi\" : {{\"tempo\" : {replayData.tempo}, \"noteRecords\" : \"[{ParseToServer(JsonConvert.SerializeObject(replayData.noteRecords))}]\", \"originFileName\" : \"{replayData.originFileName}\"}}}}";
        Debug.Log(jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Add Data Success");
        }
        else
        {
            isServerConnectionComplete = false;
            Debug.LogError(www.error);
        }
    }

    // 반환되는 참 거짓 값으로 에러의 유무를 판별
    public bool GetUserData(string userId)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{DEFAULT_SERVER_IP_PORT}/getuser/{userId}");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Get Data Success");
            userCurriculumProgress = GetUserCurriculumProgress(www.downloadHandler.text);
            userProfileURL = GetProfileURL(www.downloadHandler.text);
            Managers.Data.GetUserLogData(Managers.Data.userId);
            return true;
        }
        else
        {
            isServerConnectionComplete = false;
            Debug.LogError("Error to Get Data");
            return false;
        }
    }

    public int GetUserCurriculumProgress(string userData)
    {
        int startIndex = userData.IndexOf("\"curriculum\":") + "\"curriculum\":".Length;
        int endIndex = userData.IndexOf("}", startIndex);
        return int.Parse(userData.Substring(startIndex, endIndex - startIndex));
    }

    private string GetProfileURL(string json)
    {
        // JSON 문자열을 파싱하여 profile 정보 추출
        string profileURL = "";

        int startIndex = json.IndexOf("\"profile\":") + "\"profile\":".Length + 1;
        int endIndex = json.IndexOf("\"", startIndex + 1);
        profileURL = json.Substring(startIndex, endIndex - startIndex);

        return profileURL;
    }

    public bool GetUserLogData(string userId)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{DEFAULT_SERVER_IP_PORT}/log/getlog/{userId}");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Get Log Success");
            Debug.Log($"Log Data: {www.downloadHandler.text}");
            ParseLogDates(www.downloadHandler.text);
            return true;
        }
        else
        {
            isServerConnectionComplete = false;
            return false;
        }
    }

    private void ParseLogDates(string logData)
    {
        userLogDates.Clear();

        try
        {
            // JSON 데이터 파싱
            var logs = JsonUtility.FromJson<LogDateWrapper>($"{{\"logs\":{logData}}}");

            foreach (var log in logs.logs)
            {
                // 시간대 정보를 제거하고 날짜만 유지
                if (DateTime.TryParse(log.date.Substring(0, 10), out DateTime logDate))
                {
                    userLogDates.Add(logDate);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse log data: {e.Message}");
        }
    }


    public IEnumerator SetProfileImage(string imageURL, Image profileImage)
    {
        using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                // 텍스처 다운로드 및 이미지 UI에 설정
                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }

    public void SetUserCurriculumProgress(int value)
    {
        UnityWebRequest www = new UnityWebRequest($"http://{DEFAULT_SERVER_IP_PORT}/setcurriculum", "POST");
        string jsonData = $"{{\"id\" : \"{userId}\", \"curriculum\" : {value}}}";
        Debug.Log(jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            isServerConnectionComplete = true;
            Debug.Log("Set Data Success");
        }
        else
        {
            isServerConnectionComplete = false;
            Debug.LogError($"Error to Set Data - {www.error}");
        }
    }

    string ParseToServer(string origin)
    {
        Debug.Log(origin.Replace("\"", "\\\""));
        return origin.Replace("\"", "\\\"");
    }

    public void AddLog(string userID)
    {
        string baseURL = "http://15.164.2.49:3000/log/addlog/";
        string url = baseURL + userID;

        string logData = "{\"date\":\"" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\"}";

        UnityWebRequest www = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(logData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        www.SetRequestHeader("Content-Type", "application/json");

        www.SendWebRequest().completed += (AsyncOperation op) =>
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Log added successfully!");
            }
            else
            {
                Debug.LogError("Error adding log: " + www.error);
            }
        };
    }

}
