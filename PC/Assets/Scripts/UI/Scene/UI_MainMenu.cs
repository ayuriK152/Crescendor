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
    Image profileImage;
    private string baseURL = "http://15.164.2.49:3000/login"; // 기본 URL
    Sprite originalSprite;

    enum Buttons
    {
        PlayButton,
        OptionButton,
        ExitButton,
        LoginBtn,
        SignUpBtn,
        ProfileBtn,
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
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClick);
        GetButton((int)Buttons.LoginBtn).gameObject.BindEvent(OnLoginButtonClick);
        GetButton((int)Buttons.SignUpBtn).gameObject.BindEvent(OnSignupButtonClick);
        GetButton((int)Buttons.ProfileBtn).gameObject.BindEvent(OnMyPageButtonClick);
        idInput = GameObject.Find("MainMenu/LoginStuff/ID").GetComponent<TMP_InputField>(); 
        passwordInput = GameObject.Find("MainMenu/LoginStuff/PASSWORD").GetComponent<TMP_InputField>();
        signUpBtn = GameObject.Find("MainMenu/LoginStuff/SignUpBtn").GetComponent<Button>();
        loginBtn = GameObject.Find("MainMenu/LoginStuff/LoginBtn").GetComponent<Button>();
        idText = GameObject.Find("MainMenu/NavBar/IDText").GetComponent<TextMeshProUGUI>();
        profileImage = GameObject.Find("MainMenu/NavBar/ProfileBtn").GetComponent<Image>();
        originalSprite = profileImage.sprite;
        if (!Managers.Data.isUserLoggedIn)
        {
            idInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            signUpBtn.gameObject.SetActive(true);
            idText.gameObject.SetActive(false);
            Managers.Data.userId = "";
            loginBtn.GetComponentInChildren<TextMeshProUGUI>().text = "LogIn";
            Managers.Data.isUserLoggedIn = false;
        }
        else
        {
            idText.text = Managers.Data.userId;
            idInput.text = null;
            passwordInput.text = null;
            idText.gameObject.SetActive(true);
            idInput.gameObject.SetActive(false);
            passwordInput.gameObject.SetActive(false);
            signUpBtn.gameObject.SetActive(false);
            loginBtn.GetComponentInChildren<TextMeshProUGUI>().text = "LogOut";
            LoadImage(Managers.Data.userId);
        }
    }

    public void LoginUpdateNavBar() // 로그인 상태 시 NavBar 수정
    {
        Managers.Data.userId = idInput.text;
        idText.text = Managers.Data.userId;
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

    void OnExitButtonClick(PointerEventData data)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnLoginButtonClick(PointerEventData data)
    {
        if (Managers.Data.isUserLoggedIn) // 로그아웃 버튼 클릭 시 
        {
            idInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            signUpBtn.gameObject.SetActive(true);
            idText.gameObject.SetActive(false);
            Managers.Data.userId = "";
            loginBtn.GetComponentInChildren<TextMeshProUGUI>().text = "LogIn";
            Managers.Data.isUserLoggedIn = false;
            // 에러메시지 표시
            ShowErrorMsg("LogOut Successs");
            profileImage.sprite = originalSprite;
        }
        else // 로그인 버튼 클릭 시 
        {
            string id = idInput.text;
            string password = passwordInput.text;
            // ID 또는 비밀번호가 null인지 확인
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                ShowErrorMsg("Please enter your id");
                return; // 전송을 중지
            }

            // ID와 비밀번호가 유효한 경우에만 요청을 보냄.
            StartCoroutine(LoginRequest(id, password));
        }
    }

    public void OnSignupButtonClick(PointerEventData data)
    {
        // 회원가입 팝업창 생성
        Managers.ManagerInstance.AddComponent<BaseUIController>().ShowPopupUI<UI_SignUp>();
    }

    public void OnMyPageButtonClick(PointerEventData data)
    {
        if(Managers.Data.isUserLoggedIn)
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
            Managers.Data.isUserLoggedIn = true;
            ShowErrorMsg("Login Success");
            LoginUpdateNavBar(); // NavBar 변경
            LoadImage(Managers.Data.userId); // 프로필 변경
        }
        else
        {
            ShowErrorMsg("Login Failed");
            Debug.Log(www.error);
        }
    }

    IEnumerator LoginRequest(string id, string password)
    {
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";
        yield return StartCoroutine(SendRequest(baseURL, json, "POST"));
    }

    #region Image Settings
    public void LoadImage(string userId)
    {
        StartCoroutine(GetUserProfile(userId));
    }


    private IEnumerator GetUserProfile(string userID)
    {

        string url = "http://15.164.2.49:3000/getuser/" + userID;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                // 응답된 JSON 문자열에서 원하는 정보 추출
                string profileURL = GetProfileURL(responseText);
                // 프로필 이미지 다운로드 및 설정
                yield return StartCoroutine(SetProfileImage(profileURL));
            }
            else
            {
                Debug.LogError("유저를 찾을 수 없음" + request.error);
            }
        }
    }

    private IEnumerator SetProfileImage(string imageURL)
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


    private string GetProfileURL(string json)
    {
        // JSON 문자열을 파싱하여 profile 정보 추출
        string profileURL = "";

        // JSON 문자열을 파싱하고 원하는 정보를 추출하는 로직을 작성
        // 여기서는 간단하게 문자열을 찾아내는 방식으로 작성하였습니다.
        int startIndex = json.IndexOf("\"profile\":") + "\"profile\":".Length + 1;
        int endIndex = json.IndexOf("\"", startIndex + 1);
        profileURL = json.Substring(startIndex, endIndex - startIndex);

        return profileURL;
    }

    #endregion Image Settings 
}
