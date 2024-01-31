/* 인게임 매니저
 * 작성 - 이원섭
 * 연주가 진행되면서 처리하거나 읽는데 공유가 필요한 데이터를 처리하기 위한 객체 */

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
