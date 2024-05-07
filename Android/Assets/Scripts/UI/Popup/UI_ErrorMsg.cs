using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_ErrorMsg : UI_Popup
{
    enum Buttons
    {
        CheckBtn,
    }

    public TextMeshProUGUI errorMsg;
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CheckBtn).gameObject.BindEvent(CheckBtnClicked);
    }

    public void CheckBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }




}
