using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSelectUI : MonoBehaviour
{
    void Start()
    {
        Managers.UI.ShowSceneUI<UI_Select>();
    }

}
