public class UI_Scene : UI_Base
{
    public override void Init()
    {
        (Managers.UI.currentUIController as BaseUIController).SetCanvas(gameObject, false);
    }
}
