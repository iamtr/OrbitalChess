using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;


public class SettingsScript : MonoBehaviour
{
	public GameObject mainMenuPanel;
	public GameObject settingsPanel;

	public TMP_Text volumeNumber;
	public Slider slider;

    private void Start()
    {
		AssertAllReferenceIsNotNull();
    }

    private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(mainMenuPanel);
		Assert.IsNotNull(settingsPanel);
		Assert.IsNotNull(volumeNumber);
		Assert.IsNotNull(slider);
	}

	public void SetVolumeNumber()
	{
		volumeNumber.text = slider.value.ToString();
	}

	public void ChangeFromMainMenuToSettings()
	{
		mainMenuPanel.SetActive(false);
		settingsPanel.SetActive(true);
	}

	public void ChangeFromSettingsToMainMenu()
	{
		settingsPanel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}
}