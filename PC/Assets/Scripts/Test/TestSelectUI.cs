using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSelectUI : MonoBehaviour
{
    void Start()
    {
        (Managers.UI.currentUIController as OutGameUIController).ShowSceneUI<UI_Select>();
    }

}
