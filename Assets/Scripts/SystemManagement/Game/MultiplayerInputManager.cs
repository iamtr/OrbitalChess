using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerInputManager : InputManager
{
	protected PlayerManager player;
	private PhotonView pv;

	protected override void Start()
	{
		bc = FindObjectOfType<MultiplayerBoardController>();
		gc = FindObjectOfType<MultiplayerGameController>();
		hm = FindObjectOfType<HighlightManager>();
		player = FindObjectOfType<PlayerManager>();
	}

	public override void HandleColliderClicked(Collider2D col)
	{
		if (col == null)
		{
			bc.DisableAllUIElements();
		}
		else if (col.gameObject.CompareTag("Highlight Square"))
		{
			hm.HandleHighlightSquareClicked(col);
		}
		else if (col.gameObject.CompareTag("Piece")
			&& col.GetComponent<Piece>().Player == GameController.GetCurrPlayer()
			&& GameController.GetGameState() == GameState.Play
			&& player.Player == GameController.GetCurrPlayer())
		{
			bc.HandlePieceClicked(col);
		}
		else if (col.gameObject.CompareTag("Promotion Button") 
			&& GameController.GetGameState() == GameState.Promoting 
			&& player.Player == GameController.GetCurrPlayer())
		{
			bc.HandlePromotionButtonClicked(col);
		}
		else if (col.gameObject.CompareTag("Buy Option")
			&& player.Player == GameController.GetCurrPlayer())
		{
			// Cannot buy pieces if is in check
			if (gc.IsCheck) return;

			if (gc.GetCurrPlayerManager().Money < col.GetComponent<Piece>().Value)
			{
				Debug.Log("Not Enough Money!");
				return;
			}

			Piece piece = col.gameObject.GetComponent<Piece>();
			bc.SetPieceToInstantiate(piece);
			hm.HighlightSpawnPiece(piece);
		}
		else if (col.gameObject.CompareTag("Card")
			&& player.Player == GameController.GetCurrPlayer()
			&& col.GetComponent<Card>().player == GameController.GetCurrPlayer()
			&& GameController.GetGameState() == GameState.Play) 
		{
			bc.SyncCurrCard(col.GetComponent<Card>());	
			col.GetComponent<Card>().Trigger();	
		}
	}
}
