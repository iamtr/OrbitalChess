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
}
