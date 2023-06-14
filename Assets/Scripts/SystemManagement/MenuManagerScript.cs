using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    public string menuScene = "main-menu";
    public string gameStartScene = "Main";

    [SerializeField] private SettingsScript settings;

    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
	{
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
}
