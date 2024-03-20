using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UI_MyPage : UI_Scene
{
    TextMeshProUGUI nickname;
    enum Buttons
    {
        MainMenuBtn,
        SongSelectBtn,
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.MainMenuBtn).gameObject.BindEvent(OnMainMenuBtnClick);
        GetButton((int)Buttons.SongSelectBtn).gameObject.BindEvent(OnSongSelectBtnClick);
        string savedID = PlayerPrefs.GetString("PlayerID");
        nickname = GameObject.Find("UI_MyPage/UserInfo/Profile/Nickname").GetComponent<TextMeshProUGUI>();
        nickname.text = savedID;
    }

    public void OnMainMenuBtnClick(PointerEventData data)
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnSongSelectBtnClick(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
}
