using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PracticeModUIController : MonoBehaviour
{
    public TextMeshProUGUI songTitleTMP;
    public TextMeshProUGUI songNoteMountTMP;
    public TextMeshProUGUI songBpmTMP;
    public TextMeshProUGUI songBeatTMP;
    public Slider songTimeSlider;
    public GameObject songTimeSliderHandle;
    public GameObject loopStartMarker;
    public Image loopStartMarkerSprite;
    public GameObject loopEndMarker;
    public Image loopEndMarkerSprite;

    PracticeModController practiceModController;

    public void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/TimeSlider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        songBpmTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
        songTimeSlider = GameObject.Find("MainCanvas/TimeSlider/Slider").GetComponent<Slider>();
        songTimeSliderHandle = GameObject.Find("MainCanvas/TimeSlider/Slider/Handle Slide Area/Handle");
        loopStartMarker = GameObject.Find("MainCanvas/TimeSlider/Slider/LoopStartMarker");
        loopEndMarker = GameObject.Find("MainCanvas/TimeSlider/Slider/LoopEndMarker");
        loopStartMarkerSprite = loopStartMarker.GetComponent<Image>();
        loopEndMarkerSprite = loopEndMarker.GetComponent<Image>();
        practiceModController = Managers.Ingame.controller as PracticeModController;

        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
    }

    public void SetLoopStartMarker()
    {
        loopStartMarker.transform.position = songTimeSliderHandle.transform.position;
        loopStartMarkerSprite.enabled = true;
    }

    public void SetLoopEndMarker()
    {
        loopEndMarker.transform.position = songTimeSliderHandle.transform.position;
        loopEndMarkerSprite.enabled = true;
    }

    public void SwapStartEndMarker()
    {
        Vector3 temp = loopStartMarker.transform.position;
        loopStartMarker.transform.position = loopEndMarker.transform.position;
        loopEndMarker.transform.position = loopStartMarker.transform.position;
    }

    public void TurnOffLoop()
    {
        loopStartMarkerSprite.enabled = false;
        loopEndMarkerSprite.enabled = false;
    }

    public void UpdatePassedNote()
    {
        songNoteMountTMP.text = $"{practiceModController.passedNote}/{practiceModController.totalNote}";
    }
}
