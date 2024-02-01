/* UI �Ŵ���
 * �ۼ� - �̿���
 * ���� ������ ���Ǵ� UI�� �ϰ� �����ϰ� ���ε��ϱ� ���� ����ϴ� ��ü */

using Unity.VisualScripting;
using static Define;

public class UIManager
{
    public object currentUIController;

    public void Init()
    {
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
        }
    }
}
