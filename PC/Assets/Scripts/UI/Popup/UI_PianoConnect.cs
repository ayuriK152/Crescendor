using Melanchall.DryWetMidi.Multimedia;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PianoConnect : UI_Popup
{
    private Transform deviceInstantiateTransform;

    enum Buttons
    {
        CloseBtn
    }

    void Start()
    {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(OnCloseBtnClick);

        deviceInstantiateTransform = transform.Find("Panel/Scroll View/Viewport/Content");
        Managers.Input.UpdateConnectableDevice();

        for (int i = 0; i < Managers.Input.inputDevices.Count; i++)
        {
            int currentIndex = i;
            GameObject deviceObj = Instantiate(Resources.Load("Prefabs/UI/Sub/MidiDevice") as GameObject, deviceInstantiateTransform);
            deviceObj.transform.Find("Sign").GetComponent<TextMeshProUGUI>().text = Managers.Input.inputDevices[i].Name;
            deviceObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => Managers.Input.ConnectPiano(currentIndex));
        }
    }

    public void OnCloseBtnClick(PointerEventData data)
    {
        Destroy(gameObject);
    }
}
