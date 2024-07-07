using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HandMenu : MonoBehaviour
{
    public void OnClickSongSelectBtn()
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
}
