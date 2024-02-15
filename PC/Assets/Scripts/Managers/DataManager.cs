using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager
{
    public string jsonDataFromServer;

    public void Init()
    {
        jsonDataFromServer = "init data";
    }

    public T Load<T>(string path) where T : Object
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

        GameObject go = Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

    public void GetRankListFromServer(string songFileName)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://15.164.2.49:3000/ranking/{songFileName}");

        www.SendWebRequest();  // 응답이 올때까지 대기한다.
        while (!www.isDone) { }

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            Debug.Log("Get Data Success");
            jsonDataFromServer = $"{{\"records\":{www.downloadHandler.text}}}";
        }
        else
        {
            Debug.LogError("Error to Get Data");
        }
    }
}
