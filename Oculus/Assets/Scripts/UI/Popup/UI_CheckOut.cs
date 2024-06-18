using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_CheckOut : UI_Popup
{
    enum Buttons
    {
        YesBtn,
        NoBtn,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.YesBtn).gameObject.BindEvent(YesBtnClicked);
        GetButton((int)Buttons.NoBtn).gameObject.BindEvent(NoBtnClicked);
    }

    public void YesBtnClicked(PointerEventData data)
    {
        StartCoroutine(SignOutRequest(Managers.Data.userId));
    }

    public void NoBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }


    // È¸¿øÅ»Åð API
    IEnumerator SendSignOut(string url, string json, string method)
    {
        UnityWebRequest www;

        if (method == "POST")
        {
            www = UnityWebRequest.PostWwwForm(url, json);
        }
        else if (method == "PUT")
        {
            www = UnityWebRequest.Put(url, json);
        }
        else
        {
            Debug.LogError("Invalid HTTP method!");
            yield break;
        }

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Managers.Data.isUserLoggedIn = false;
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            Debug.Log(www.error);
        }
    }


    IEnumerator SignOutRequest(string id)
    {
        string baseURL = "http://15.164.2.49:3000/signout"; // ±âº» URL
        string json = "{\"id\":\"" + id + "\"}";
        yield return StartCoroutine(SendSignOut(baseURL, json, "POST"));
    }
}
