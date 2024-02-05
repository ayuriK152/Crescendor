using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SongPopup : UI_Popup
{

    enum Buttons
    {
        PracticeBtn,
        ActualBtn,
        CloseBtn,
    }
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.PracticeBtn).gameObject.BindEvent(PracticeBtnClicked);
        GetButton((int)Buttons.ActualBtn).gameObject.BindEvent(ActualBtnClicked);
        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(CloseBtnClicked);
        
    }

    public void PracticeBtnClicked(PointerEventData data)
    {
        SceneManager.LoadScene("PracticeModScene");
    }
    
    public void ActualBtnClicked(PointerEventData data)
    {
        SceneManager.LoadScene("ActualModScene");
    }

    public void CloseBtnClicked(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }


}
