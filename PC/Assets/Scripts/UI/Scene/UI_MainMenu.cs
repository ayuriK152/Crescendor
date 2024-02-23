using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : UI_Scene
{
    TMP_InputField idInput;
    TMP_InputField passwordInput;
    bool isLogin = false; // 로그인 유무
    private string baseURL = "http://15.164.2.49:3000/login"; // 기본 URL
    
    enum Buttons
    {
        PlayButton,
        OptionButton,
        ExitButton,
        LoginBtn,
        SignUpBtn,
        MypageBtn,
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.PlayButton).gameObject.BindEvent(OnPlayButtonClick);
        GetButton((int)Buttons.LoginBtn).gameObject.BindEvent(OnLoginButtonClick);
        GetButton((int)Buttons.SignUpBtn).gameObject.BindEvent(OnSignupButtonClick);
        GetButton((int)Buttons.MypageBtn).gameObject.BindEvent(OnMyPageButtonClick);
        idInput = GameObject.Find("MainMenu/NavBar/ID").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("MainMenu/NavBar/PASSWORD").GetComponent<TMP_InputField>();
    }
      
    public void OnPlayButtonClick(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
    public void OnLoginButtonClick(PointerEventData data)
    {
        Debug.Log("로그인 버튼 클릭");
        string id = idInput.text;
        string password = passwordInput.text;
        StartCoroutine(LoginRequest(id, password));
    }

    public void OnSignupButtonClick(PointerEventData data)
    {
        Debug.Log("회원가입 버튼 클릭");
        // 회원가입 팝업창 생성
        Managers.ManagerInstance.AddComponent<SongSelectUIController>().ShowPopupUI<UI_SignUp>();
    }
    public void OnMyPageButtonClick(PointerEventData data)
    {
        if(isLogin)
        {
            SceneManager.LoadScene("MyPageScene");
        }
        else
        {
            Debug.Log("로그인을 해주세요");
        }
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
            Debug.Log("로그인 성공!");
            Debug.Log(www.downloadHandler.text);
            isLogin = true;
        }
        else
        {
            Debug.Log("로그인 실패!");
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator LoginRequest(string id, string password)
    {
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";
        yield return StartCoroutine(SendRequest(baseURL, json, "POST"));
    }
}
