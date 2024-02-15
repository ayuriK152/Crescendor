using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SignUpTest : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    private string baseURL = "http://15.164.2.49:3000/signup"; // ±âº» URL

    private void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpButtonClick);
    }

    IEnumerator SignUpRequest(string id, string password)
    {
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";

        UnityWebRequest www = UnityWebRequest.PostWwwForm(baseURL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SignUp Successful!");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("SignUp Failed: " + www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    void OnSignUpButtonClick()
    {
        string id = idInput.text;
        string password = passwordInput.text;
        StartCoroutine(SignUpRequest(id, password));
    }
}
