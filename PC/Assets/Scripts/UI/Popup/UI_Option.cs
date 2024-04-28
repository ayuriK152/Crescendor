using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Option : UI_Popup
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
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(CloseBtnClicked);
    }

    public void CloseBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }
}
