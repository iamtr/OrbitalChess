using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{

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

		void GetMovesFromDirection(int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				if (x < 0 || x > 7 || y < 0 || y > 7) break;
				int pos = BoardController.i.ConvertToPos(x, y);
				Move m = new Move(CurrPos, pos, this);
				if (BoardController.i.IsBeingCheckedAfterMove(m, Player)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos)) break;
			}
		}

		GetMovesFromDirection(1, 1, 8);
		GetMovesFromDirection(1, -1, 8);
		GetMovesFromDirection(-1, 1, 8);
		GetMovesFromDirection(-1, -1, 8);

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		moves.Clear();

		void GetMovesFromDirection(int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				if (x < 0 || x > 7 || y < 0 || y > 7) break;
				int pos = BoardController.i.ConvertToPos(x, y);
				Move m = new Move(CurrPos, pos, this);
				moves.Add(m);
				if (BoardController.i.TestArrayIsOccupied(pos)) break;
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
		if (BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}
}
