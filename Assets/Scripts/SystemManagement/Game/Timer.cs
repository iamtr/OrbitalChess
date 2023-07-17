using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Timer : MonoBehaviour
{
	public static bool isGameStart = false;
	private static float blackTimeRemaining;
	private static float whiteTimeRemaining;
	public static float startMinutes;
	public static float secondsToAddAfterMove;

	public TMP_Text blackTimerText;
	public TMP_Text whiteTimerText;

	[SerializeField] private Transform bottomTimerTransform;
	[SerializeField] private Transform topTimerTransform;

	public static bool isBlackBelow = true;

	private void Start()
	{
		AssertAllReferenceIsNotNull();

		if (isBlackBelow)
		{
			//whiteTimerText.rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
			//whiteTimerText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
			//whiteTimerText.rectTransform.anchoredPosition = new Vector2(-260, -125);
			//blackTimerText.rectTransform.anchorMin = new Vector2(1.0f, 0.0f);
			//blackTimerText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
			//blackTimerText.rectTransform.anchoredPosition = new Vector2(-260, 125);
			blackTimerText.gameObject.transform.position = bottomTimerTransform.position;
			whiteTimerText.gameObject.transform.position = topTimerTransform.position;
		}
		else
		{
			//whiteTimerText.rectTransform.anchorMin = new Vector2(1.0f, 0.0f);
			//whiteTimerText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
			//whiteTimerText.rectTransform.anchoredPosition = new Vector2(-260, 125);
			//blackTimerText.rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
			//blackTimerText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
			//blackTimerText.rectTransform.anchoredPosition = new Vector2(-260, -125);
			whiteTimerText.gameObject.transform.position = bottomTimerTransform.position;
			blackTimerText.gameObject.transform.position = topTimerTransform.position;
		}
		
	}

    private void OnEnable()
    {
		GameController.OnRoundEnd += AddPlayerSeconds;
	}

    private void OnDisable()
    {
		GameController.OnRoundEnd -= AddPlayerSeconds;
	}

    private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(blackTimerText);
		Assert.IsNotNull(whiteTimerText);
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
		DisplayTimer(blackTimeRemaining, blackTimerText);
		DisplayTimer(whiteTimeRemaining, whiteTimerText);
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
			DisplayTimer(blackTimeRemaining, blackTimerText);
		}
		if (GameController.GetCurrPlayer() == PlayerType.White)
		{
			whiteTimeRemaining += secondsToAddAfterMove;
			DisplayTimer(whiteTimeRemaining, whiteTimerText);
		}
	}

	public static void ResetTimers()
	{
		blackTimeRemaining = startMinutes * 60;
		whiteTimeRemaining = startMinutes * 60;
	}
}