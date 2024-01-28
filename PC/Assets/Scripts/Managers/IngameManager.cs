/* 인게임 매니저
 * 작성 - 이원섭
 * 연주가 진행되면서 처리하거나 읽는데 공유가 필요한 데이터를 처리하기 위한 객체 */

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
        if (loopEndDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
        }
        Debug.Log($"Loop Start Delta Time Set to {loopStartDeltaTime}");
    }

    void SetEndDeltaTime()
    {
        loopEndDeltaTime = currentDeltaTime;
        if (loopStartDeltaTime >= 0 && loopStartDeltaTime > loopEndDeltaTime)
        {
            int temp = loopEndDeltaTime;
            loopEndDeltaTime = loopStartDeltaTime;
            loopStartDeltaTime = temp;
            Debug.Log($"Start/End Time Swaped! {loopStartDeltaTime} ~ {loopEndDeltaTime}");
        }
        Debug.Log($"Loop End Delta Time Set to {loopEndDeltaTime}");
    }
}
