using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorOptionDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Text selectedOptionText;
    public void Dropdown(int index)
    {
        switch (index)
        {
            case 0:
                Piece.isBlackBelow = true;
                BoardController.isBlackBelow = true;
                HighlightManager.isBlackBelow = true;
                Timer.isBlackBelow = true;
                selectedOptionText.text = "Black";
                break;
            case 1:
                Piece.isBlackBelow = false;
                BoardController.isBlackBelow = false;
                HighlightManager.isBlackBelow = false;
                Timer.isBlackBelow = false;
                selectedOptionText.text = "White";
                break;
        }
    }
}
