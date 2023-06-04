using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		void GetMovesFromDirection(int currX, int currY, int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				int pos = y * 8 + x;
				Move m = new Move(CurrPos, pos, this);
				// if (!IsLegalMove(x, y, this)) break;
				if (!IsLegalMove(m)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		GetMovesFromDirection(currX, currY, 1, 1, 8);
		GetMovesFromDirection(currX, currY, -1, 1, 8);
		GetMovesFromDirection(currX, currY, 1, -1, 8);
		GetMovesFromDirection(currX, currY, -1, -1, 8);
		GetMovesFromDirection(currX, currY, 1, 0, 8); // Right
		GetMovesFromDirection(currX, currY, -1, 0, 8	); // Left
		GetMovesFromDirection(currX, currY, 0, 1, 8); // Up
		GetMovesFromDirection(currX, currY, 0, -1, 8); // Down

		return moves;
	}

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		if (BoardController.i.IsBeingCheckedAfterMove(move)) return false;
		return true;
	}	
}

