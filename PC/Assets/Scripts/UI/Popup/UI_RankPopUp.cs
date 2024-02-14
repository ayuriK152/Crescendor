using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RankPopUp : UI_Popup
{
    enum Buttons
    {
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
        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(CloseBtnClicked);

    }

    public void CloseBtnClicked(PointerEventData data)
    {
        (Managers.UI.currentUIController as SongSelectUIController).ClosePopupUI(this);
    }
}
