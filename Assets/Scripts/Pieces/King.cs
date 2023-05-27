using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
	private bool hasMoved = false;

	public override void InitPiece(PlayerType p)
	{
		base.InitPiece(p);
		OnAfterMove += SetKingBoolean;
	}

	public override void GetAvailableMoves()
	{
		void HighlightDirection(BoardController bc, int currX, int currY, int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				int pos = y * 8 + x;
				if (!IsLegalMove(x, y, this)) break;
				bc.Highlight(x, y, this);
				if (bc.IsOccupied(pos) && !bc.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		HighlightDirection(bc, currX, currY, 1, 1, 1);
		HighlightDirection(bc, currX, currY, -1, 1, 1);
		HighlightDirection(bc, currX, currY, 1, -1, 1);
		HighlightDirection(bc, currX, currY, -1, -1, 1);
		HighlightDirection(bc, currX, currY, 1, 0, 1); // Right
		HighlightDirection(bc, currX, currY, -1, 0, 1); // Left
		HighlightDirection(bc, currX, currY, 0, 1, 1); // Up
		HighlightDirection(bc, currX, currY, 0, -1, 1); // Down
		HighlightCastling();
	}

	public void HighlightCastling()
	{
		if (hasMoved) return;
		int leftDirection = -1;
		int rightDirection = 1;
		int x = currX;
		if (Castling.i.IsAbleToCastling(currX, currY, leftDirection))
        {
			int pos = BoardController.i.ConvertToPos(0, currY);
			BoardController.i.SetHighlightColor(pos, Color.green);
		}
			
		if (Castling.i.IsAbleToCastling(currX, currY, rightDirection))
		{
			int pos = BoardController.i.ConvertToPos(7, currY);
			BoardController.i.SetHighlightColor(pos, Color.green);
		}
	}

	public override bool IsLegalMove(int x, int y, Piece p)
	{
		int pos = y * 8 + x;
		if (!bc.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos))
		{
			return false;
		}

		return true;
	}

	public void SetKingBoolean()
	{
		hasMoved = true;
	}
}
