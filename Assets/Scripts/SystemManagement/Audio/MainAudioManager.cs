using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MainAudioManager : MonoBehaviour
{
	public Slider slider;
	public AudioSource mainAudioSource;
    public static float volume = 1;

    private void Start()
    {
        AssertAllReferenceIsNotNull();
        mainAudioSource.volume = volume;
    }

    private void AssertAllReferenceIsNotNull()
    {
        Assert.IsNotNull(slider);
        Assert.IsNotNull(mainAudioSource);
    }

    private void Update()
	{
        volume = slider.value / 100;
        mainAudioSource.volume = volume;
	}
}