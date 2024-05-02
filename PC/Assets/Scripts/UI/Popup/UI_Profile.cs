using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_Profile : UI_Popup
{
    Image profileImage;
    TMP_InputField imageUrl;
    enum Buttons
    {
        Btn,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Btn).gameObject.BindEvent(OnBtnClicked);
        profileImage = GameObject.Find("ProfileBtn").GetComponent<Image>();
        imageUrl = GameObject.Find("@UI/UI_Profile/Panel/URL").GetComponent<TMP_InputField>();
    }

    public void OnBtnClicked(PointerEventData data)
    {
        LoadImageFromURL();
    }

    // ����ڰ� �̹����� �����Ͽ� ������ �̹����� �����ϴ� �Լ�
    public void LoadImageFromURL()
    {
        // �̹��� URL�� �����ͼ� �̹��� �ε�
        string URL = imageUrl.text;
        StartCoroutine(LoadImageFromURLCoroutine(URL));
    }

    // �̹��� URL�κ��� �̹����� �ٿ�ε��Ͽ� ������ �̹����� �����ϴ� �ڷ�ƾ �Լ�
    private IEnumerator LoadImageFromURLCoroutine(string imageURL)
    {
        // �̹��� �ٿ�ε�
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
        yield return request.SendWebRequest();

        // �̹��� �ٿ�ε忡 ������ ���
        if (!request.isNetworkError && !request.isHttpError)
        {
            // �ؽ��� ����
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Sprite ����
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // ������ �̹����� ����
            profileImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to load image from URL: " + request.error);
        }
    }
}
