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
    private string baseURL = "http://15.164.2.49:3000/login"; // �⺻ URL
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

    public void LoginUpdateNavBar() // �α��� ���� �� NavBar ����
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
        if (Managers.Data.isUserLoggedIn) // �α׾ƿ� ��ư Ŭ�� ��
        {
            idInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            signUpBtn.gameObject.SetActive(true);
            idText.gameObject.SetActive(false);
            Managers.Data.userId = "";
            loginBtn.GetComponentInChildren<TextMeshProUGUI>().text = "LogIn";
            Managers.Data.isUserLoggedIn = false;
            // �����޽��� ǥ��
            ShowErrorMsg("LogOut Successs");
            profileImage.sprite = originalSprite;
        }
        else // �α��� ��ư Ŭ�� ��
        {
            string id = idInput.text;
            string password = passwordInput.text;
            // ID �Ǵ� ��й�ȣ�� null���� Ȯ��
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                ShowErrorMsg("Please enter your id");
                return; // ������ ����
            }

            // ID�� ��й�ȣ�� ��ȿ�� ��쿡�� ��û�� ����.
            StartCoroutine(LoginRequest(id, password));
        }
    }

    void OnOptionButtonClick(PointerEventData data)
    {
        Managers.ManagerInstance.AddComponent<BaseUIController>().ShowPopupUI<UI_Option>();
    }

    public void OnSignupButtonClick(PointerEventData data)
    {
        // ȸ������ �˾�â ����
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

    public void ShowErrorMsg(string msg) // ���� �˾�â ����
    {
        bool isShowed = false;
        // GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/XR_Popup/UI_ErrorMsg");
        // GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
        if(!isShowed)
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
            LoginUpdateNavBar(); // NavBar ����
            LoadImage(Managers.Data.userId); // ������ ����
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

                // ?�답??JSON 문자?�에???�하???�보 추출
                string profileURL = GetProfileURL(responseText);
                // ?�로???��?지 ?�운로드 �??�정
                yield return StartCoroutine(SetProfileImage(profileURL));
            }
            else
            {
                Debug.LogError("?��?�?찾을 ???�음" + request.error);
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
                // ?�스�??�운로드 �??��?지 UI???�정
                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }


    private string GetProfileURL(string json)
    {
        // JSON 문자?�을 ?�싱?�여 profile ?�보 추출
        string profileURL = "";

        // JSON 문자?�을 ?�싱?�고 ?�하???�보�?추출?�는 로직???�성
        // ?�기?�는 간단?�게 문자?�을 찾아?�는 방식?�로 ?�성?��??�니??
        int startIndex = json.IndexOf("\"profile\":") + "\"profile\":".Length + 1;
        int endIndex = json.IndexOf("\"", startIndex + 1);
        profileURL = json.Substring(startIndex, endIndex - startIndex);

        return profileURL;
    }

    #endregion Image Settings 
}
