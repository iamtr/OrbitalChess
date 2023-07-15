using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonAudioClip;

    public void PlayButtonAudio()
    {
        audioSource.PlayOneShot(buttonAudioClip);
    }
}
