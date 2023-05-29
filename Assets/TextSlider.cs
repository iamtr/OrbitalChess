using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSlider : MonoBehaviour
{
    public TMP_Text volumeNumber;

    public void SetVolumeNumber(float value)
    {
        int i = (int)value;
        volumeNumber.text = i.ToString();
    }
}
