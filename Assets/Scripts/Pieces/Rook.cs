using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
	private bool hasMoved = false;

	public override void InitPiece(PlayerType p)
	{
		base.InitPiece(p);
		OnAfterMove += SetRookBoolean;
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
				//BoardController.i.Highlight(x, y, this);
				m.Add(new Move(CurrPos, pos, this));
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}

			return m;
		}

		moves.AddRange(GetMovesFromDirection(currX, currY, 1, 0, 8));
		moves.AddRange(GetMovesFromDirection(currX, currY, -1, 0, 8));
		moves.AddRange(GetMovesFromDirection(currX, currY, 0, 1, 8));
		moves.AddRange(GetMovesFromDirection(currX, currY, 0, -1, 8));

		return moves;
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

	public bool IsMoved()
    {
		return hasMoved;
    }

	public void SetRookBoolean()
    {
		hasMoved = true;
	}
}
