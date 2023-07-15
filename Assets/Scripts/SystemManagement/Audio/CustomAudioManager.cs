using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAudioManager : BackgroundAudioManager
{
    [SerializeField] private AudioSource customAudioSource;
    [SerializeField] private AudioClip buyAudioClip;

    public void PlayBuyAudio()
    {
        customAudioSource.PlayOneShot(buyAudioClip);
    }
}
