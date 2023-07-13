using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningsGameController : GameController
{
    protected override void Start()
    {
        base.Start();
        SetGameState(GameState.GameOver);
    }

    public override void SetPlayer()
    {
        return;
    }

    public override void ResetGame()
    {
        return;
    }
}
