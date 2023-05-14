using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
	public override void GetAvailableMoves()
	{
		int deltaX = 0;
		int deltaY = 1;

		if (Player == PlayerType.Black)
		{
			if (IsLegalMove(currX + deltaX, currY + deltaY, this))
			{
				bc.Highlight(currX + deltaX, currY + deltaY, this);
			}
		}
		else if (Player == PlayerType.White)
		{
			if (IsLegalMove(currX - deltaX, currY - deltaY, this))
			{
				bc.Highlight(currX - deltaX, currY - deltaY, this);
			}
		}
	}

	public override bool IsLegalMove(int x, int y, Piece p)
	{
		int pos = y * 8 + x;
		if (!bc.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos) || bc.IsOccupied(pos))
		{
			return false;
		}

		return true;
	}
}

