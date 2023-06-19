using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public static InputManager i { get; private set; }

	private void Awake()
	{
		if (i != null && i != this) Destroy(this);
		else i = this;
	}

	/// <summary>
	/// Handles the mouse events (click)
	/// </summary>
	/// <param name="col"></param>
	public static void HandleColliderClicked(Collider2D col)
	{
		if (col == null)
		{
			BoardController.i.UnhighlightAllSqaures();
		}
		
		else if (col.gameObject.CompareTag("Highlight Square"))
		{
			BoardController.i.HandleHighlightSquareClicked(col);
		}

		else if (col.gameObject.CompareTag("Piece") 
			&& col.GetComponent<Piece>().Player == GameController.GetCurrPlayer()
			&& GameController.GetGameState() == GameState.Play)
		{
			BoardController.i.HandlePieceClicked(col);
		}

		else if (col.gameObject.CompareTag("Promotion Button") && GameController.GetGameState() == GameState.Promoting)
		{
			BoardController.i.HandlePromotionButtonClicked(col);
		}

		else if (col.gameObject.CompareTag("Buy Option"))
		{
			Piece piece = col.gameObject.GetComponent<Piece>();
			BoardController.i.HighlightSpawnPiece(piece);
		}

		else if (col.gameObject.CompareTag("Card"))
		{
			Card c = col.gameObject.GetComponent<Card>();
			c.Trigger();
		}
	}
}
