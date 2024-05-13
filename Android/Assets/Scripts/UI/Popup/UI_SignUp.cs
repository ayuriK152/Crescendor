using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_SignUp : UI_Popup
{
    TMP_InputField idInput;
    TMP_InputField passwordInput;
    private string baseURL = "http://15.164.2.49:3000/signup";
    enum Buttons
    {
        SignUpBtn,
        CloseBtn,
    }


    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(CloseBtnClicked);
        GetButton((int)Buttons.SignUpBtn).gameObject.BindEvent(SignUpBtnClicked);
        idInput = GameObject.Find("@UI/UI_SignUp/Panel/ID").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("@UI/UI_SignUp/Panel/PW").GetComponent<TMP_InputField>();
    }

    public void CloseBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }

    public void SignUpBtnClicked(PointerEventData data)
    {
        string id = idInput.text;
        string password = passwordInput.text;
        StartCoroutine(SignUpRequest(id, password));
    }

    IEnumerator SignUpRequest(string id, string password)
    {
        // 회원가입 요청 정보 로그 출력
        Debug.Log("Sign Up Request - ID: " + id + ", Password: " + password);

        // JSON 형식으로 데이터 가공
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";

        // 회원가입 요청 보내기 전에 JSON 데이터 로그 출력
        Debug.Log("Sending SignUp Request with JSON: " + json);

        // 회원가입 API 요청 보내기
        using (UnityWebRequest www = new UnityWebRequest(baseURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            // 요청 성공 여부 확인 후 처리
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sign Up Successful!");
                Debug.Log(www.downloadHandler.text);
                // 에러 메시지 생성
                GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_ErrorMsg");
                GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
                loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = "Sign up completed";
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Sign Up Failed: " + www.error);
                Debug.Log(www.downloadHandler.text);
                // 에러 메시지 생성
                GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_ErrorMsg");
                GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
                loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = "The ID that already exists";
            }
        }
    }
}
