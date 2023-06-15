using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
	private int[,] deltas = new int[,] { { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 }, { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 } };

	private void OnEnable()
	{
		OnAfterMove += GameController.InvokeOnRoundEnd;
	}
	private void OnDisable()
	{
		OnAfterMove -= GameController.InvokeOnRoundEnd;
	}

	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];
			if (currX + deltaX < 0 || currY + deltaY < 0 || currX + deltaX > 7 || currY + deltaY > 7) continue;
			int newPos = BoardController.i.ConvPos(currX + deltaX, currY + deltaY);

			Move m = new Move(CurrPos, newPos, this);

			if (IsLegalMove(m) && !BoardController.i.IsBeingCheckedAfterMove(m, Player))
			{
				moves.Add(m);
			}
		}

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		moves.Clear();

		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];
			if (currX + deltaX < 0 || currY + deltaY < 0 || currX + deltaX > 7 || currY + deltaY > 7) continue;
			int newPos = BoardController.i.ConvPos(currX + deltaX, currY + deltaY);

			Move m = new Move(CurrPos, newPos, this);

			if (IsLegalMove(m)) moves.Add(m);
		}

		return moves;
	}


	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}
}
