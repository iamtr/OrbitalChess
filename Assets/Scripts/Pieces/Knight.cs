using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
	private int[,] deltas = new int[,] { { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 }, { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 } };

	public override bool IsLegalMove(int x, int y, Piece p) 
	{
		int pos = y * 8 + x;
		if (!BoardController.i.IsInBounds(x, y) || BoardController.i.IsSamePlayer(this.CurrPos, pos))
		{
			return false;
		}

		return true;

	}

	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];

			if (IsLegalMove(currX + deltaX, currY + deltaY, this))
			{
				// BoardController.i.Highlight(currX + deltaX, currY + deltaY, this);
				int newPos = BoardController.i.ConvertToPos(currX + deltaX, currY + deltaY);
				moves.Add(new Move(CurrPos, newPos, this));
			}
		}

		return moves;
	}
}
