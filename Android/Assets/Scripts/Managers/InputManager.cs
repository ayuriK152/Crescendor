/* ��ǲ �Ŵ���
 * �ۼ� - �̿���
 * �Է� ����� ������� ó���ϱ� ���� ����ϴ� ��ü */

using System;
using UnityEngine;

public class InputManager : IMidiEventHandler
{
    public bool[] keyChecks = new bool[88];
    public bool isPianoConnected = false;

    public Action<KeyCode, Define.InputType> keyAction;
    public Action<int, int> noteAction;
    public Action<bool> pianoConnectionAction;

    public void Init()
    {
#if !UNITY_EDITOR
        Managers.ManagerObj.AddComponent<AndroidMidiManager>();
        AndroidMidiManager.Instance.RegisterEventHandler(this);
#endif
        keyAction = null;
    }

    public void Update()
    {
        if (keyAction != null)
        {
            // [, ] �Է� �̺�Ʈ. ���� �ݺ��� ���
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                keyAction.Invoke(KeyCode.LeftBracket, Define.InputType.OnKeyDown);
            if (Input.GetKeyDown(KeyCode.RightBracket))
                keyAction.Invoke(KeyCode.RightBracket, Define.InputType.OnKeyDown);

            // For Test
            if (Input.GetKeyDown(KeyCode.A))
                keyAction.Invoke(KeyCode.A, Define.InputType.OnKeyDown);
            if (Input.GetKeyUp(KeyCode.A))
                keyAction.Invoke(KeyCode.A, Define.InputType.OnKeyUp);

            if (Input.GetKeyDown(KeyCode.Escape))
                keyAction.Invoke(KeyCode.Escape, Define.InputType.OnKeyDown);
        }
    }

    public void DeviceAttached(string deviceName)
    {
        isPianoConnected = true;
        if (pianoConnectionAction != null)
        {
            pianoConnectionAction.Invoke(isPianoConnected);
        }
    }

    public void DeviceDetached(string deviceName)
    {
        isPianoConnected = false;
        if (pianoConnectionAction != null)
        {
            pianoConnectionAction.Invoke(isPianoConnected);
        }
    }

    public void NoteOn(int note, int velocity)
    {
        if (noteAction != null)
        {
            noteAction.Invoke(note, velocity);
        }
    }

    public void NoteOff(int note)
    {
        if (noteAction != null)
        {
            noteAction.Invoke(note, 0);
        }
    }
}
