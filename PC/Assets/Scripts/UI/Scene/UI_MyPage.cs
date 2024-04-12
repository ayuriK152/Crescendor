using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UI_MyPage : UI_Scene
{
    TextMeshProUGUI _userNameTMP;

    public List<Image> grassImages; // 여러 개의 이미지를 담을 리스트
    private string baseURL = "http://15.164.2.49:3000/log/getlog/";
    enum Buttons
    {
        MainMenuBtn,
        SongSelectBtn,
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
        _userNameTMP = transform.Find("UserInfo/Name/Value").GetComponent<TextMeshProUGUI>();
        _userNameTMP.text = Managers.Data.userId;
        FindImages(); // 하이라이키에서  이미지 찾기
        StartCoroutine(GetLogsForUser(Managers.Data.userId));

    }

    public void OnMainMenuBtnClick(PointerEventData data)
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnSongSelectBtnClick(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }

    // 이미지 찾아서 리스트에 추가하는 함수
    private void FindImages() 
    {
        Transform grassParent = transform.Find("UserInfo/Frequency/Grass");

        foreach (Transform child in grassParent)
        {
            Image grassImage = child.GetComponent<Image>();
            if (grassImage != null)
            {
                grassImages.Add(grassImage);
            }
        }
    }

    // 사용자의 로그를 가져오는 함수
    public IEnumerator GetLogsForUser(string userID)
    {
        string url = baseURL + userID;

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = www.downloadHandler.text;
            Debug.Log("Logs for User " + userID + ": " + jsonResult);
            // 일주일 동안의 로그 가져와서 이미지로 표시
            int logCount = GetLogPastWeek(jsonResult);
            DisplayLog(logCount);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    // 일주일 동안의 로그를 가져오는 함수
    private int GetLogPastWeek(string jsonResult)
    {
        List<LogEntry> logEntries = JsonUtility.FromJson<LogEntryList>("{\"logs\":" + jsonResult + "}").logs;
        DateTime currentDate = DateTime.Today;
        DateTime oneWeekAgo = currentDate.AddDays(-7); // 일주일 전의 날짜 계산
        int logCount = 0;

        foreach (var entry in logEntries)
        {
            DateTime date = DateTime.Parse(entry.date).Date;
            // 기록된 날짜가 일주일 전보다 이후이면서 현재 날짜보다 이전인 경우 로그 횟수 증가
            if (date >= oneWeekAgo && date <= currentDate)
            {
                logCount++;
            }
        }
        return logCount;
    }

    private void DisplayLog(int practiceCount)
    {
        Color greenColor = new Color(0f, 1f, 0f, 1f); 

        for (int i = 0; i < grassImages.Count; i++)
        {
            if (i < practiceCount) 
            {
                grassImages[i].color = greenColor; // 연습한 횟수만큼 이미지를 연두색으로 색칠
            }
            else
            {
                grassImages[i].color = Color.red; // 연습하지 않은 날은 이미지를 빨강색으로 표시
            }
        }
    }

    [Serializable]
    public class LogEntry
    {
        public string date;
    }

    [Serializable]
    public class LogEntryList
    {
        public List<LogEntry> logs;
    }


}
