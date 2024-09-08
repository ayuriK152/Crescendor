using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HandMenu : IngameUIController
{
    public void OnClickSongSelectBtn()
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
}