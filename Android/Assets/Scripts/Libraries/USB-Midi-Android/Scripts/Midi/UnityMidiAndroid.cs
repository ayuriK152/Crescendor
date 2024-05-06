using UnityEngine;


public class UnityMidiAndroid
{
    private const string PluginName = "com.arthaiirgames.usbmidiandroid.UsbMidiController";
    private const string UnityPlayer = "com.unity3d.player.UnityPlayer";
    private AndroidJavaClass _pluginClass;
    private AndroidJavaObject _pluginInstance;
    private UnityMidiAndroidCallBack _unityMidiAndroidCallBack;

    public UnityMidiAndroid(IMidiEventHandler midiEventHandler)
    {
        _unityMidiAndroidCallBack = new UnityMidiAndroidCallBack(midiEventHandler);
        AndroidJavaClass unityPlayer = new AndroidJavaClass(UnityPlayer);
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        PluginInstance.Call("ctor", _unityMidiAndroidCallBack, activity);
    }
    private AndroidJavaClass PluginClass => _pluginClass ??= new AndroidJavaClass(PluginName);

    private AndroidJavaObject PluginInstance => _pluginInstance ??= PluginClass.CallStatic<AndroidJavaObject>("getInstance");
}
