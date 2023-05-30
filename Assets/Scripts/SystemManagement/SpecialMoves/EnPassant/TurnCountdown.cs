//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TurnCountdown : MonoBehaviour
//{
//    private bool justMoved = false;
//    private int TurnRemain;

//    public void TriggerTurnCountdown()
//    {
//        justMoved = true;
//        TurnRemain = 1;
//    }

//    public bool IsCountdownAvailable()
//    {
//        if (TurnRemain > 0)
//        {
//            TurnRemain -= 1;
//            return true;
//        }
//        return false;

//    }

//    public void InvokeTimer()
//    {
//        if (IsCountdownAvailable()) return;
//        justMoved = false;
//    }

//    public bool IsJustMoved()
//    {
//        return justMoved;
//    }
//}
