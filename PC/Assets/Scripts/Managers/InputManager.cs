/* ��ǲ �Ŵ���
 * �ۼ� - �̿���
 * �Է� ����� ������� ó���ϱ� ���� ����ϴ� ��ü */

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using UnityEngine;

public class InputManager
{
    public InputDevice inputDevice;
    public bool[] keyChecks = new bool[88];

    public Action<KeyCode, Define.InputType> keyAction;

    public void Init()
    {
        ConnectPiano();
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

    public void ConnectPiano()
    {
        try
        {
            inputDevice = InputDevice.GetByName("Digital Piano");
            inputDevice.StartEventsListening();
            Debug.Log(inputDevice.IsListeningForEvents);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
