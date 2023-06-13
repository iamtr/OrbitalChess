using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelectButton : MonoBehaviour
{
    [SerializeField] private int minutesPerPlayer;
    [SerializeField] private int secondsAdded;

    public void SetMode()
    {
        Timer.startMinutes = minutesPerPlayer;
        Timer.secondsToAddAfterMove = secondsAdded;
    }
}
