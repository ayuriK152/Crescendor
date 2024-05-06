public interface IMidiEventHandler
{
    void NoteOn(int note, int velocity);
    void NoteOff(int note);
    void DeviceAttached(string deviceName);
    void DeviceDetached(string deviceName);
}
