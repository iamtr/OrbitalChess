using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
	private int[,] deltas = new int[,] { { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 }, { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 } };

	public override bool IsLegalMove(int x, int y, Piece p) 
	{
		int pos = y * 8 + x;
		if (!BoardController.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos))
		{
			return false;
		}

		return true;

	}

	public override void GetAvailableMoves()
	{
		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];

			if (IsLegalMove(currX + deltaX, currY + deltaY, this))
			{
				bc.Highlight(currX + deltaX, currY + deltaY, this);
			}
		}
	}
}
