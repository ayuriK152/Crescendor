using UnityEngine;

public class TestUI : MonoBehaviour
{
    PracticeModController practiceModController;

    void Start()
    {
        practiceModController = GameObject.Find("@Manager").GetComponent<PracticeModController>();
    }

    public void ScrollNextBtn()
    {
        practiceModController.IncreaseCurrentNoteIndex();
    }

    public void DisconnectPianoBtn()
    {
        practiceModController.DisconnectPiano();
    }

    public void AutoScrollBtn()
    {
        practiceModController.AutoScroll();
    }

    public void TurnOffLoop()
    {
        practiceModController.TurnOffLoop();
    }
}
