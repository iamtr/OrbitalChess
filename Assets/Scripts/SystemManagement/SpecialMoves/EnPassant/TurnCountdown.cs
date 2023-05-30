//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

/// <summary>
/// A countdown that decreases its 'timer' with each turn
/// </summary>
public class TurnCountdown : MonoBehaviour
{
    /// <summary>
    /// A boolean to check whether the countdown is stil ongoing
    /// A new instantiated turn countdown has its countdown over
    /// </summary>
    private bool IsCountdownOngoing = false;
    private int TurnsRemaining;

    /// <summary>
    /// Triggers the countdown and sets the turns remaining to 1
    /// </summary>
    public void TriggerTurnCountdown()
    {
        IsCountdownOngoing = true;
        TurnsRemaining = 1;
    }

    /// <summary>
    /// Checks whether the countdown is still available and reduce the 'timer' by 1
    /// </summary>
    /// <returns></returns>
    public bool IsCountdownAvailable()
    {
        if (TurnsRemaining > 0)
        {
            TurnsRemaining -= 1;
            return true;
        }
        return false;
    }
    //    public bool IsCountdownAvailable()
    //    {
    //        if (TurnRemain > 0)
    //        {
    //            TurnRemain -= 1;
    //            return true;
    //        }
    //        return false;
    //    }

    /// <summary>
    /// Adjust the countdown boolean depending on the availability of the countdown
    /// </summary>
    public void InvokeTurnCountdown()
    {
        if (IsCountdownAvailable()) return;
        IsCountdownOngoing = false;
    }

    public bool getCountdownOngoing()
    {
        return IsCountdownOngoing;
    }
}
