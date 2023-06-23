using UnityEngine;

public class SettingsScript : MonoBehaviour
{
	public GameObject mainMenuPanel;
	public GameObject settingsPanel;

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