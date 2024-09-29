public class UI_Popup : UI_Base
{
    public override void Init()
    {
        (Managers.UI.currentUIController as BaseUIController).SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        (Managers.UI.currentUIController as BaseUIController).ClosePopupUI(this);
    }
}
