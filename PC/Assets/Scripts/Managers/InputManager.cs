/* 인풋 매니저
 * 작성 - 이원섭
 * 입력 기기의 입출력을 처리하기 위해 사용하는 객체 */

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using UnityEngine;

public class InputManager
{
    public InputDevice inputDevice;
    public bool[] keyChecks = new bool[88];

    public Action<KeyCode> keyAction;

    public void Init()
    {
        ConnectPiano();
        keyAction = null;
    }

    public void Update()
    {
        // [, ] 입력 이벤트. 구간 반복에 사용
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            keyAction.Invoke(KeyCode.LeftBracket);
        if (Input.GetKeyDown(KeyCode.RightBracket))
            keyAction.Invoke(KeyCode.RightBracket);
    }

    public void ConnectPiano()
    {
        try
        {
            inputDevice = InputDevice.GetByName("Digital Piano");
            inputDevice.EventReceived -= OnEventReceived;
            inputDevice.EventReceived += OnEventReceived;
            inputDevice.StartEventsListening();
            Debug.Log(inputDevice.IsListeningForEvents);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if (e.Event.EventType != MidiEventType.ActiveSensing)
        {
            NoteEvent noteEvent = e.Event as NoteEvent;
            
            // 노트 입력 시작
            if (noteEvent.Velocity != 0)
            {
                keyChecks[noteEvent.NoteNumber - 1] = true;
                Debug.Log(noteEvent);
            }
            // 노트 입력 종료
            else if (noteEvent.Velocity == 0)
            {
                keyChecks[noteEvent.NoteNumber - 1] = false;
            }
        }
    }
}
