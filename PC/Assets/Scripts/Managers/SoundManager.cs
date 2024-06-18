using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public Action<bool> metronomeAction;
    public int metronomeOffset = 0;

    private AudioSource _metronomeAudioSource;

    public void Init()
    {
        metronomeAction += TriggerMetronomeSound;
        if (Managers.ManagerObj.GetComponent<AudioSource>() == null)
        {
            _metronomeAudioSource = Managers.ManagerObj.AddComponent<AudioSource>();
        }
        _metronomeAudioSource.clip = Resources.Load("Sounds/Metronome2") as AudioClip;
        _metronomeAudioSource.loop = false;
        _metronomeAudioSource.playOnAwake = false;

        if (!PlayerPrefs.HasKey("user_MetronomeVolume"))
        {
            PlayerPrefs.SetFloat("user_MetronomeVolume", 0.5f);
        }
        _metronomeAudioSource.volume = PlayerPrefs.GetFloat("user_MetronomeVolume");

        if (!PlayerPrefs.HasKey("user_MetronomeOffset"))
        {
            PlayerPrefs.SetInt("user_MetronomeOffset", 0);
        }
        metronomeOffset = PlayerPrefs.GetInt("user_MetronomeOffset");
    }

    public void SetMetronomeVolume(float value)
    {
        _metronomeAudioSource.volume = value;
    }

    private void TriggerMetronomeSound(bool isHighPitch)
    {
        if (isHighPitch)
        {
            _metronomeAudioSource.pitch = 2;
        }
        else
        {
            _metronomeAudioSource.pitch = 1;
        }
        _metronomeAudioSource.Play();
    }
}
