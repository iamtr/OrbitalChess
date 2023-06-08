using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
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
				int pos = y * 8 + x;
				Move m = new Move(CurrPos, pos, this);
				if (!IsLegalMove(m) || BoardController.i.IsBeingCheckedAfterMove(m)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos)) break;
			}
		}

		GetMovesFromDirection(1, 1, 8);
		GetMovesFromDirection(-1, 1, 8);
		GetMovesFromDirection(1, -1, 8);
		GetMovesFromDirection(-1, -1, 8);
		GetMovesFromDirection(1, 0, 8); // Right
		GetMovesFromDirection(-1, 0, 8); // Left
		GetMovesFromDirection(0, 1, 8); // Up
		GetMovesFromDirection(0, -1, 8); // Down

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
				int pos = y * 8 + x;
				Move m = new Move(CurrPos, pos, this);
				if (!IsLegalMove(m)) break;
				moves.Add(m);
				if (BoardController.i.TestArrayIsOccupied(pos)) break;
			}
		}

		GetMovesFromDirection(1, 1, 8);
		GetMovesFromDirection(-1, 1, 8);
		GetMovesFromDirection(1, -1, 8);
		GetMovesFromDirection(-1, -1, 8);
		GetMovesFromDirection(1, 0, 8); // Right
		GetMovesFromDirection(-1, 0, 8); // Left
		GetMovesFromDirection(0, 1, 8); // Up
		GetMovesFromDirection(0, -1, 8); // Down

		return moves;
	}

	public override bool IsLegalMove(Move move)
	{
		if (BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}	
}

