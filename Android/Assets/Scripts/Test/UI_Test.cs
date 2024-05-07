using Unity.VisualScripting;
using UnityEngine;

public class UI_Test : MonoBehaviour
{
    void Start()
    {
        // 可记芒 积己
        Managers.ManagerInstance.AddComponent<OutGameUIController>().ShowPopupUI<UI_Option>();
    }

}
