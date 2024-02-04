/* �ΰ��� �Ŵ���
 * �ۼ� - �̿���
 * ���ְ� ����Ǹ鼭 ó���ϰų� �дµ� ������ �ʿ��� �����͸� ó���ϱ� ���� ��ü
 * Ư�� ������ ����� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ �����ϴ� �뵵*/

using Unity.VisualScripting;
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

            case Scene.ActualModScene:
                controller = Managers.ManagerInstance.GetComponent<ActualModController>();
                if (controller == null)
                    controller = Managers.ManagerInstance.AddComponent<ActualModController>();
                (controller as ActualModController).Init();
                break;
        }
    }
}
