using System;
using UnityEngine;
using UnityEngine.UI;

namespace Midi
{
    public class MidiEventHandler : MonoBehaviour, IMidiEventHandler
    {
        [SerializeField] private Text text;

        private void Awake()
        {
            gameObject.AddComponent<AndroidMidiManager>();
        }

        private void Start()
        {
            AndroidMidiManager.Instance.RegisterEventHandler(this);
        }

        public void NoteOn(int note, int velocity)
        {
            Debug.Log("Note On " + note + " velocity " + velocity);
            text.text += "Note On " + note + " velocity " + velocity + Environment.NewLine;
        }

        public void NoteOff(int note)
        {
            Debug.Log("Note off " + note);
            text.text += "Note off " + note + Environment.NewLine;
        }

        public void DeviceAttached(string deviceName)
        {
            Debug.Log("Device Attached " + deviceName);
            text.text += "Device Attached " + deviceName + Environment.NewLine;
        }

        public void DeviceDetached(string deviceName)
        {
            Debug.Log("Device Detached " + deviceName);
            text.text += "Device Detached " + deviceName + Environment.NewLine;
        }
        public void RawMidi(byte command, byte data1, byte data2)
        {
            Debug.Log("Raw Midi " + command + "data1" + data1 + "data2" + data2);
            text.text += "Raw Midi " + command + "data1" + data1 + "data2" + data2 + Environment.NewLine;
        }
    }
}