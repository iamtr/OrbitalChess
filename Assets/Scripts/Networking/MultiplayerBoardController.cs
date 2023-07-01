using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerBoardController : BoardController
{
	[SerializeField] private PlayerManager localPlayer;

	public override Piece InstantiatePiece(Piece piece, int pos)
	{
		Piece p = base.InstantiatePiece(piece, pos);
		if (localPlayer?.Player != PlayerType.Black) RotatePiece(p);
		return p;
	}
}
