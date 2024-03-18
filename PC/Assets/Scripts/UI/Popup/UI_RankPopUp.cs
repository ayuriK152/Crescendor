using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RankPopUp : UI_Popup
{
    enum Buttons
    {
        CloseButton,
        ReplayButton,
    }

    enum Texts
    {
        Title,
        Composer,
    }

    enum TextParents
    {
        Rank,
        PlayerName,
        Date,
        Score,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Texts));
        //Bind<GameObject>(typeof(TextParents));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClick);
        GetButton((int)Buttons.ReplayButton).gameObject.BindEvent(OnReplayButtonClick);

        GetObject((int)Texts.Title).transform.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[0].Replace("_", " ");
        GetObject((int)Texts.Composer).transform.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[1].Replace("_", " ");
    }

    public void OnCloseButtonClick(PointerEventData data)
    {
        (Managers.UI.currentUIController as OutGameUIController).ClosePopupUI(this);
    }

    void OnReplayButtonClick(PointerEventData data)
    {

    }
}
