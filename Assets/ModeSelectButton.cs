using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModeSelectButton : MonoBehaviour
{
    [SerializeField] private int minutesPerPlayer;
    [SerializeField] private int secondsAdded;
    public TMP_Text selectedModeText;
    public TMP_Text buttonText;

    public void SetMode()
    {
        Timer.startMinutes = minutesPerPlayer;
        Timer.secondsToAddAfterMove = secondsAdded;
        selectedModeText.text = "Mode chosen: " + buttonText.text;
    }
}
