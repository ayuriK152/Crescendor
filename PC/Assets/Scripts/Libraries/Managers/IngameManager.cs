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
    public object controller;

    public void Init()
    {
        if (controller != null)
        {
            Type type = controller.GetType();
            switch (type.FullName)
            {
                case "PracticeModController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<PracticeModController>());
                    break;
                case "ActualModController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ActualModController>());
                    break;
                case "ResultController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ResultController>());
                    break;
            }
        }

        switch (Managers.Scene.currentScene)
        {
            case Scene.PracticeModScene:
                controller = Managers.ManagerInstance.GetComponent<PracticeModController>();
                if (controller == null)
                    controller = Managers.ManagerInstance.AddComponent<PracticeModController>();
                (controller as PracticeModController).Init();
                break;

            case Scene.ActualModScene:
                controller = Managers.ManagerInstance.GetComponent<ActualModController>();
                if (controller == null)
                    controller = Managers.ManagerInstance.AddComponent<ActualModController>();
                (controller as ActualModController).Init();
                break;

            case Scene.ResultScene:
                controller = Managers.ManagerInstance.GetComponent<ResultController>();
                if (controller == null)
                    controller = Managers.ManagerInstance.AddComponent<ResultController>();
                (controller as ResultController).Init();
                break;
        }
    }
}
