using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Datas;

public class UI_MainMenu : UI_Scene
{
    TMP_InputField idInput;
    TMP_InputField passwordInput;
    Button signUpBtn;
    Button loginBtn;
    TextMeshProUGUI idText;
    Image profileImage;
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
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnOptionButtonClick);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClick);
        GetButton((int)Buttons.LoginBtn).gameObject.BindEvent(OnLoginButtonClick);
        GetButton((int)Buttons.SignUpBtn).gameObject.BindEvent(OnSignupButtonClick);
        GetButton((int)Buttons.ProfileBtn).gameObject.BindEvent(OnMyPageButtonClick);
        idInput = GameObject.Find("MainMenu/LoginStuff/ID").GetComponent<TMP_InputField>(); 
        passwordInput = GameObject.Find("MainMenu/LoginStuff/PASSWORD").GetComponent<TMP_InputField>();
        signUpBtn = GameObject.Find("MainMenu/LoginStuff/SignUpBtn").GetComponent<Button>();
        loginBtn = GameObject.Find("MainMenu/LoginStuff/LoginBtn").GetComponent<Button>();
        idText = GameObject.Find("MainMenu/NavBar/IDText").GetComponent<TextMeshProUGUI>();
        profileImage = GameObject.Find("MainMenu/NavBar/ProfileMask/ProfileBtn").GetComponent<Image>();
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
            LoadImage();
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
            StartCoroutine(Login(id, password));
        }
    }

    void OnOptionButtonClick(PointerEventData data)
    {
        Managers.ManagerInstance.AddComponent<BaseUIController>().ShowPopupUI<UI_Option>();
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

    IEnumerator SendLoginRequest(string url, string json, string method)
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
            Managers.Data.GetUserData(idInput.text);
            Managers.Data.GetUserLogData(idInput.text);
            LoginUpdateNavBar(); // NavBar 변경
            LoadImage(); // 프로필 변경
        }
        else
        {
            ShowErrorMsg("Login Failed");
            Debug.Log(www.error);
        }
    }

    IEnumerator Login(string id, string password)
    {
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";
        yield return StartCoroutine(SendLoginRequest($"http://{DEFAULT_SERVER_IP_PORT}/login", json, "POST"));
    }

    public void LoadImage()
    {
        StartCoroutine(Managers.Data.SetProfileImage(Managers.Data.userProfileURL, profileImage));
    }
}
