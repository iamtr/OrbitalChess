using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorOptionDropdown : MonoBehaviour
{
    public void Dropdown(int index)
    {
        switch (index)
        {
            case 0:
                Piece.isBlackBelow = true;
                BoardController.isBlackBelow = true;
                break;
            case 1:
                Piece.isBlackBelow = false;
                BoardController.isBlackBelow = false;
                break;
        }
    }
}
