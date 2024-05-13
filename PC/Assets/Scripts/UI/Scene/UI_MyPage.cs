using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UI_MyPage : UI_Scene
{
    TextMeshProUGUI _userNameTMP;
    public List<Image> grassImages; // 이미지를 담을 리스트
    Image profileImage;
    private string baseURL = "http://15.164.2.49:3000/log/getlog/";

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
        _userNameTMP = transform.Find("UserInfo/Name/Value").GetComponent<TextMeshProUGUI>();
        _userNameTMP.text = Managers.Data.userId;
        profileImage = transform.Find("UserInfo/ProfileBtn").GetComponent<Image>();
        GetButton((int)Buttons.LogOutBtn).gameObject.BindEvent(OnLogoutBtnClick);
        GetButton((int)Buttons.SecessionBtn).gameObject.BindEvent(OnSeccssionClick);
        GetButton((int)Buttons.ProfileBtn).gameObject.BindEvent(OnProfileBtnClick);
        FindImages(); // 이미지 찾기
        LoadImage(Managers.Data.userId);
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

    public void OnLogoutBtnClick(PointerEventData data)
    {
        Managers.Data.isUserLoggedIn = false;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnSeccssionClick(PointerEventData data)
    {
        Managers.ManagerInstance.AddComponent<BaseUIController>().ShowPopupUI<UI_Secession>();
    }

    public void OnProfileBtnClick(PointerEventData data)
    {
        Managers.ManagerInstance.AddComponent<OutGameUIController>().ShowPopupUI<UI_Profile>();
    }



    #region Image Settings
    public void LoadImage(string userId)
    {
        StartCoroutine(GetUserProfile(userId));
    }


    private IEnumerator GetUserProfile(string userID)
    {

        string url = "http://15.164.2.49:3000/getuser/" + userID;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                // 응답된 JSON 문자열에서 원하는 정보 추출
                string profileURL = GetProfileURL(responseText);
                // 프로필 이미지 다운로드 및 설정
                yield return StartCoroutine(SetProfileImage(profileURL));
            }
            else
            {
                Debug.LogError("유저를 찾을 수 없음" + request.error);
            }
        }
    }

    private IEnumerator SetProfileImage(string imageURL)
    {
        using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                // 텍스처 다운로드 및 이미지 UI에 설정
                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }


    private string GetProfileURL(string json)
    {
        // JSON 문자열을 파싱하여 profile 정보 추출
        string profileURL = "";

        // JSON 문자열을 파싱하고 원하는 정보를 추출하는 로직을 작성
        // 여기서는 간단하게 문자열을 찾아내는 방식으로 작성하였습니다.
        int startIndex = json.IndexOf("\"profile\":") + "\"profile\":".Length + 1;
        int endIndex = json.IndexOf("\"", startIndex + 1);
        profileURL = json.Substring(startIndex, endIndex - startIndex);

        return profileURL;
    }

    #endregion Image Settings  



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
            int[] logCounts = GetLogPastWeek(jsonResult);
            DisplayLog(logCounts);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    private int[] GetLogPastWeek(string jsonResult)
    {
        List<LogEntry> logEntries = JsonUtility.FromJson<LogEntryList>("{\"logs\":" + jsonResult + "}").logs;

        //  배열
        int[] logCounts = new int[12 * 4];

        foreach (var entry in logEntries)
        {
            DateTime date = DateTime.Parse(entry.date).Date;
            int month = date.Month;
            int week = Mathf.CeilToInt(date.Day / 7.0f);
            int rowIndex = (week - 1) * 12; // 행 인덱스
            int colIndex = month - 1; // 열 인덱스
            int imageIndex = rowIndex + colIndex;

            // 이미지 인덱스가 배열 범위를 벗어나지 않도록 보정
            if (imageIndex >= 0 && imageIndex < logCounts.Length)
            {
                logCounts[imageIndex]++;
            }
        }
        return logCounts;
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
