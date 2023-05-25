using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castling : MonoBehaviour
{
    public static Castling i;

    private void Start()
    {
        if (i != null && i != this) Destroy(this);
        else i = this;
    }

    public bool IsAbleToCastling(int currX, int currY, int direction)
    {
        int x = currX;
        Piece foundPiece;
        while (true)
        {
            x += direction;
            int pos = BoardController.i.ConvertToPos(x, currY);
            if (!BoardController.i.IsInBounds(x, currY)) return false;
            Piece piece = BoardController.i.GetPieceFromPos(pos);
            if (piece != null)
            {
                foundPiece = piece; 
                break;
            }
        }
        if(foundPiece is Rook)
        {
            Rook rook = (Rook)foundPiece;
            return !rook.IsMoved();
        }
        return false;
    }

    public void MoveCastling(int x, int y, Piece piece)
    {
        int oldPos = piece.CurrPos;
        int[] oldXY = BoardController.i.ConvertToXY(oldPos);
        int newX;
        if (x == 0)
        {
            newX = oldXY[0] - 2;
        }
        else
        {
            newX = oldXY[0] + 2;
        }
        int newPos = BoardController.i.ConvertToPos(newX, y);
        
        piece.InvokeOnBeforeMove();
        piece.SetCoords(x, y);
        ////////
        BoardController.i.SetPiecePos(piece, newPos);
        BoardController.i.SetPieceNull(oldPos);
        piece.InvokeOnAfterMove();
    }
}
