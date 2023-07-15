using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BackgroundAudioManager : MonoBehaviour
{
    public AudioSource backgroundAudioSource;
    private void Start()
    {
        AssertAllReferenceIsNotNull();
        backgroundAudioSource.volume = MainAudioManager.volume;
    }

    private void AssertAllReferenceIsNotNull()
    {
        Assert.IsNotNull(backgroundAudioSource);
    }
}
