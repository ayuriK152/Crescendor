/* �ΰ��� �Ŵ���
 * �ۼ� - �̿���
 * ���ְ� ����Ǹ鼭 ó���ϰų� �дµ� ������ �ʿ��� �����͸� ó���ϱ� ���� ��ü */

using UnityEngine;

public class IngameManager
{
    public int passedNote;
    public int totalNote;

    public bool isLoop;
    public int loopStartDeltaTime;
    public int loopEndDeltaTime;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    public void Init()
    {
        passedNote = 0;
        totalNote = 0;

        isLoop = false;
        loopStartDeltaTime = -1;
        loopEndDeltaTime = -1;
        currentDeltaTimeF = 0;

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void SyncDeltaTime(bool isIntToFloat)
    {
        if (isIntToFloat)
        {
            currentDeltaTimeF = currentDeltaTime;
        }
        else
        {
            currentDeltaTime = currentDeltaTimeF - (int)currentDeltaTimeF < 0.5f ? (int)currentDeltaTimeF : (int)currentDeltaTimeF + 1;
        }
        Managers.UI.songTimeSlider.SetValueWithoutNotify(currentDeltaTime);
    }

    void InputKeyEvent(KeyCode keyCode)
    {
        switch(keyCode)
        {
            case KeyCode.LeftBracket:
                SetStartDeltaTime();
                break;
            case KeyCode.RightBracket:
                SetEndDeltaTime();
                break;
        }
    }

    void SetStartDeltaTime()
    {
        loopStartDeltaTime = currentDeltaTime;
        Managers.UI.SetLoopStartMarker();
        if (loopEndDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            Managers.UI.SwapStartEndMarker();
        }
        Debug.Log($"Loop Start Delta Time Set to {loopStartDeltaTime}");
        if (loopEndDeltaTime >= 0)
            isLoop = true;
    }

    void SetEndDeltaTime()
    {
        loopEndDeltaTime = currentDeltaTime;
        Managers.UI.SetLoopEndMarker();
        if (loopStartDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
            Managers.UI.SwapStartEndMarker();
        }
        Debug.Log($"Loop End Delta Time Set to {loopEndDeltaTime}");
        if (loopStartDeltaTime >= 0)
            isLoop = true;
    }
}
