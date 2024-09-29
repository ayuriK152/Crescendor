using System.Collections;
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
    Image pianoConnectionCheckImg;

    public GameObject loginSuccessPopup;

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

        idInput = GameObject.Find("MainMenu/BackGround/LoginStuff/ID").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("MainMenu/BackGround/LoginStuff/PASSWORD").GetComponent<TMP_InputField>();
        signUpBtn = GameObject.Find("MainMenu/BackGround/LoginStuff/SignUpBtn").GetComponent<Button>();
        loginBtn = GameObject.Find("MainMenu/BackGround/LoginStuff/LoginBtn").GetComponent<Button>();
        idText = GameObject.Find("MainMenu/NavBar/IDText").GetComponent<TextMeshProUGUI>();
        profileImage = GameObject.Find("MainMenu/NavBar/ProfileMask/ProfileBtn").GetComponent<Image>();
        pianoConnectionCheckImg = GameObject.Find("MainMenu/NavBar/PianoConnectionCheck").GetComponent<Image>();
        if (Managers.Input.isPianoConnected)
        {
            pianoConnectionCheckImg.color = new Color(1, 1, 1);
        }
        else
        {
            pianoConnectionCheckImg.color = new Color(0.6f, 0.6f, 0.6f);
        }
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

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
        Managers.Input.pianoConnectionAction -= PianoConnectionUpdate;
        Managers.Input.pianoConnectionAction += PianoConnectionUpdate;
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
        if (Managers.Data.isUserLoggedIn)
        {
            SceneManager.LoadScene("MyPageScene");
        }
        else
        {
            ShowErrorMsg("Please log in");
        }
    }

    public void ShowErrorMsg(string msg) // 俊矾 扑诀芒 积己
    {
        GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_ErrorMsg");
        GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
        loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        bool isShowed = false;
        // GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/XR_Popup/UI_ErrorMsg");
        // GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
        if (!isShowed)
        {
            loginSuccessPopup.SetActive(true);
            loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        else
        {
            loginSuccessPopup.SetActive(false);
        }
        isShowed = !isShowed;
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

    void PianoConnectionUpdate(bool isConnected)
    {
        if (isConnected)
        {
            pianoConnectionCheckImg.color = new Color(1, 1, 1);
        }
        else
        {
            pianoConnectionCheckImg.color = new Color(0.6f, 0.6f, 0.6f);
        }
    }

    void InputKeyEvent(KeyCode keyCode, Define.InputType inputType)
    {
        switch (inputType)
        {
            case Define.InputType.OnKeyDown:
                switch (keyCode)
                {
                    case KeyCode.Escape:
                        Application.Quit();
                        break;
                }
                break;
            case Define.InputType.OnKeyUp:
                break;
        }
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

                // ?묐떟??JSON 臾몄옄?댁뿉???먰븯???뺣낫 異붿텧
                string profileURL = GetProfileURL(responseText);
                // ?꾨줈???대?吏 ?ㅼ슫濡쒕뱶 諛??ㅼ젙
                yield return StartCoroutine(SetProfileImage(profileURL));
            }
            else
            {
                Debug.LogError("?좎?瑜?李얠쓣 ???놁쓬" + request.error);
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
                // ?띿뒪泥??ㅼ슫濡쒕뱶 諛??대?吏 UI???ㅼ젙
                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }


    private string GetProfileURL(string json)
    {
        // JSON 臾몄옄?댁쓣 ?뚯떛?섏뿬 profile ?뺣낫 異붿텧
        string profileURL = "";

        // JSON 臾몄옄?댁쓣 ?뚯떛?섍퀬 ?먰븯???뺣낫瑜?異붿텧?섎뒗 濡쒖쭅???묒꽦
        // ?ш린?쒕뒗 媛꾨떒?섍쾶 臾몄옄?댁쓣 李얠븘?대뒗 諛⑹떇?쇰줈 ?묒꽦?섏??듬땲??
        int startIndex = json.IndexOf("\"profile\":") + "\"profile\":".Length + 1;
        int endIndex = json.IndexOf("\"", startIndex + 1);
        profileURL = json.Substring(startIndex, endIndex - startIndex);

        return profileURL;
    }

    #endregion Image Settings 
}
