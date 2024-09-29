using UnityEngine;

public class UnityMidiAndroidCallBack : AndroidJavaProxy
{
    private IMidiEventHandler _eventHandler;
    public UnityMidiAndroidCallBack(IMidiEventHandler midiEventHandler) : base("com.arthaiirgames.usbmidiandroid.IMidiCallback")
    {
        _eventHandler = midiEventHandler;
    }
    
    public void NoteOn(int note, int velocity)
    {
        _eventHandler.NoteOn(note, velocity);
    }

    public void NoteOff(int note)
    {
        _eventHandler.NoteOff(note);
    }

    public void DeviceAttached(string deviceName)
    {
        _eventHandler.DeviceAttached(deviceName);
    }

    public void DeviceDetached(string deviceName)
    {
        _eventHandler.DeviceDetached(deviceName);
    }
    public void RawMidi(byte command, byte data1, byte data2)
    {
        _eventHandler.RawMidi(command, data1, data2);
    }
}