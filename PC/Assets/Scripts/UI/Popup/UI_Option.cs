using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Option : UI_Popup
{
    enum OptionPanels
    {
        MetronomeVolume,
    }

    enum Buttons
    {
        CloseBtn,
    }

    Slider metronomeSlider;
    TextMeshProUGUI metronomeText;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(OptionPanels));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(CloseBtnClicked);
        metronomeSlider = Get<GameObject>((int)OptionPanels.MetronomeVolume).transform.Find("Slider").GetComponent<Slider>();
        metronomeText = Get<GameObject>((int)OptionPanels.MetronomeVolume).transform.Find("Value").GetComponent<TextMeshProUGUI>();
        metronomeSlider.value = PlayerPrefs.GetFloat("user_MetronomeVolume");
        metronomeText.text = $"{Math.Truncate(metronomeSlider.value * 100)}";
        metronomeSlider.onValueChanged.AddListener((float value) => OnSliderValueChanged(OptionPanels.MetronomeVolume, value));
    }

    public void CloseBtnClicked(PointerEventData data)
    {
        Destroy(gameObject);
    }

    void OnSliderValueChanged(OptionPanels options, float value)
    {
        switch (options)
        {
            case OptionPanels.MetronomeVolume:
                metronomeSlider.value = value;
                metronomeText.text = $"{Math.Truncate(value * 100)}";
                Managers.Sound.SetMetronomeVolume(value);
                PlayerPrefs.SetFloat("user_MetronomeVolume", value);
                break;
        }
    }
}
