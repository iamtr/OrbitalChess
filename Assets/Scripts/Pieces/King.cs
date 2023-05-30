using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
	/// <summary>
	/// A boolean to check whether the pawn has moved from its initial position
	/// The boolean is set to false by default
	/// </summary>
	private bool hasMoved = false;

	public override void InitPiece(PlayerType p)
	{
		base.InitPiece(p);
		OnAfterMove += SetKingBoolean;
	}

	public override void GetAvailableMoves()
	{
		void HighlightDirection(int dx, int dy, int maxDistance)
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

		HighlightDirection(1, 1, 1);
		HighlightDirection(-1, 1, 1);
		HighlightDirection(1, -1, 1);
		HighlightDirection(-1, -1, 1);
		HighlightDirection(1, 0, 1); // Right
		HighlightDirection(-1, 0, 1); // Left
		HighlightDirection(0, 1, 1); // Up
		HighlightDirection(0, -1, 1); // Down

		HighlightCastling();
	}

	/// <summary>
	/// If available and legal, highlights the castling of king and rook
	/// </summary>
	public void HighlightCastling()
	{
		int leftDirection = -1;
		int rightDirection = 1;
		if (IsAbleToCastling(leftDirection))
        {
			int pos = BoardController.ConvertToPos(0, currY);
			BoardController.i.SetHighlightColor(pos, Color.green);
		}
			
		if (IsAbleToCastling(rightDirection))
		{
			int pos = BoardController.ConvertToPos(7, currY);
			BoardController.i.SetHighlightColor(pos, Color.green);
		}
	}

	public bool IsAbleToCastling(int direction)
	{
		if (hasMoved) return false;
		int x = currX;
		Piece foundPiece;
		while (true)
		{
			x += direction;
			int pos = BoardController.ConvertToPos(x, currY);
			if (!BoardController.IsInBounds(x, currY)) return false;
			Piece piece = BoardController.i.GetPieceFromPos(pos);
			if (piece != null)
			{
				foundPiece = piece;
				break;
			}
		}
		if (foundPiece is Rook rook)
		{
			return !rook.IsMoved();
		}
		return false;
	}

	public override bool IsLegalMove(int x, int y, Piece p)
	{
		int pos = y * 8 + x;
		if (!BoardController.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Sets the hasMoved boolean in its inital move
	/// </summary>
	public void SetKingBoolean()
	{
		hasMoved = true;
	}
}
