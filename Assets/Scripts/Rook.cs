using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
	public int[,] delta = new int[1, 2] {{0, 1}};

	public override void GetAvailableMoves()
	{
		for (int i = 0; i < delta.Length - 1; i++)
		{
			int x = delta[i, 0];
			int y = delta[i, 1];
			if (boardController.IsLegalMove(currX + x, currY + y))
			{
				Debug.Log("Legal");
				boardController.Highlight(currX + x, currY + y);
			}
		}

	}
}
