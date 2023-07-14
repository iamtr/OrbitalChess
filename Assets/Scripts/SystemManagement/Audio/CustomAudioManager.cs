using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buyAudioClip;

    public void PlayBuyAudio()
    {
        audioSource.PlayOneShot(buyAudioClip);
    }
}
