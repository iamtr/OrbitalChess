using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System;

public class MultiplayerBoardController : BoardController
{
	protected PhotonView pv;
	protected PlayerManager playerManager;

	[SerializeField] protected SpecialPlayerManager blackPlayer;
	[SerializeField] protected SpecialPlayerManager whitePlayer;

	public List<Card> allMultiplayerCards { get; private set; }

	public override void Start()
	{
		hm = FindObjectOfType<HighlightManager>();
		um = FindObjectOfType<UIManager>();
		gc = FindObjectOfType<MultiplayerGameController>();
		pv = GetComponent<PhotonView>();

		pieceTransform = GameObject.Find("Pieces")?.transform;
		allMoves = new List<Move>();
		playerManager = FindObjectOfType<PlayerManager>();
		InstantiateMines();
		AssertAllReferenceIsNotNull();
	}

	public override Piece InstantiatePiece(Piece piece, int pos)
	{
		Piece p = base.InstantiatePiece(piece, pos);
		if (playerManager.Player == PlayerType.White) RotatePiece(p);
		return p;
	}

	public override void InstantiatePieces()
	{
		pv.RPC(nameof(RPC_InstantiatePieces), RpcTarget.All);
	}

	public override void MovePiece(int x, int y, Piece piece)
	{
		int oldPos = CurrPiece.CurrPos;
		int newPos = ConvPos(x, y);
		if (piece == null) Debug.Log("RPC_MovePiece: Piece is null!");
		pv.RPC(nameof(RPC_SetPiecePos), RpcTarget.All, new object[] { oldPos, newPos });

		if (piece is Pawn pawn)
		{
			SetPawnBooleanToMoved(CurrPiece.CurrPos);
			SetPawnBooleanToTwoStep(CurrPiece.CurrPos);
		}

		//if (gc.IsSpecialMode) TriggerMine(newPos);
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

	public override void SyncCurrCard(Card card)
	{
		int player = card.player == PlayerType.Black ? 0 : 1;
		int currIndex = card.currIndex;
		pv.RPC(nameof(RPC_SyncCurrCard), RpcTarget.All, player, currIndex);
	}

	public override void DistributeRandomCard(SpecialPlayerManager player)
	{
		if (GameController.GetCurrPlayer() != playerManager.Player) return;

		int rand = UnityEngine.Random.Range(0, cards.Length);
		pv.RPC(nameof(RPC_DistributeRandomCard), RpcTarget.All, rand);
	}

	public override void DestroyCurrentCard()
	{
		pv.RPC(nameof(RPC_DestroyCurrentCard), RpcTarget.All);
	}

	public override void Bomb(int pos)
	{
		pv.RPC(nameof(RPC_Bomb), RpcTarget.All, pos);
	}

	public override void PlantMine(int pos)
	{
		pv.RPC(nameof(RPC_PlantMine), RpcTarget.All, pos);
	}

	public override void TriggerMine(int pos)
	{
		pv.RPC(nameof(RPC_TriggerMine), RpcTarget.All, pos);
	}

	public override void RandomizeAllPieces()
	{
		foreach (Piece piece in pieces)
		{
			// Only 50% chance of randomizing a piece
			bool coinflip = UnityEngine.Random.Range(0, 2) == 0;
			if (piece == null || piece is King || coinflip) continue;

			int rand = UnityEngine.Random.Range(0, 16);
			PlayerType p = piece.Player;

			Piece newPiece;

			if (rand == 0) newPiece = GetPromotionPiece(0, p);
			else if (rand == 1 || rand == 2 || rand == 7) newPiece = GetPromotionPiece(1, p);
			else if (rand == 3 || rand == 4) newPiece = GetPromotionPiece(2, p);
			else if (rand == 5 || rand == 6) newPiece = GetPromotionPiece(3, p);
			else newPiece = GetPromotionPiece(4, p);

			pv.RPC(nameof(RPC_DestroyPiece), RpcTarget.All, piece.CurrPos);
			InstantiatePiece(newPiece, piece.CurrPos);
		}

		pv.RPC(nameof(RPC_CloneTestArray), RpcTarget.All);
	}

	//public override void DestroyPiece(int pos)
	//{
	//	Debug.Log("RPC Destroy Piece");
	//	pv.RPC(nameof(RPC_DestroyPiece), RpcTarget., pos);
	//}

	//public override Piece InstantiatePiece(Piece newPiece, int pos)
	//{
	//	int player = newPiece.Player == PlayerType.Black ? 0 : 1;
	//	int pieceType = -1;
	//	Type t = newPiece?.GetType();

	//	if (t == typeof(Queen)) pieceType = 0;
	//	else if (t == typeof(Knight)) pieceType = 1;
	//	else if (t == typeof(Rook)) pieceType = 2;
	//	else if (t == typeof(Bishop)) pieceType = 3;
	//	else if (t == typeof(Pawn)) pieceType = 4;
	//	else if (t == typeof(King)) pieceType = 5;

	//	pv.RPC(nameof(RPC_InstantiatePiece), RpcTarget.All, pieceType, player, pos);

	//	return newPiece;
	//}

	public override void StealOpponentPiece(int pos)
	{
		pv.RPC(nameof(RPC_StealOpponentPiece), RpcTarget.All, pos);
	}

	public override void SetDoubleTurn(bool boolean)
	{
		pv.RPC(nameof(RPC_SetDoubleTurn), RpcTarget.All, boolean);
	}

	[PunRPC]
	public void RPC_InstantiatePieces()
	{
		for (var i = 0; i < 64; i++)
		{
			if (pieces[i] != null)
			{
				Piece p = base.InstantiatePiece(pieces[i], i);
				if (playerManager.Player == PlayerType.White) RotatePiece(p);
			}
		}


		// RotateAllPieces();
		testArray = pieces.Clone() as Piece[];
	}

	[PunRPC]
	public void RPC_DestroyPiece(int pos)
	{
		//Debug.Log("RPC Destroy Piece Executing");
		base.DestroyPiece(pos);
	}

	[PunRPC]
	public void RPC_InstantiatePiece(int pieceType, int playerType, int pos)
	{
		PlayerType player = playerType == 0 ? PlayerType.Black : PlayerType.White;
		Piece piece = GetPromotionPiece(pieceType, player);
		if (playerManager.Player == PlayerType.White)
		{
			Debug.Log("Player Manager White: ROtate piece");
			RotatePiece(piece);
		}
		base.InstantiatePiece(piece, pos);
	}

	[PunRPC]
	public void RPC_CloneTestArray()
	{
		testArray = pieces.Clone() as Piece[];
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

	#region Special Moves

	[PunRPC]
	public void RPC_SyncCurrCard(int player, int index)
	{
		if (player == 0)
		{
			currCard = blackPlayer.PlayerCards[index];
		}
		else if (player == 1)
		{
			currCard = whitePlayer.PlayerCards[index];
		}
	}


	[PunRPC]
	public void RPC_DistributeRandomCard(int cardIndex)
	{
		Card card  = cards[cardIndex];
		PlayerType p = GameController.GetCurrPlayer();

		if (p == PlayerType.Black)
			blackPlayer.AddCard(card);
		else if (p == PlayerType.White)
			whitePlayer.AddCard(card);
	}

	[PunRPC]
	public void RPC_DestroyCurrentCard()
	{
		base.DestroyCurrentCard();
	}

	[PunRPC]
	public void RPC_Bomb(int pos)
	{
		base.Bomb(pos);
	}

	[PunRPC]
	public void RPC_PlantMine(int pos)
	{
		base.PlantMine(pos);
	}

	[PunRPC]
	public void RPC_TriggerMine(int pos)
	{
		base.TriggerMine(pos);
	}

	[PunRPC]
	public void RPC_StealOpponentPiece(int pos)
	{
		base.StealOpponentPiece(pos);
	}

	[PunRPC]
	public void RPC_SetDoubleTurn(bool boolean)
	{
		gc.IsDoubleTurn = true;
	}

	#endregion
}
