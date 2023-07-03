using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerBoardController : BoardController
{
	[SerializeField] public PlayerManager localPlayer;
	[SerializeField] private PhotonView pv;

	public override void Start()
	{
		hm = FindObjectOfType<HighlightManager>();
		um = FindObjectOfType<UIManager>();
		gc = FindObjectOfType<GameController>();
		pv = GetComponent<PhotonView>();

		pieceTransform = GameObject.Find("Pieces")?.transform;
		allMoves = new List<Move>();

		// InstantiatePieces();
		// testArray = pieces.Clone() as Piece[];
		AssertAllReferenceIsNotNull();	
	}

	public override Piece InstantiatePiece(Piece piece, int pos)
	{
		Piece p = base.InstantiatePiece(piece, pos);
		if (localPlayer.Player == PlayerType.White) RotatePiece(p);
		return p;
	}

	public override void HandleHighlightSquareClicked(Collider2D col)
	{
		var h = col.GetComponent<HighlightSquare>();
		var temp = ConvXY(h.Position);
		CurrPiece?.InvokeOnBeforeMove();

		if (h.Special == SpecialMove.Play && CurrPiece is Pawn pawn)
		{
			pawn.SetTwoStepMove(temp[1]);
		}
		if (h.Special == SpecialMove.EnPassant)
		{
			MoveEnPassantPiece(temp[0], temp[1], CurrPiece);
		}
		if (h.Special == SpecialMove.Castling)
		{
			MoveCastling(temp[0], temp[1], CurrPiece);
		}
		if (h.Special == SpecialMove.Play)
		{
			MovePiece(temp[0], temp[1], CurrPiece);
		}

		// Below are special moves, they return early to prevent execution of unwanted code
		if (h.Special == SpecialMove.Bomb)
		{
			Bomb(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Steal)
		{
			StealOpponentPiece(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Spawn)
		{
			um.DisableBuyOptions();
			BuyPiece(pieceToInstantiate);
			PlaceBoughtPiece(h.Position);
			DisableAllUIElements();
			return;
		}
		if (h.Special == SpecialMove.Mine)
		{
			PlantMine(h.Position);
		}

		SetHighLightSpecial(h, SpecialMove.Play);
		DisableAllUIElements();
		CurrPiece?.InvokeOnAfterMove();
	}

	public override void MovePiece(int x, int y, Piece piece)
	{
		int oldPos = CurrPiece.CurrPos;
		int newPos = ConvPos(x, y);
		if (piece == null) Debug.Log("RPC_MovePiece: Piece is null!");
		pv.RPC(nameof(RPC_SetPiecePos), RpcTarget.All, new object[] { oldPos, newPos });
	}

	[PunRPC]
	public void RPC_SetPiecePos(int oldPos, int newPos)
	{
		SetPiecePos(oldPos, newPos);
	}
}
