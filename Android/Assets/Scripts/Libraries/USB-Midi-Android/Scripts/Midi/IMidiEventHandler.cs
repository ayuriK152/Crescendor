public interface IMidiEventHandler
{
    void NoteOn(int note, int velocity);
    void NoteOff(int note);
    void RawMidi(byte command, byte data1, byte data2);
    void DeviceAttached(string deviceName);
    void DeviceDetached(string deviceName);
}
