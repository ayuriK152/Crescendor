/* 데이터 매니저
 * 작성 - 이원섭
 * 데이터의 입출력, 서버와의 통신과 관련된 기능을 수행하는 매니저
 * 유저의 로그인과 관련된 전반적인 기능 또한 맡아서 처리함.
 * 필요한 경우 서버 통신과 관련된 모든 기능을 다른 매니저에 구현하는 것도 생각해볼 수 있음 */

using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager
{
    public string jsonDataFromServer;
    public string userId = "";
    public bool isServerConnectionComplete = false;
    public bool isUserLoggedIn = false;
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
        Define.RankRecordList rankRecordList = new Define.RankRecordList();
        DirectoryInfo replayDirInfo = new DirectoryInfo($"{Application.dataPath}/RecordReplay");
        foreach (FileInfo file in replayDirInfo.GetFiles())
        {
            string songTitle = file.Name.Split("-")[0];
            if (songTitle == songFileName.Split("-")[0] && file.Name.Split(".")[file.Name.Split(".").Length - 1] == "json")
            {
                string dataStr = File.ReadAllText($"{Application.dataPath}/RecordReplay/{file.Name}");
                Define.UserReplayRecord userReplayRecord = JsonConvert.DeserializeObject<Define.UserReplayRecord>(dataStr);
                rankRecordList.records.Add(new Define.RankRecord(songTitle, file.Name.Split("-")[1], userReplayRecord.accuracy, file.Name.Split("-")[2], dataStr));
            }
        }

        return rankRecordList;
    }

    public void GetRankListFromServer(string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://15.164.2.49:3000/ranking/{songFileName}");

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
        UnityWebRequest www = UnityWebRequest.Get($"http://15.164.2.49:3000/record/getscore/{userId}/{songFileName}");

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
        UnityWebRequest www = new UnityWebRequest($"http://15.164.2.49:3000/record/setscore/{userId}/{songFileName}", "PUT");
        string jsonData = $"{{\"score\" : {score}, \"midi\" : {{\"tempo\" : {replayData.tempo}, \"noteRecords\" : \"[{JsonConvert.SerializeObject(replayData.noteRecords)}]\", \"originFileName\" : \"{replayData.originFileName}\"}}}}";
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
        UnityWebRequest www = new UnityWebRequest($"http://15.164.2.49:3000/record/addscore/{userId}/{songFileName}", "POST");
        string jsonData = $"{{\"score\" : {score}, \"midi\" : {{\"tempo\" : {replayData.tempo}, \"noteRecords\" : \"[{JsonConvert.SerializeObject(replayData.noteRecords)}]\", \"originFileName\" : \"{replayData.originFileName}\"}}}}";
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

    string ParseToServer(string origin)
    {
        Debug.Log(origin.Replace("\"", "\\\"").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace(":", "\\:").Replace("-", "\\-"));
        return origin.Replace("\"", "\\\"").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace(":", "\\:").Replace("-", "\\-");
    }
}
