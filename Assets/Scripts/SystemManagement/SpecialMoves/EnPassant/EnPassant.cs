using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnPassant : MonoBehaviour
{
    [SerializeField] private Timer[] timers;
    public Timer Timer;

    [SerializeField] private int id;
    private int numOfPawns = 16;

    public Timer InstantiateTimer()
    {
        if (id == numOfPawns)
        {
            id = 0;
        }
        Timer timer = Instantiate(Timer);
        timers[id] = timer;
        id += 1;
        return timer;
    }

    public void InvokeEveryTimer()
    {
        foreach (Timer timer in timers)
        {
            timer.InvokeTimer();
        }
    }
}
