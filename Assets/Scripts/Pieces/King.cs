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

	public override List<Move> GetLegalMoves()
	{
		List<Move> moves = new List<Move>();

		List<Move> GetMovesFromDirection(int currX, int currY, int dx, int dy, int maxDistance)
		{
			List<Move> m = new List<Move>();	

			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				int pos = y * 8 + x;
				if (!IsLegalMove(x, y, this)) break;
				m.Add(new Move(CurrPos, pos, this));
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}

			return m;
		}

		moves.AddRange(GetMovesFromDirection(currX, currY, 1, 1, 1));
		moves.AddRange(GetMovesFromDirection(currX, currY, -1, 1, 1));
		moves.AddRange(GetMovesFromDirection(currX, currY, 1, -1, 1));
		moves.AddRange(GetMovesFromDirection(currX, currY, -1, -1, 1));
		moves.AddRange(GetMovesFromDirection(currX, currY, 1, 0, 1)); // Right
		moves.AddRange(GetMovesFromDirection(currX, currY, -1, 0, 1)); // Left
		moves.AddRange(GetMovesFromDirection(currX, currY, 0, 1, 1)); // Up
		moves.AddRange(GetMovesFromDirection(currX, currY, 0, -1, 1)); // Down
		moves.AddRange(GetCastlingMoves());

		return moves;
	}

	public List<Move> GetCastlingMoves()
	{
		List<Move> moves = new List<Move>();

		if (hasMoved) return moves;
		int leftDirection = -1;
		int rightDirection = 1;
		if (IsAbleToCastling(leftDirection))
        {
			int pos = BoardController.i.ConvertToPos(0, currY);
			moves.Add(new Move(CurrPos, pos, this, Move.Flag.Castling));
			// TODO: Highlight
			// BoardController.i.SetHighlightColor(pos, Color.green);
		}
			
		if (IsAbleToCastling(rightDirection))
		{
			int pos = BoardController.i.ConvertToPos(7, currY);
			moves.Add(new Move(CurrPos, pos, this, Move.Flag.Castling));
			// TODO: Highlight
			// BoardController.i.SetHighlightColor(pos, Color.green);
		}

		return moves;
	}

	public bool IsAbleToCastling(int direction)
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
		if (foundPiece is Rook rook)
		{
			return !rook.IsMoved();
		}
		return false;
	}

	public override bool IsLegalMove(int x, int y, Piece p)
	{
		int pos = y * 8 + x;
		if (!BoardController.i.IsInBounds(x, y) || BoardController.i.IsSamePlayer(this.CurrPos, pos))
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
