using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
	public static string menuScene = "Main Menu";
	public static string mainGameStartScene = "Main";
	public static string blitzGameStartScene = "Blitz";
	public static string customGameModeStartScene = "Custom Game Mode";
	public static string tutorialStartScene = "Tutorial";
	public static string openingsStartScene = "Openings";
	public static string multiplayerLobbyScene = "Multiplayer Lobby";

	[SerializeField] private SettingsScript settings;

	public void StartGame()
	{
		SceneManager.LoadSceneAsync(mainGameStartScene);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void BackToMenu()
	{
		Timer.isGameStart = false;
		SceneManager.LoadSceneAsync(menuScene);
	}

	public void OpenSettings()
	{
		settings.ChangeFromMainMenuToSettings();
	}

	public void FromSettingsToMenu()
	{
		settings.ChangeFromSettingsToMainMenu();
	}

	public void PlayDropdown(int index)
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

	public void TutorialDropdown(int index)
    {
		switch (index)
		{
			case 0:
				SceneManager.LoadScene(tutorialStartScene);
				break;

			case 1:
				SceneManager.LoadScene(openingsStartScene);
				break;
		}
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void GoToMultiplayerLobby()
	{
		SceneManager.LoadScene(multiplayerLobbyScene);
	}
}