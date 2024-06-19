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
        MetronomeOffset,
        ScrollSpeed
    }

    enum Buttons
    {
        CloseBtn,
    }

    Slider metronomeVolumeSlider;
    Slider metronomeOffsetSlider;
    Slider scrollSpeedSlider;
    TextMeshProUGUI metronomeVolumeText;
    TextMeshProUGUI metronomeOffsetText;
    TextMeshProUGUI scrollSpeedText;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(OptionPanels));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(CloseBtnClicked);

        metronomeVolumeSlider = Get<GameObject>((int)OptionPanels.MetronomeVolume).transform.Find("Slider").GetComponent<Slider>();
        metronomeVolumeText = Get<GameObject>((int)OptionPanels.MetronomeVolume).transform.Find("Value").GetComponent<TextMeshProUGUI>();
        metronomeVolumeSlider.value = PlayerPrefs.GetFloat("user_MetronomeVolume");
        metronomeVolumeText.text = $"{Math.Truncate(metronomeVolumeSlider.value * 100)}";
        metronomeVolumeSlider.onValueChanged.AddListener((float value) => OnSliderValueChanged(OptionPanels.MetronomeVolume, value));

        metronomeOffsetSlider = Get<GameObject>((int)OptionPanels.MetronomeOffset).transform.Find("Slider").GetComponent<Slider>();
        metronomeOffsetText = Get<GameObject>((int)OptionPanels.MetronomeOffset).transform.Find("Value").GetComponent<TextMeshProUGUI>();
        if (!PlayerPrefs.HasKey("user_MetronomeOffset"))
        {
            PlayerPrefs.SetInt("user_MetronomeOffset", 0);
        }
        metronomeOffsetSlider.value = PlayerPrefs.GetInt("user_MetronomeOffset");
        metronomeOffsetText.text = $"{metronomeOffsetSlider.value}";
        metronomeOffsetSlider.onValueChanged.AddListener((float value) => OnSliderValueChanged(OptionPanels.MetronomeOffset, value));

        scrollSpeedSlider = Get<GameObject>((int)OptionPanels.ScrollSpeed).transform.Find("Slider").GetComponent<Slider>();
        scrollSpeedText = Get<GameObject>((int)OptionPanels.ScrollSpeed).transform.Find("Value").GetComponent<TextMeshProUGUI>();
        if (!PlayerPrefs.HasKey("user_ScrollSpeed"))
        {
            PlayerPrefs.SetFloat("user_ScrollSpeed", 1.0f);
        }
        Managers.Midi.noteScaleZ = PlayerPrefs.GetFloat("user_ScrollSpeed");
        scrollSpeedSlider.value = PlayerPrefs.GetFloat("user_ScrollSpeed");
        scrollSpeedText.text = $"{Math.Truncate(metronomeOffsetSlider.value * 100.0f) / 100.0f}";
        scrollSpeedSlider.onValueChanged.AddListener((float value) => OnSliderValueChanged(OptionPanels.ScrollSpeed, value));
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
                metronomeVolumeSlider.value = value;
                metronomeVolumeText.text = $"{Math.Truncate(value * 100)}";
                Managers.Sound.SetMetronomeVolume(value);
                PlayerPrefs.SetFloat("user_MetronomeVolume", value);
                break;

            case OptionPanels.MetronomeOffset:
                metronomeOffsetSlider.value = value;
                metronomeOffsetText.text = $"{value}";
                Managers.Sound.metronomeOffset = (int)value;
                PlayerPrefs.SetInt("user_MetronomeOffset", (int)value);
                break;

            case OptionPanels.ScrollSpeed:
                Managers.Midi.noteScaleZ = value;
                scrollSpeedText.text = $"{Math.Truncate(value * 100.0f) / 100.0f}";
                PlayerPrefs.SetFloat("user_ScrollSpeed", (float)Math.Truncate(value * 100.0f) / 100.0f);
                break;
        }
    }
}
