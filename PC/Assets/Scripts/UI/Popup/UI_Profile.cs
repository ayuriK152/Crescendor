using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UI_Profile : UI_Popup
{
    private TMP_InputField imageUrlInput;
    UI_MyPage myPage;
    Image profileImage;

    private string imageUrlPattern = @"^(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png)";

    enum Buttons
    {
        Btn,
        CloseBtn,
    }
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Btn).gameObject.BindEvent(OnBtnClicked);
        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(OnCloseBtnClicked);
        // UI 요소 초기화
        imageUrlInput = GameObject.Find("@UI/UI_Profile/Panel/URL").GetComponent<TMP_InputField>();
        // ProfileBtn을 찾기 위해 UI_MyPage를 가져옴
        myPage = FindObjectOfType<UI_MyPage>();
        if (myPage != null)
        {
            // UserInfo에서 ProfileBtn을 찾아서 profileImage에 할당
            Transform profileBtn = myPage.transform.Find("UserInfo/ProfileBtn");
            if (profileBtn != null)
            {
                profileImage = profileBtn.GetComponent<Image>();
            }
        }
    }

    // 이미지 URL이 유효한지 확인하는 함수
    private bool IsValidImageUrl(string imageUrl)
    {
        // 정규식 패턴을 사용하여 이미지 URL을 검사
        return Regex.IsMatch(imageUrl, imageUrlPattern);
    }

    public void ShowErrorMsg(string msg) // 에러 팝업창 생성
    {
        GameObject loginSuccessPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_ErrorMsg");
        GameObject loginSuccessPopup = Instantiate(loginSuccessPrefab, transform.parent);
        loginSuccessPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;
    }


    // 사용자가 이미지를 선택하여 프로필 이미지로 설정하는 함수
    public void UpdateProfile()
    {
        // 이미지 URL을 가져옴
        string imageURL = imageUrlInput.text;

        // 이미지 URL이 유효한지 확인
        if (IsValidImageUrl(imageURL))
        {
            // 서버에 프로필 업데이트 요청을 보냄
            StartCoroutine(UpdateProfileCoroutine(imageURL));
        }
        else
        {
            ShowErrorMsg("Image url is invalid.");
        }
    }

    public void OnBtnClicked(PointerEventData data)
    {
        UpdateProfile();
    }

    public void OnCloseBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }

    // 서버에 프로필 업데이트 요청을 보내는 코루틴 함수
    private IEnumerator UpdateProfileCoroutine(string imageURL)
    {
        // 업데이트할 프로필 정보 생성
        ProfileUpdateData updateData = new ProfileUpdateData();
        updateData.id = Managers.Data.userId; // 사용자 ID
        updateData.profile = imageURL; // 이미지 URL

        // JSON 형식으로 변환
        string jsonData = JsonUtility.ToJson(updateData);

        // 서버 URL 설정
        string serverURL = "http://15.164.2.49:3000/changeprofile";

        // POST 요청 보내기
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(serverURL, jsonData))
        {
            // 요청 헤더 설정
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 본문 설정
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

            // 응답 대기
            yield return request.SendWebRequest();

            // 성공 여부 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("프로필 업데이트 성공");
                Debug.Log("Response: " + request.downloadHandler.text);

                // 프로필 이미지 업데이트
                UpdateProfileImage(imageURL);
            }
            else
            {
                Debug.LogError("이미지 URL이 유효하지 않음. " + request.error);
            }
        }
    }

    // 프로필 이미지 업데이트 함수
    private void UpdateProfileImage(string imageURL)
    {
        // 이미지를 가져와서 텍스처로 변환하여 프로필 이미지에 설정
        StartCoroutine(LoadImage(imageURL, (texture) =>
        {
            profileImage.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }));
    }

    // 이미지를 비동기적으로 로드하는 함수
    private IEnumerator LoadImage(string url, System.Action<Texture> onComplete)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onComplete(((DownloadHandlerTexture)www.downloadHandler).texture);
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }

}

[System.Serializable]
public class ProfileUpdateData
{
    public string id;
    public string profile;
}
