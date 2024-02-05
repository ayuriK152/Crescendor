/* 인게임 매니저
 * 작성 - 이원섭
 * 연주가 진행되면서 처리하거나 읽는데 공유가 필요한 데이터를 처리하기 위한 객체 */

using UnityEngine;

public class IngameManager
{
    public int passedNote;
    public int totalNote;
    public int currentNoteIndex;

    public bool isLoop;
    public int loopStartDeltaTime;
    public int loopEndDeltaTime;
    public int loopStartNoteIndex;
    public int loopStartPassedNote;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    public void Init()
    {
        passedNote = 0;
        totalNote = 0;
        currentNoteIndex = 0;
        loopStartNoteIndex = 0;
        loopStartPassedNote = 0;

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
        loopStartNoteIndex = currentNoteIndex;
        loopStartPassedNote = passedNote;
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
        if (loopStartDeltaTime < 0)
            return;
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

    public void TurnOffLoop()
    {
        isLoop = false;
        loopStartDeltaTime = -1;
        loopEndDeltaTime = -1;
    }
}
