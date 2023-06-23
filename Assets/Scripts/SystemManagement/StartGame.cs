using UnityEngine;

public class StartGame : MonoBehaviour
{
	public GameObject board;
	public GameObject gameCanvas;
	public GameObject modeSelectCanvas;

	public void startGame()
	{
		modeSelectCanvas.SetActive(false);
		board.SetActive(true);
		gameCanvas.SetActive(true);
		Timer.isGameStart = true;
	}
}