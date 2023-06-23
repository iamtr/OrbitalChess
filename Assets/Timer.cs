using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public static bool isGameStart = false;
	private static float blackTimeRemaining;
	private static float whiteTimeRemaining;
	public static int startMinutes;
	public static int secondsToAddAfterMove;
	public TMP_Text blackText;
	public TMP_Text whiteText;

	public static bool isBlackBelow = true;

	private void Start()
	{
		if (isBlackBelow)
		{
			whiteText.rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
			whiteText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
			whiteText.rectTransform.anchoredPosition = new Vector2(-260, -125);
			blackText.rectTransform.anchorMin = new Vector2(1.0f, 0.0f);
			blackText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
			blackText.rectTransform.anchoredPosition = new Vector2(-260, 125);
		}
		else
		{
			whiteText.rectTransform.anchorMin = new Vector2(1.0f, 0.0f);
			whiteText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
			whiteText.rectTransform.anchoredPosition = new Vector2(-260, 125);
			blackText.rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
			blackText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
			blackText.rectTransform.anchoredPosition = new Vector2(-260, -125);
		}
		GameController.OnRoundEnd += AddPlayerSeconds;
	}

	private void Update()
	{
		if (!isGameStart)
		{
			ResetTimers();
		}
		if (GameController.GetGameState() == GameState.GameOver
			|| GameController.GetGameState() == GameState.Pause
			|| !isGameStart) return;
		if (GameController.GetCurrPlayer() == PlayerType.White)
		{
			whiteTimeRemaining -= Time.deltaTime;
		}
		if (GameController.GetCurrPlayer() == PlayerType.Black)
		{
			blackTimeRemaining -= Time.deltaTime;
		}
		DisplayTimer(blackTimeRemaining, blackText);
		DisplayTimer(whiteTimeRemaining, whiteText);
	}

	public void DisplayTimer(float timeToDisplay, TMP_Text Text)
	{
		if (timeToDisplay < 0)
		{
			timeToDisplay = 0;
			GameController.SetGameState(GameState.GameOver);
		}
		TimeSpan time = TimeSpan.FromSeconds(timeToDisplay);
		Text.text = time.ToString(@"mm\:ss");
	}

	public void AddPlayerSeconds()
	{
		if (GameController.GetCurrPlayer() == PlayerType.Black)
		{
			blackTimeRemaining += secondsToAddAfterMove;
			DisplayTimer(blackTimeRemaining, blackText);
		}
		if (GameController.GetCurrPlayer() == PlayerType.White)
		{
			whiteTimeRemaining += secondsToAddAfterMove;
			DisplayTimer(whiteTimeRemaining, whiteText);
		}
	}

	public static void ResetTimers()
	{
		blackTimeRemaining = startMinutes * 60;
		whiteTimeRemaining = startMinutes * 60;
	}
}