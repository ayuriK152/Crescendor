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
    TextMeshProUGUI calendarText;
    public List<Image> dateImages; // 달력 이미지 리스트
    public List<Image> badgeImages;
    Image profileImage;

    int year;
    int month;
    enum Buttons
    {
        MainMenuBtn,
        SongSelectBtn,
        LogOutBtn,
        SecessionBtn,
        ProfileBtn,
        PreviousBtn,
        NextBtn,
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
        calendarText = transform.Find("Frequency/Calendar/CalendarText").GetComponent<TextMeshProUGUI>();
        profileImage = transform.Find("UserInfo/ProfileBtn").GetComponent<Image>();
        GetButton((int)Buttons.LogOutBtn).gameObject.BindEvent(OnLogoutBtnClick);
        GetButton((int)Buttons.SecessionBtn).gameObject.BindEvent(OnSeccssionClick);
        GetButton((int)Buttons.ProfileBtn).gameObject.BindEvent(OnProfileBtnClick);
        GetButton((int)Buttons.PreviousBtn).gameObject.BindEvent(OnPreviousBtnClick);
        GetButton((int)Buttons.NextBtn).gameObject.BindEvent(OnNextBtnClick);
        LoadImage();
        Managers.Data.GetUserData(Managers.Data.userId);
        SetBadge(Managers.Data.userCurriculumProgress);

        DateTime now = DateTime.Now;
        year = now.Year;
        month = now.Month;
        UpdateCalendar();
    }

    public void OnMainMenuBtnClick(PointerEventData data)
    {
        ResetLogData();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnSongSelectBtnClick(PointerEventData data)
    {
        ResetLogData();
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

    public void OnPreviousBtnClick(PointerEventData data)
    {
        if (month == 1)
        {
            month = 12;
            year--;
        }
        else
        {
            month--;
        }
        UpdateCalendar();
    }

    public void OnNextBtnClick(PointerEventData data)
    {
        if (month == 12)
        {
            month = 1;
            year++;
        }
        else
        {
            month++;
        }
        UpdateCalendar();
    }

    public void LoadImage()
    {
        StartCoroutine(Managers.Data.SetProfileImage(Managers.Data.userProfileURL, profileImage));
    }

    // 로그 데이터와 이미지를 초기화하는 함수
    private void ResetLogData()
    {
        // 로그 데이터를 초기화
        for (int i = 0; i < Managers.Data.logCounts.Length; i++)
        {
            Managers.Data.logCounts[i] = 0;
        }
    }

    private void SetBadge(int progressAmount)
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

        if (progressAmount == 22)
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


    private void UpdateCalendar()
    {
        Transform dateParent = transform.Find("Frequency/Calendar/DateImages");

        // 기존 dateImages 리스트를 초기화
        dateImages.Clear();

        // 달력의 모든 날짜 이미지를 리스트에 추가
        foreach (Transform child in dateParent)
        {
            Image dateImage = child.GetComponent<Image>();
            if (dateImage != null)
            {
                dateImages.Add(dateImage);
            }
        }

        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        // 텍스트 업데이트
        calendarText.text = $"{year}/{month}";

        // 달력 초기화
        foreach (Image image in dateImages)
        {
            TextMeshProUGUI dateText = image.GetComponentInChildren<TextMeshProUGUI>();
            dateText.text = "";
            dateText.color = Color.red; // 기본 색상 빨강
        }

        // 날짜 채우기
        for (int day = 1; day <= daysInMonth; day++)
        {
            int index = firstDayOfWeek + (day - 1);
            TextMeshProUGUI dateText = dateImages[index].GetComponentInChildren<TextMeshProUGUI>();
            dateText.text = day.ToString();
        }

        // 로그 데이터를 바탕으로 날짜 색상 업데이트
        if (Managers.Data.GetUserLogData(Managers.Data.userId)) // 적절한 userId를 넣으세요
        {
            HighlightLogDates(Managers.Data.userLogDates);
        }

    }

    private void HighlightLogDates(List<DateTime> logDates)
    {
        foreach (DateTime logDate in logDates)
        {
            if (logDate.Year == year && logDate.Month == month)
            {
                int day = logDate.Day;
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
                int index = firstDayOfWeek + (day - 1);

                if (index >= 0 && index < dateImages.Count)
                {
                    TextMeshProUGUI dateText = dateImages[index].GetComponentInChildren<TextMeshProUGUI>();
                    dateText.color = Color.green; // 로그 있는 날짜 초록색
                }
            }
        }
    }


}