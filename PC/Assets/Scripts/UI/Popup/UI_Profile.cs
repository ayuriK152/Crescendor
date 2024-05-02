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

    // 사용자가 이미지를 선택하여 프로필 이미지로 설정하는 함수
    public void LoadImageFromURL()
    {
        // 이미지 URL을 가져와서 이미지 로드
        string URL = imageUrl.text;
        StartCoroutine(LoadImageFromURLCoroutine(URL));
    }

    // 이미지 URL로부터 이미지를 다운로드하여 프로필 이미지로 설정하는 코루틴 함수
    private IEnumerator LoadImageFromURLCoroutine(string imageURL)
    {
        // 이미지 다운로드
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
        yield return request.SendWebRequest();

        // 이미지 다운로드에 성공한 경우
        if (!request.isNetworkError && !request.isHttpError)
        {
            // 텍스쳐 생성
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Sprite 생성
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // 프로필 이미지로 설정
            profileImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to load image from URL: " + request.error);
        }
    }
}
