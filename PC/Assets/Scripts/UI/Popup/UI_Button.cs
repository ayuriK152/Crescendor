using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    enum Buttons
    {
        SongButton
    }

    enum Texts
    {
        SongTitle,
        Composer,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        SongIcon,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.SongButton).gameObject.BindEvent(OnButtonClicked);

        GameObject go = GetImage((int)Images.SongIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }


    public void OnButtonClicked(PointerEventData data)
    {

    }
}
