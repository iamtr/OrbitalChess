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

	public void HandleColliderClicked(Collider2D col)
	{
		if (col.gameObject.CompareTag("Highlight Square"))
		{
			BoardController.i.HandleHighlightSquareClicked(col);
		}

		if (col.gameObject.CompareTag("Piece") 
			&& col.GetComponent<Piece>().Player == GameController.i.CurrPlayer
			&& GameController.i.GameState == GameState.Play)
		{
			BoardController.i.HandlePieceClicked(col);
		}

		if (col.gameObject.CompareTag("Promotion Button") && GameController.i.GameState == GameState.Promoting)
		{
			PawnPromotion.i.HandlePromotionButtonClicked(col);
		}
	}
}
