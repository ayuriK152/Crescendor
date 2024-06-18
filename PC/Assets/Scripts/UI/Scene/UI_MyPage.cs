using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Datas;

public class UI_MyPage : UI_Scene
{
    TextMeshProUGUI _userNameTMP;
    public List<Image> grassImages; // 이미지를 담을 리스트
    public List<Image> badgeImages;
    Image profileImage;

    enum Buttons
    {
        MainMenuBtn,
        SongSelectBtn,
        LogOutBtn,
        SecessionBtn,
        ProfileBtn,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.MainMenuBtn).gameObject.BindEvent(OnMainMenuBtnClick);
        GetButton((int)Buttons.SongSelectBtn).gameObject.BindEvent(OnSongSelectBtnClick);
        _userNameTMP = transform.Find("UserInfo/UserInfoDetail/Name/Value").GetComponent<TextMeshProUGUI>();
        _userNameTMP.text = Managers.Data.userId;
        profileImage = transform.Find("UserInfo/ProfileBtn").GetComponent<Image>();
        GetButton((int)Buttons.LogOutBtn).gameObject.BindEvent(OnLogoutBtnClick);
        GetButton((int)Buttons.SecessionBtn).gameObject.BindEvent(OnSeccssionClick);
        GetButton((int)Buttons.ProfileBtn).gameObject.BindEvent(OnProfileBtnClick);
        FindImages(); // 이미지 찾기
        LoadImage();
        Managers.Data.GetUserData(Managers.Data.userId);
        DisplayLog(Managers.Data.logCounts);
        SetBadge(Managers.Data.userCurriculumProgress);
    }


    public void OnMainMenuBtnClick(PointerEventData data)
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnSongSelectBtnClick(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }

    public void OnLogoutBtnClick(PointerEventData data)
    {
        Managers.Data.isUserLoggedIn = false;
        ResetLogData();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnSeccssionClick(PointerEventData data)
    {
        Managers.ManagerInstance.AddComponent<BaseUIController>().ShowPopupUI<UI_Secession>();
    }

    public void OnProfileBtnClick(PointerEventData data)
    {
        Managers.ManagerInstance.AddComponent<BaseUIController>().ShowPopupUI<UI_Profile>();
    }

    public void LoadImage()
    {
        StartCoroutine(Managers.Data.SetProfileImage(Managers.Data.userProfileURL, profileImage));
    }

    // 이미지 찾아서 리스트에 추가하는 함수
    private void FindImages()
    {
        Transform grassParent = transform.Find("Frequency/Background/Grass");

        foreach (Transform child in grassParent)
        {
            Image grassImage = child.GetComponent<Image>();
            if (grassImage != null)
            {
                grassImages.Add(grassImage);
            }
        }
    }

    // 로그 횟수에 따라 색상을 계산하여 이미지에 적용하는 함수
    private void DisplayLog(int[] logCounts)
    {
        for (int i = 0; i < grassImages.Count; i++)
        {
            Image grassImage = grassImages[i];
            int logCount = logCounts[i];
            Color color = CalculateColor(logCount);
            grassImage.color = color;
        }
    }

    // 로그 횟수에 따라 색상을 계산하는 함수
    private Color CalculateColor(int logCount)
    {
        if (logCount >= 5) // 많은 활동이 있었던 날은 진한 초록색으로 표시
        {
            return new Color(0f, 0.5f, 0f); // Dark green
        }
        else if (logCount > 0) // 활동이 있었던 날은 연한 초록색으로 표시
        {
            return new Color(0.5f, 1f, 0.5f); // Light green
        }
        else // 활동이 없었던 날은 회색으로 표시
        {
            return new Color(0.75f, 0.75f, 0.75f); // Gray
        }
    }

    // 로그 데이터와 이미지를 초기화하는 함수
    private void ResetLogData()
    {
        // 로그 데이터를 초기화
        for (int i = 0; i < Managers.Data.logCounts.Length; i++)
        {
            Managers.Data.logCounts[i] = 0;
        }

        // 이미지를 초기화 (회색으로 설정)
        DisplayLog(Managers.Data.logCounts);
    }

    private void SetBadge(int progreessAmount)
    {
        Transform badgeParent = transform.Find("UserInfo/CurriInfo/Badges");

        foreach (Transform child in badgeParent)
        {
            Image badgeImage = child.GetComponent<Image>();
            if (badgeImage != null)
            {
                badgeImages.Add(badgeImage);
            }
        }

        if (progreessAmount == 22)
        {
            Sprite badgeMark = Resources.Load<Sprite>("Textures/HanonBadge");

            // BadgeMark 스프라이트가 잘 로드되었는지 확인
            if (badgeMark == null)
            {
                Debug.LogError("BadgeMark sprite not found in Resources/Textures");
                return;
            }

            foreach (Transform child in badgeParent)
            {
                Image badgeImage = child.GetComponent<Image>();
                if (badgeImage != null)
                {
                    // 첫 번째 뱃지만 변경하고 루프를 탈출
                    badgeImage.sprite = badgeMark;
                    break;
                }
            }
        }
    }

}
