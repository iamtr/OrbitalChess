using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoveAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveSelfAudioClip;
    [SerializeField] private AudioClip captureAudioClip;
    [SerializeField] private AudioClip castleAudioClip;
    [SerializeField] private AudioClip moveCheckAudioClip;
    [SerializeField] private AudioClip promoteAudioClip;
    [SerializeField] private AudioClip purchaseSuccessAudioClip;

    public void PlayMoveSelfAudio()
    {
        audioSource.PlayOneShot(moveSelfAudioClip);
    }

    public void PlayCaptureAudio()
    {
        audioSource.PlayOneShot(captureAudioClip);
    }

    public void PlayCastleAudio()
    {
        audioSource.PlayOneShot(castleAudioClip);
    }

    public void PlayMoveCheckAudio()
    {
        audioSource.PlayOneShot(moveCheckAudioClip);
    }

    public void PlayPromoteAudio()
    {
        audioSource.PlayOneShot(promoteAudioClip);
    }

    public void PlayPurchaseSuccessAudio()
    {
        audioSource.PlayOneShot(purchaseSuccessAudioClip);
    }
}
