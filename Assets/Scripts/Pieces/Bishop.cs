using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
	public override List<int> GetAvailableMoves()
	{
		void HighlightDirection(int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				int pos = bc.ConvertToPos(x, y);
				if (!IsLegalMove(x, y, this)) break;
				bc.Highlight(x, y, this);
				if (bc.IsOccupied(pos) && !bc.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		HighlightDirection(1, 1, 8); 
		HighlightDirection(-1, 1, 8); 
		HighlightDirection(1, -1, 8); 
		HighlightDirection(-1, -1, 8);

		return null;
	}

	public override bool IsLegalMove(int x, int y, Piece p)
	{
		int pos = bc.ConvertToPos(x, y);
		if (!bc.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos))
		{
			return false;
		}

		return true;
	}
}
