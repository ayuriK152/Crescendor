using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager
{
    public string jsonDataFromServer;
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

    public void GetRankListFromServer(string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://15.164.2.49:3000/ranking/{songFileName}");

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
        {
            Debug.Log("Get Data Success");
            jsonDataFromServer = $"{{\"records\":{www.downloadHandler.text}}}";
        }
        else
        {
            Debug.LogError("Error to Get Data");
        }
    }

    public float GetBestRankFromServer(string userId, string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://15.164.2.49:3000/record/getscore/{userId}/{songFileName}");

        www.SendWebRequest();  // ������ �ö����� ����Ѵ�.
        while (!www.isDone) { }

        if (www.error == null)  // ������ ���� ������ ����.
        {
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
            Debug.LogError("Error to Get Data");
            return -2;
        }
    }
}
