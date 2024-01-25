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

    public void Init()
    {
        ConnectPiano();
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
            
            // ��Ʈ �Է� ����
            if (noteEvent.Velocity != 0)
            {
                keyChecks[noteEvent.NoteNumber - 1] = true;
                Debug.Log(noteEvent);
            }
            // ��Ʈ �Է� ����
            else if (noteEvent.Velocity == 0)
            {
                keyChecks[noteEvent.NoteNumber - 1] = false;
            }
        }
    }
}
