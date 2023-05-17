using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private bool hasMoved = false;

    public override void InitPiece(PlayerType p)
    {
        base.InitPiece(p);
        OnMove += SetPawnBoolean;
    }

    public override void GetAvailableMoves()
    {
        int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        if (IsLegalMove(currX, newY, this))
        {
            bc.Highlight(currX, newY, this);

            if (!hasMoved && !bc.IsOccupied(bc.ConvertToPos(currX, newY + direction)))
            {
                bc.Highlight(currX, newY + direction, this);
            }
        }

        HighlightPawnDiagonals(direction);
    }

    public override bool IsLegalMove(int x, int y, Piece p)
    {
        int pos = y * 8 + x;
        if (!bc.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos) || bc.IsOccupied(bc.ConvertToPos(x, y)))
        {
            return false;
        }

        return true;
    }

    public void HighlightPawnDiagonals(int direction)
    {
        int rightX = currX + 1;
        int leftX = currX - 1;
        int newY = currY + direction;

        if (bc.GetPieceFromPos(bc.ConvertToPos(rightX, newY)) != null && bc.GetPieceFromPos(bc.ConvertToPos(rightX, newY)).Player != this.Player && bc.IsLegalMove(rightX, newY, this))
        {
            bc.Highlight(rightX, newY, this);
        }

        if (bc.GetPieceFromPos(bc.ConvertToPos(leftX, newY)) != null && bc.GetPieceFromPos(bc.ConvertToPos(leftX, newY)).Player != this.Player && bc.IsLegalMove(leftX, newY, this))
        {
            bc.Highlight(leftX, newY, this);
        }
    }

    public void SetPawnBoolean()
    {
        hasMoved = true;
    }
}
