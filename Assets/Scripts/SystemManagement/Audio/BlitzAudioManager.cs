using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bulletAudioClip;
    [SerializeField] private AudioClip blitzAudioClip;
    [SerializeField] private AudioClip rapidAudioClip;
    [SerializeField] private AudioClip gameStartAudioClip;

    public void PlayBulletAudio()
    {
        audioSource.PlayOneShot(bulletAudioClip);
    }
    public void PlayBlitzAudio()
    {
        audioSource.PlayOneShot(blitzAudioClip);
    }
    public void PlayRapidAudio()
    {
        audioSource.PlayOneShot(rapidAudioClip);
    }
    public void PlayGameStartAudio()
    {
        audioSource.PlayOneShot(gameStartAudioClip);
    }
}
