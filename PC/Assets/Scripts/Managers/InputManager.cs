/* ��ǲ �Ŵ���
 * �ۼ� - �̿���
 * �Է� ����� ������� ó���ϱ� ���� ����ϴ� ��ü */

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager
{
    public InputDevice selectedInputDevice;
    public List<InputDevice> inputDevices;
    public bool[] keyChecks = new bool[88];

    public Action<KeyCode, Define.InputType> keyAction;

    public void Init()
    {
        inputDevices = new List<InputDevice>();
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

    public void ConnectPiano(int deviceIdx)
    {
        try
        {
            if (selectedInputDevice != null)
            {
                if (selectedInputDevice.IsListeningForEvents)
                {
                    DisconnectPiano();
                }
            }
            selectedInputDevice = InputDevice.GetByIndex(deviceIdx);
            selectedInputDevice.StartEventsListening();
            Debug.Log(selectedInputDevice.IsListeningForEvents);

            Managers.UI.ShowMsg("피아노 연결 성공");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Managers.UI.ShowMsg($"피아노 연결 실패\n{e.Message}");
        }
    }

    public void DisconnectPiano()
    {
        try
        {
            selectedInputDevice.StopEventsListening();
            selectedInputDevice.Dispose();
            Debug.Log("Piano Disconnected!");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void UpdateConnectableDevice()
    {
        inputDevices.Clear();
        inputDevices = InputDevice.GetAll().ToList();
    }
}
