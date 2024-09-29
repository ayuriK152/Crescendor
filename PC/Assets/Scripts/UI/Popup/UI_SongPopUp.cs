using TMPro;
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
        string[] fileName = PlayerPrefs.GetString("trans_SongTitle").Replace("_", " ").Split("-");
        transform.Find("Panel/Title").GetComponent<TextMeshProUGUI>().text = fileName[0];
        transform.Find("Panel/Composer").GetComponent<TextMeshProUGUI>().text = fileName[1];
    }

    public void PracticeBtnClicked(PointerEventData data)
    {
        LoadingController.SceneLoading("PracticeModScene");
    }
    
    public void ActualBtnClicked(PointerEventData data)
    {
        LoadingController.SceneLoading("ActualModScene");
    }

    public void CloseBtnClicked(PointerEventData data)
    {
        (Managers.UI.currentUIController as BaseUIController).ClosePopupUI(this);
    }
}
