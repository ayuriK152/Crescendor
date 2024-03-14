using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    Button signUpBtn;
    Button loginBtn;
    TextMeshProUGUI idText;
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
        signUpBtn = GameObject.Find("MainMenu/NavBar/SignUpBtn").GetComponent<Button>();
        loginBtn = GameObject.Find("MainMenu/NavBar/LoginBtn").GetComponent<Button>();
        idText = GameObject.Find("MainMenu/NavBar/IDText").GetComponent<TextMeshProUGUI>();
    }

    public void LoginUpdateNavBar() // 로그인 상태 시 NavBar 수정
    {
        idText.text = idInput.text;
        idInput.text = null;
        passwordInput.text = null;
        idText.gameObject.SetActive(true);
        idInput.gameObject.SetActive(false);
        passwordInput.gameObject.SetActive(false);
        signUpBtn.gameObject.SetActive(false);
        loginBtn.GetComponentInChildren<TextMeshProUGUI>().text = "LogOut";       
    }


    public void OnPlayButtonClick(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
    public void OnLoginButtonClick(PointerEventData data)
    {
        if (isLogin) // 로그아웃 버튼 클릭 시 
        {
            idInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            signUpBtn.gameObject.SetActive(true);
            idText.gameObject.SetActive(false); 
            loginBtn.GetComponentInChildren<TextMeshProUGUI>().text = "LogIn";
            isLogin = false;
            // 에러메시지 표시
            ShowErrorMsg("LogOut Successs");
        }
        else // 로그인 버튼 클릭 시 
        {
            string id = idInput.text;
            string password = passwordInput.text;
            StartCoroutine(LoginRequest(id, password));
        }
    }

    public void OnSignupButtonClick(PointerEventData data)
    {
        // 회원가입 팝업창 생성
        Managers.ManagerInstance.AddComponent<OutGameUIController>().ShowPopupUI<UI_SignUp>();
    }
    public void OnMyPageButtonClick(PointerEventData data)
    {
        if(isLogin)
        {
            SceneManager.LoadScene("MyPageScene");
        }
        else
        {
            ShowErrorMsg("Please log in");
        }
    }

    public void ShowErrorMsg(string msg) // 에러 팝업창 생성
    {
        GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_ErrorMsg");
        GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
        loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;
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
            isLogin = true;
            ShowErrorMsg("Login Success");
            LoginUpdateNavBar(); // NavBar 변경
        }
        else
        {
            ShowErrorMsg("Login Failed");
        }
    }

    IEnumerator LoginRequest(string id, string password)
    {
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";
        yield return StartCoroutine(SendRequest(baseURL, json, "POST"));
    }
}
