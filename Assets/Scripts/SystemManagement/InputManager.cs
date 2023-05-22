using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public static InputManager i { get; private set; }

	private void Start()
	{
		if (i != null && i != this) Destroy(this);
		else i = this;
	}

	public void HandleColliderClicked(Collider2D col)
	{
		if (col.gameObject.CompareTag("Highlight Square"))
		{
			var h = col.GetComponent<HighlightSquare>();
			var temp = BoardController.i.ConvertToXY(h.Position);
			BoardController.i.MovePiece(temp[0], temp[1], BoardController.i.CurrPiece);
			BoardController.i.UnhighlightAllSqaures();
		}

		if (col.gameObject.CompareTag("Piece") && col.GetComponent<Piece>().Player == GameController.i.CurrPlayer)
		{
			BoardController.i.UnhighlightAllSqaures();
			PawnPromotion.i.UnhighlightAllPromotingButtons();
			BoardController.i.CurrPiece = col.GetComponent<Piece>();
			BoardController.i.CurrPiece.GetAvailableMoves();
		}

		if (col.gameObject.CompareTag("Promotion Button"))
		{
			int id = col.GetComponent<PromotionButton>().id;
			Piece promotedPiece = PawnPromotion.i.FindPromotion(id, BoardController.i.CurrPiece.Player);
			PawnPromotion.i.PromotePiece(promotedPiece);
			PawnPromotion.i.UnhighlightAllPromotingButtons();
		}
	}
}
