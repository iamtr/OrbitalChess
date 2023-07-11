using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
	public static string menuScene = "Main Menu";
	public static string mainGameStartScene = "Main";
	public static string blitzGameStartScene = "Blitz";
	public static string customGameModeStartScene = "Custom Game Mode";

	[SerializeField] private SettingsScript settings;

	public void StartGame()
	{
		SceneManager.LoadScene(mainGameStartScene);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void BackToMenu()
	{
		Timer.isGameStart = false;
		SceneManager.LoadScene(menuScene);
	}

	public void OpenSettings()
	{
		settings.ChangeFromMainMenuToSettings();
	}

	public void FromSettingsToMenu()
	{
		settings.ChangeFromSettingsToMainMenu();
	}

	public void DropDown(int index)
	{
		switch (index)
		{
			case 0:
				SceneManager.LoadScene(mainGameStartScene);
				break;

			case 1:
				SceneManager.LoadScene(blitzGameStartScene);
				break;

			case 2:
				SceneManager.LoadScene(customGameModeStartScene);
				break;
		}
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}
}