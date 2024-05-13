/* �ΰ��� �Ŵ���
 * �ۼ� - �̿���
 * ���ְ� ����Ǹ鼭 ó���ϰų� �дµ� ������ �ʿ��� �����͸� ó���ϱ� ���� ��ü
 * Ư�� ������ ����� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ �����ϴ� �뵵*/

using System;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class IngameManager
{
    public object currentController;

    public void Init()
    {
        if (currentController != null)
        {
            Type type = currentController.GetType();
            switch (type.FullName)
            {
                case "PracticeModController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<PracticeModController>());
                    break;
                case "ActualModController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ActualModController>());
                    break;
                case "ReplayModController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ReplayModController>());
                    break;
                case "ResultController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ResultController>());
                    break;
            }
        }

        switch (Managers.Scene.currentScene)
        {
            case Scene.PracticeModScene:
                currentController = Managers.ManagerInstance.GetComponent<PracticeModController>();
                if (currentController == null)
                    currentController = Managers.ManagerInstance.AddComponent<PracticeModController>();
                (currentController as PracticeModController).Init();
                break;

            case Scene.ActualModScene:
                currentController = Managers.ManagerInstance.GetComponent<ActualModController>();
                if (currentController == null)
                    currentController = Managers.ManagerInstance.AddComponent<ActualModController>();
                (currentController as ActualModController).Init();
                break;

            case Scene.ReplayModScene:
                currentController = Managers.ManagerInstance.GetComponent<ReplayModController>();
                if (currentController == null)
                    currentController = Managers.ManagerInstance.AddComponent<ReplayModController>();
                (currentController as ReplayModController).Init();
                break;

            case Scene.ResultScene:
                currentController = Managers.ManagerInstance.GetComponent<ResultController>();
                if (currentController == null)
                    currentController = Managers.ManagerInstance.AddComponent<ResultController>();
                (currentController as ResultController).Init();
                break;
            case Scene.SongSelectScene:
                (Managers.UI.currentUIController as OutGameUIController).ShowSceneUI<UI_Select>();
                break;
        }
    }
}
