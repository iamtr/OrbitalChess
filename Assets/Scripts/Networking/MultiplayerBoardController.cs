using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerBoardController : BoardController
{
	protected PhotonView pv;
	protected PlayerManager playerManager;

	public override void Start()
	{
		hm = FindObjectOfType<HighlightManager>();
		um = FindObjectOfType<UIManager>();
		gc = FindObjectOfType<GameController>();
		pv = GetComponent<PhotonView>();

		pieceTransform = GameObject.Find("Pieces")?.transform;
		allMoves = new List<Move>();
		playerManager = FindObjectOfType<PlayerManager>();

		AssertAllReferenceIsNotNull();	
	}

	public override Piece InstantiatePiece(Piece piece, int pos)
	{
		Piece p = base.InstantiatePiece(piece, pos);
		if (playerManager.Player == PlayerType.White) RotatePiece(p);
		return p;
	}

	//public override void HandleHighlightSquareClicked(Collider2D col)
	//{
	//	var h = col.GetComponent<HighlightSquare>();
	//	var temp = ConvXY(h.Position);
	//	CurrPiece?.InvokeOnBeforeMove();

	//	if (h.Special == SpecialMove.Play && CurrPiece is Pawn pawn)
	//	{
	//		SetPawnBooleanToTwoStep(pawn.CurrPos);
	//	}
	//	if (h.Special == SpecialMove.EnPassant)
	//	{
	//		MoveEnPassantPiece(temp[0], temp[1], CurrPiece);
	//	}
	//	if (h.Special == SpecialMove.Castling)
	//	{
	//		MoveCastling(temp[0], temp[1], CurrPiece);
	//	}
	//	if (h.Special == SpecialMove.Play)
	//	{
	//		MovePiece(temp[0], temp[1], CurrPiece);
	//	}

	//	// Below are special moves, they return early to prevent execution of unwanted code
	//	if (h.Special == SpecialMove.Bomb)
	//	{
	//		Bomb(h.Position);
	//		GameController.InvokeOnRoundEnd();
	//		return;
	//	}
	//	if (h.Special == SpecialMove.Burgle)
	//	{
	//		StealOpponentPiece(h.Position);
	//		GameController.InvokeOnRoundEnd();
	//		return;
	//	}
	//	if (h.Special == SpecialMove.Spawn)
	//	{
	//		um.DisableBuyOptions();
	//		BuyPiece(pieceToInstantiate);
	//		PlaceBoughtPiece(h.Position);
	//		DisableAllUIElements();
	//		return;
	//	}
	//	if (h.Special == SpecialMove.Mine)
	//	{
	//		PlantMine(h.Position);
	//	}

	//	SetHighLightSpecial(h, SpecialMove.Play);
	//	DisableAllUIElements();
	//	CurrPiece?.InvokeOnAfterMove();
	//}

	public override void MovePiece(int x, int y, Piece piece)
	{
		int oldPos = piece.CurrPos;
		int newPos = ConvPos(x, y);
		if (piece == null) Debug.Log("RPC_MovePiece: Piece is null!");
		pv.RPC(nameof(RPC_SetPiecePos), RpcTarget.All, new object[] { oldPos, newPos });

		if (piece is Pawn pawn)
		{
			SetPawnBooleanToMoved(piece.CurrPos);
		}
	}

	public override void HandlePieceClicked(Collider2D col)
	{
		base.HandlePieceClicked(col);
		Piece p = col.GetComponent<Piece>();
		SyncCurrPiece(p.CurrPos);
	}

	public override void SetPawnBooleansToFalse()
	{
		pv.RPC(nameof(RPC_SetPawnBooleansToFalse), RpcTarget.All);
	}

	public override void SetPawnBooleanToMoved(int pos)
	{
		pv.RPC(nameof(RPC_SetPawnBooleanToMoved), RpcTarget.All, pos);
	}

	public override void SetPawnBooleanToTwoStep(int pos)
	{
		pv.RPC(nameof(RPC_SetPawnBooleanToTwoStep), RpcTarget.All, pos);
	}

	public override void MoveEnPassantPiece(int x, int y, Piece piece)
	{
		pv.RPC(nameof(RPC_MoveEnPassantPiece), RpcTarget.All, new object[] { x, y, piece.CurrPos });
	}

	public override void SyncCurrPiece(int piecePos)
	{
		pv.RPC(nameof(RPC_SyncCurrPiece), RpcTarget.All, piecePos);	
	}

	[PunRPC]
	public void RPC_SetPiecePos(int oldPos, int newPos)
	{
		SetPiecePos(oldPos, newPos);
	}

	[PunRPC]
	public void RPC_SetPawnBooleansToFalse()
	{
		base.SetPawnBooleansToFalse();
	}

	[PunRPC]
	public void RPC_SetPawnBooleanToMoved(int pos)
	{
		base.SetPawnBooleanToMoved(pos);
	}

	[PunRPC]
	public void RPC_SetPawnBooleanToTwoStep(int pos)
	{
		base.SetPawnBooleanToTwoStep(pos);
	}

	[PunRPC]
	public void RPC_MoveEnPassantPiece(int x, int y, int piecePos)
	{
		base.MoveEnPassantPiece(x, y, GetPieceFromPos(piecePos));
	}

	[PunRPC]
	public void RPC_SyncCurrPiece(int piecePos)
	{
		base.SyncCurrPiece(piecePos);
	}
}
