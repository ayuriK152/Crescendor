/* UI �Ŵ���
 * �ۼ� - �̿���
 * ���� ������ ���Ǵ� UI�� �ϰ� �����ϰ� ���ε��ϱ� ���� ����ϴ� ��ü */

using System;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class UIManager
{
    public object currentUIController;

    public void Init()
    {
        if (currentUIController != null)
        {
            Type type = currentUIController.GetType();
            switch (type.FullName)
            {
                case "PracticeModUIController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<PracticeModUIController>());
                    break;
                case "ActualModUIController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ActualModUIController>());
                    break;
                case "ResultUIController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ResultUIController>());
                    break;
            }
        }

        switch (Managers.Scene.currentScene)
        {
            case Scene.PracticeModScene:
                currentUIController = Managers.ManagerInstance.GetComponent<PracticeModUIController>();
                if (currentUIController == null)
                    currentUIController = Managers.ManagerInstance.AddComponent<PracticeModUIController>();
                break;

            case Scene.ActualModScene:
                currentUIController = Managers.ManagerInstance.GetComponent<ActualModUIController>();
                if (currentUIController == null)
                    currentUIController = Managers.ManagerInstance.AddComponent<ActualModUIController>();
                break;

            case Scene.ResultScene:
                currentUIController = Managers.ManagerInstance.GetComponent<ResultUIController>();
                if (currentUIController == null)
                    currentUIController = Managers.ManagerInstance.AddComponent<ResultUIController>();
                break;
        }
    }
}
