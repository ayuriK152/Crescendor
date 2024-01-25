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

    public void SyncDeltaTime()
    {
        currentDeltaTimeF = currentDeltaTime;
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
        Debug.Log($"Loop Start Delta Time Set to {loopStartDeltaTime}");
    }

    void SetEndDeltaTime()
    {
        loopEndDeltaTime = currentDeltaTime;
        Debug.Log($"Loop Start Delta Time Set to {loopEndDeltaTime}");
    }
}
