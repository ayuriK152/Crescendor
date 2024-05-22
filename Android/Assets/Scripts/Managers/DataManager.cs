/* ������ �Ŵ���
 * �ۼ� - �̿���
 * �������� �����, �������� ��Ű� ���õ� ����� �����ϴ� �Ŵ���
 * ������ �α��ΰ� ���õ� �������� ��� ���� �þƼ� ó����.
 * �ʿ��� ��� ���� ��Ű� ���õ� ��� ����� �ٸ� �Ŵ����� �����ϴ� �͵� �����غ� �� ���� */

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;
using static Datas;

public class DataManager
{
    public string jsonDataFromServer;
    public string userId = "";
    public int userCurriculumProgress = 0;
    public bool isServerConnectionComplete = false;
    public bool isUserLoggedIn = false;
    public Define.RankRecord rankRecord;
    public Define.UserReplayRecord userReplayRecord;

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
        if (!Directory.Exists($"{Application.persistentDataPath}/RecordReplay/"))
            Directory.CreateDirectory($"{Application.persistentDataPath}/RecordReplay/");
        Define.RankRecordList rankRecordList = new Define.RankRecordList();
        DirectoryInfo replayDirInfo = new DirectoryInfo($"{Application.persistentDataPath}/RecordReplay");
        foreach (FileInfo file in replayDirInfo.GetFiles())
        {
            string songTitle = file.Name.Split("-")[0];
            if (songTitle == songFileName.Split("-")[0] && file.Name.Split(".")[file.Name.Split(".").Length - 1] == "json")
            {
                string dataStr = File.ReadAllText($"{Application.persistentDataPath}/RecordReplay/{file.Name}");
                Define.UserReplayRecord userReplayRecord = JsonConvert.DeserializeObject<Define.UserReplayRecord>(dataStr);
                rankRecordList.records.Add(new Define.RankRecord(songTitle, file.Name.Split("-")[1], userReplayRecord.accuracy, file.Name.Split("-")[2], dataStr));
            }
        }

        return rankRecordList;
    }

    public void GetRankListFromServer(string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{DEFAULT_SERVER_IP_PORT}/ranking/{songFileName}");

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
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

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
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

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
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

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
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

    // ��ȯ�Ǵ� �� ���� ������ ������ ������ �Ǻ�
    public bool GetUserData(string userId)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{DEFAULT_SERVER_IP_PORT}/getuser/{userId}");

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
        {
            isServerConnectionComplete = true;
            Debug.Log("Get Data Success");
            userCurriculumProgress = GetUserCurriculumProgress(www.downloadHandler.text);
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
        int startIndex = userData.IndexOf("\"curriculum\":") + "\"curriculum\":".Length + 1;
        int endIndex = userData.IndexOf("}", startIndex + 1);
        return int.Parse(userData.Substring(startIndex, endIndex - startIndex));
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

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
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
}