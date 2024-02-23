using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SignUp : UI_Popup
{
    TMP_InputField idInput;
    TMP_InputField passwordInput;
    enum Buttons
    {
        SignUpBtn,
        CloseBtn,
    }


    void Start()
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
