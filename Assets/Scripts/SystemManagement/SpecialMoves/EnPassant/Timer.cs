using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool justMoved = false;
    private PlayerType player;
    public GameController gc;
    private int TurnRemain;
    public void TriggerTimer(Pawn pawn)
    {
        justMoved = true;
        player = pawn.Player;
        TurnRemain = 1;
    }

    public bool IsTimerAvailable()
    {
        if (TurnRemain > 0)
        {
            TurnRemain -= 1;
            return true;
        }
        return false;

    }

    public void InvokeTimer()
    {
        if (IsTimerAvailable()) return;
        justMoved = false;
    }
}
