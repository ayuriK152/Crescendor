using System;
using System.Collections.Generic;
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
}