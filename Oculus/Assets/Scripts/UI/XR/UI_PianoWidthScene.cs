using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PianoWidthScene : MonoBehaviour
{
    public void SongSelectBtnClicked()
    {
        SceneManager.LoadScene("SongSelectScene");
    }
}
