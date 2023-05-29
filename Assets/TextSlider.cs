using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSlider : MonoBehaviour
{
    public TMP_Text volumeNumber;

    public void SetVolumeNumber(float value)
    {
        volumeNumber.text = value.ToString();
    }
}
