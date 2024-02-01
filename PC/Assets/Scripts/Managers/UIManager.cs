/* UI 매니저
 * 작성 - 이원섭
 * 여러 씬에서 사용되는 UI를 일괄 관리하고 바인딩하기 위해 사용하는 객체 */

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
