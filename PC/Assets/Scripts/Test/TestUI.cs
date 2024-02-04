using UnityEngine;
using static Define;

public class TestUI : MonoBehaviour
{
    object controller;

    void Start()
    {
        if (Managers.Scene.currentScene == Define.Scene.PracticeModScene)
            controller = GameObject.Find("@Manager").GetComponent<PracticeModController>();
        else if (Managers.Scene.currentScene == Define.Scene.ActualModScene)
            controller = GameObject.Find("@Manager").GetComponent<ActualModController>();
    }

    public void ScrollNextBtn()
    {
        (controller as PracticeModController).IncreaseCurrentNoteIndex();
    }

    public void DisconnectPianoBtn()
    {
        if (Managers.Scene.currentScene == Define.Scene.PracticeModScene)
            (controller as PracticeModController).DisconnectPiano();
        else if (Managers.Scene.currentScene == Define.Scene.ActualModScene)
            (controller as ActualModController).DisconnectPiano();
    }

    public void AutoScrollBtn()
    {
        (controller as PracticeModController).AutoScroll();
    }

    public void TurnOffLoop()
    {
        (controller as PracticeModController).TurnOffLoop();
    }
}
