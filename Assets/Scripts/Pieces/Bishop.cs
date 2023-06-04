using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		void GetMovesFromDirection(int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				int pos = BoardController.i.ConvertToPos(x, y);
				Move m = new Move(CurrPos, pos, this);
				if (!IsLegalMove(m)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		GetMovesFromDirection(1, 1, 8);
		GetMovesFromDirection(1, -1, 8);
		GetMovesFromDirection(-1, 1, 8);
		GetMovesFromDirection(-1, -1, 8);

		return moves;
	}

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		if (BoardController.i.IsBeingCheckedAfterMove(move)) return false;
		return true;
	}
}
