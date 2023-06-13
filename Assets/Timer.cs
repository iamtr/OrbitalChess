using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float blackTimeRemaining;
    float whiteTimeRemaining;
    public int startMinutes;
    public int secondsToAddAfterMove;
    public TMP_Text blackText;
    public TMP_Text whiteText;

    void Start()
    {
        blackTimeRemaining = startMinutes * 60;
        whiteTimeRemaining = startMinutes * 60;
        GameController.OnRoundEnd += AddPlayerSeconds;
    }

    void Update()
    {
        if (GameController.GetGameState() == GameState.GameOver 
            || GameController.GetGameState() == GameState.Pause) return;
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
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
            GameController.SetGameState(GameState.GameOver);
        }
        TimeSpan time = TimeSpan.FromSeconds(timeToDisplay);
        Text.text = time.ToString(@"mm\:ss");
    }

    public void AddPlayerSeconds()
    {
        if (GameController.GetCurrPlayer() == PlayerType.White)
        {
            blackTimeRemaining += secondsToAddAfterMove;
        }
        if (GameController.GetCurrPlayer() == PlayerType.Black)
        {
            whiteTimeRemaining += secondsToAddAfterMove;
        }
    }
}
