using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public override void Init()
    {
        (Managers.UI.currentUIController as SongSelectUIController).SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        (Managers.UI.currentUIController as SongSelectUIController).ClosePopupUI(this);
    }
}
