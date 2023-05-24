using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool justMoved = false;
    private PlayerType player;
    public GameController gc;
    public void TriggerTimer(Pawn pawn)
    {
        justMoved = true;
        player = pawn.Player;
    }

    public void EndTimer()
    {
        if (gc.CurrPlayer == player) justMoved = false;
    }
}
