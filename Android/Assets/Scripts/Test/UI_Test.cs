using Unity.VisualScripting;
using UnityEngine;

public class UI_Test : MonoBehaviour
{
    void Start()
    {
        // �ɼ�â ����
        Managers.ManagerInstance.AddComponent<OutGameUIController>().ShowPopupUI<UI_Option>();
    }

}
