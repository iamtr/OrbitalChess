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
		if (col.gameObject.CompareTag("Highlight Square"))
		{
			BoardController.i.HandleHighlightSquareClicked(col);
		}

		if (col.gameObject.CompareTag("Piece") 
			&& col.GetComponent<Piece>().Player == GameController.GetCurrPlayer()
			&& GameController.GetGameState() == GameState.Play)
		{
			BoardController.i.HandlePieceClicked(col);
		}

		if (col.gameObject.CompareTag("Promotion Button") && GameController.GetGameState() == GameState.Promoting)
		{
			BoardController.i.HandlePromotionButtonClicked(col);
		}
	}
}
