using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RankPopUp : UI_Popup
{
    TextMeshProUGUI _rankTMP;
    TextMeshProUGUI _playerNameTMP;
    TextMeshProUGUI _dateTMP;
    TextMeshProUGUI _scoreTMP;

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

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Texts));
        _rankTMP = transform.Find("Panel/PlayData/Rank/Value").GetComponent<TextMeshProUGUI>();
        _playerNameTMP = transform.Find("Panel/PlayData/PlayerName/Value").GetComponent<TextMeshProUGUI>();
        _dateTMP = transform.Find("Panel/PlayData/Date/Value").GetComponent<TextMeshProUGUI>();
        _scoreTMP = transform.Find("Panel/PlayData/Score/Value").GetComponent<TextMeshProUGUI>();

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClick);
        GetButton((int)Buttons.ReplayButton).gameObject.BindEvent(OnReplayButtonClick);

        GetObject((int)Texts.Title).transform.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[0].Replace("_", " ");
        GetObject((int)Texts.Composer).transform.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("trans_SongTitle").Split('-')[1].Replace("_", " ");

        _rankTMP.text = $"#{Convert.ToInt32(gameObject.name) + 1}";
        _playerNameTMP.text = Managers.Data.rankRecord.user_id;
        _dateTMP.text = $"{Managers.Data.rankRecord.date.Substring(0, 10)}";
        _scoreTMP.text = $"{Math.Truncate(Managers.Data.rankRecord.score * 10000) / 100}%";
    }

    public void OnCloseButtonClick(PointerEventData data)
    {
        (Managers.UI.currentUIController as BaseUIController).ClosePopupUI(this);
    }

    void OnReplayButtonClick(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.ReplayModScene);
    }
}
