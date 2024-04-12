using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public Action metronomeAction;

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
    }

    private void TriggerMetronomeSound()
    {
        _metronomeAudioSource.Play();
    }
}
