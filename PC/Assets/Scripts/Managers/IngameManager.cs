/* �ΰ��� �Ŵ���
 * �ۼ� - �̿���
 * ���ְ� ����Ǹ鼭 ó���ϰų� �дµ� ������ �ʿ��� �����͸� ó���ϱ� ���� ��ü */

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class IngameManager
{
    public object controller;

    public void Init()
    {
        switch (Managers.Scene.currentScene)
        {
            case Scene.PracticeModScene:
                controller = Managers.ManagerInstance.GetComponent<PracticeModController>();
                if (controller == null)
                    controller = Managers.ManagerInstance.AddComponent<PracticeModController>();
                (controller as PracticeModController).Init();
                break;
        }
    }
}
