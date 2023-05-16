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
		int deltaX = 0;
		int deltaY = 1;

		if (Player == PlayerType.Black)
		{
			int newX = currX + deltaX;
			int newY = currY + deltaY;

			if (IsLegalMove(newX, newY, this))
			{
				bc.Highlight(newX, newY, this);
			}

			if (!hasMoved)
			{
				if (IsLegalMove(newX, newY + 1, this) && !bc.IsOccupied(bc.ConvertToPos(newX, newY + 1)))
				{
					bc.Highlight(newX, newY + 1, this);
				}
			}

			if (IsLegalMove(currX + 1, currY + 1, this) && bc.GetPieceFromPos(bc.ConvertToPos(currX + 1, currY + 1)) != null) 
			{
				if (bc.GetPieceFromPos(bc.ConvertToPos(currX + 1, currY + 1)).Player != this.Player)
				{
					bc.Highlight(currX + 1, currY + 1, this);
				}
			}

			if (IsLegalMove(currX - 1, currY + 1, this) && bc.GetPieceFromPos(bc.ConvertToPos(currX - 1, currY + 1)) != null)
			{
				if (bc.GetPieceFromPos(bc.ConvertToPos(currX - 1, currY + 1)).Player != this.Player)
				{
					bc.Highlight(currX - 1, currY + 1, this);
				}
			}

			HighlightPawnDiagonals();
		}
		else if (Player == PlayerType.White)
		{
			int newX = currX - deltaX;
			int newY = currY - deltaY;

			if (IsLegalMove(newX, newY, this))
			{
				bc.Highlight(newX, newY, this);
			}

			if (!hasMoved)
			{
				if (IsLegalMove(newX, newY - 1, this ) && !bc.IsOccupied(bc.ConvertToPos(newX, newY - 1)))
				{
					bc.Highlight(newX, newY - 1, this);
				}
			}

			HighlightPawnDiagonals();
		}
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
	public void HighlightPawnDiagonals()
	{
		if (Player == PlayerType.Black)
		{
			if (bc.GetPieceFromPos(bc.ConvertToPos(currX + 1, currY + 1)) != null && bc.GetPieceFromPos(bc.ConvertToPos(currX + 1, currY + 1)).Player != this.Player && bc.IsLegalMove(currX + 1, currY + 1, this))
			{
				bc.Highlight(currX + 1, currY + 1, this);
			} 

			if (bc.GetPieceFromPos(bc.ConvertToPos(currX - 1, currY + 1)) != null && bc.GetPieceFromPos(bc.ConvertToPos(currX - 1, currY + 1)).Player != this.Player && bc.IsLegalMove(currX - 1, currY + 1, this))
			{
				bc.Highlight(currX - 1, currY + 1, this);
			}
		} 
		else if (Player == PlayerType.White)
		{
			if (bc.GetPieceFromPos(bc.ConvertToPos(currX + 1, currY - 1)) != null && bc.GetPieceFromPos(bc.ConvertToPos(currX + 1, currY - 1)).Player != this.Player && bc.IsLegalMove(currX + 1, currY - 1, this))
			{
				bc.Highlight(currX + 1, currY - 1, this);
			}

			if (bc.GetPieceFromPos(bc.ConvertToPos(currX - 1, currY - 1)) != null && bc.GetPieceFromPos(bc.ConvertToPos(currX - 1, currY - 1)).Player != this.Player && bc.IsLegalMove(currX - 1, currY - 1, this))
			{
				bc.Highlight(currX - 1, currY - 1, this);
			}
		}
	}
	public void SetPawnBoolean()
	{
		hasMoved = true;
	}
}

