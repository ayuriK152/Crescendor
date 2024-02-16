using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class SignUpTest : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    private string baseURL = "http://15.164.2.49:3000/signin";

    private void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpButtonClick);
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
            }
            else
            {
                Debug.LogError("Sign Up Failed: " + www.error);
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    void OnSignUpButtonClick()
    {
        string id = idInput.text;
        string password = passwordInput.text;
        StartCoroutine(SignUpRequest(id, password));
    }
}
