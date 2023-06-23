using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour
{
	public Slider slider;
	public AudioSource audioSource;

    private void Start()
    {
        AssertAllReferenceIsNotNull();
    }

    private void AssertAllReferenceIsNotNull()
    {
        Assert.IsNotNull(slider);
        Assert.IsNotNull(audioSource);
    }

    private void Update()
	{
		audioSource.volume = slider.value / 100;
	}
}