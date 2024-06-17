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
        SheetShowToggle
    }

    enum Buttons
    {
        CloseBtn
    }

    Slider metronomeVolumeSlider;
    Slider metronomeOffsetSlider;
    Toggle sheetShowToggle;
    TextMeshProUGUI metronomeVolumeText;
    TextMeshProUGUI metronomeOffsetText;

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
        sheetShowToggle = Get<GameObject>((int)OptionPanels.SheetShowToggle).transform.Find("Toggle").GetComponent<Toggle>();


        metronomeOffsetSlider = Get<GameObject>((int)OptionPanels.MetronomeOffset).transform.Find("Slider").GetComponent<Slider>();
        metronomeOffsetText = Get<GameObject>((int)OptionPanels.MetronomeOffset).transform.Find("Value").GetComponent<TextMeshProUGUI>();
        if (!PlayerPrefs.HasKey("user_MetronomeOffset"))
        {
            PlayerPrefs.SetInt("user_MetronomeOffset", 0);
        }
        metronomeOffsetSlider.value = PlayerPrefs.GetInt("user_MetronomeOffset");
        metronomeOffsetText.text = $"{metronomeOffsetSlider.value}";
        metronomeOffsetSlider.onValueChanged.AddListener((float value) => OnSliderValueChanged(OptionPanels.MetronomeOffset, value));

        sheetShowToggle.onValueChanged.AddListener((bool toggle) => OnSheetShowToggleChanged(toggle));
        if (!PlayerPrefs.HasKey("user_SheetShow"))
        {
            PlayerPrefs.SetInt("user_SheetShow", 0);
        }
        if (PlayerPrefs.GetInt("user_SheetShow") == 0)
            sheetShowToggle.isOn = false;
        else
            sheetShowToggle.isOn = true;
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
        }
        Managers.Ingame.OptionChangedAction.Invoke();
    }

    void OnSheetShowToggleChanged(bool toggle)
    {
        if (toggle)
        {
            PlayerPrefs.SetInt("user_SheetShow", 1);
        }
        else
        {
            PlayerPrefs.SetInt("user_SheetShow", 0);
        }
        Managers.Ingame.OptionChangedAction.Invoke();
    }
}
