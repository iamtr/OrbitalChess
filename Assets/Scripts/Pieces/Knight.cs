using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
	private int[,] deltas = new int[,] { { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 }, { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 } };

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		if (BoardController.i.IsBeingCheckedAfterMove(move)) return false;
		return true;
	}

	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];
			int newPos = BoardController.i.ConvertToPos(currX + deltaX, currY + deltaY);

			Move m = new Move(CurrPos, newPos, this);

			if (IsLegalMove(m))
			{
				moves.Add(m);
			}
		}

		return moves;
	}
}
