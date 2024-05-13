using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Secession : UI_Popup
{

    TMP_InputField passwordinput;
    bool passwordMatch;
    enum Buttons
    {
        CheckBtn,
        CloseBtn,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CheckBtn).gameObject.BindEvent(OnCheckBtnClicked);
        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(OnCloseBtnClicked);
        passwordinput = GameObject.Find("@UI/UI_Secession/Panel/PW").GetComponent<TMP_InputField>();
    }

    public void OnCheckBtnClicked(PointerEventData data)
    {
        string password = passwordinput.text;
        // ID �Ǵ� ��й�ȣ�� null���� Ȯ��
        if (string.IsNullOrEmpty(password))
        {
            return; // ������ ����
        }

        // ��й�ȣ �˻�
        StartCoroutine(CheckIDRequest(Managers.Data.userId, password));

    }

    public void OnCloseBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }

    // ��й�ȣ Ȯ�� 
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
            Managers.ManagerInstance.AddComponent<OutGameUIController>().ShowPopupUI<UI_CheckOut>();
        }
        else
        {
            Debug.Log(www.error);
            ShowErrorMsg("Passwords do not match");
        }
    }



    public void ShowErrorMsg(string msg) // ���� �˾�â ����
    {
        GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_ErrorMsg");
        GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
        loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;
    }


    IEnumerator CheckIDRequest(string id, string password)
    {
        string baseURL = "http://15.164.2.49:3000/login"; // �⺻ URL
        string json = "{\"id\":\"" + id + "\", \"password\":\"" + password + "\"}";
        yield return StartCoroutine(SendRequest(baseURL, json, "POST"));
    }





}
