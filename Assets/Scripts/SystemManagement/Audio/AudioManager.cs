using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	public Slider slider;
	public AudioSource audioSource;

	private void Update()
	{
		audioSource.volume = slider.value / 100;
	}
}