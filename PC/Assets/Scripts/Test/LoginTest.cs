using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LoginTest : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button signUpButton;
    public GameObject signUpPanel;

    private string baseURL = "http://15.164.2.49:3000/login"; // 기본 URL

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        signUpButton.onClick.AddListener(OnSignupButtonClick);
    }

    IEnumerator SendRequest(string url, string json, string method)
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
            Debug.Log("Request Successful!");
            Debug.Log(www.downloadHandler.text);
            MoveToNextScene();
        }
        else
        {
            Debug.Log("Request Failed!");
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator LoginRequest(string id, string password)
    {
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";
        yield return StartCoroutine(SendRequest(baseURL, json, "POST"));
    }

    void OnLoginButtonClick()
    {
        string id = idInput.text;
        string password = passwordInput.text;
        StartCoroutine(LoginRequest(id, password));
    }

    void OnSignupButtonClick()
    {
        signUpPanel.SetActive(true);
    }

    void MoveToNextScene()
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }


}
