using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : UI_Scene
{
    enum Buttons
    {
        LoginBtn,
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.LoginBtn).gameObject.BindEvent(OnLoginButtonClick);
        
    }

    public void OnLoginButtonClick(PointerEventData data)
    {
        SceneManager.LoadScene("SelectSongScene");
    }
}
