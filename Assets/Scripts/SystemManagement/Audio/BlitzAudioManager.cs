using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzAudioManager : BackgroundAudioManager
{
    [SerializeField] private AudioSource blitzAudioSource;
    [SerializeField] private AudioClip bulletAudioClip;
    [SerializeField] private AudioClip blitzAudioClip;
    [SerializeField] private AudioClip rapidAudioClip;
    [SerializeField] private AudioClip gameStartAudioClip;

    public void PlayBulletAudio()
    {
        blitzAudioSource.PlayOneShot(bulletAudioClip);
    }
    public void PlayBlitzAudio()
    {
        blitzAudioSource.PlayOneShot(blitzAudioClip);
    }
    public void PlayRapidAudio()
    {
        blitzAudioSource.PlayOneShot(rapidAudioClip);
    }
    public void PlayGameStartAudio()
    {
        blitzAudioSource.PlayOneShot(gameStartAudioClip);
    }
}
