using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
	public override void InitPiece(PlayerType p)
	{
		delta = new int[1, 2] { { 0, 1 } };
		player = p;
	}
}

