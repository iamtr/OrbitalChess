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
		else if (col.gameObject.CompareTag("Buy Option"))
		{
			// Cannot buy pieces if is in check
			if (gc.IsCheck) return;

			Piece piece = col.gameObject.GetComponent<Piece>();
			bc.SetPieceToInstantiate(piece);
			hm.HighlightSpawnPiece(piece);
		}
	}
}
