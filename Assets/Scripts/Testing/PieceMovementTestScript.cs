using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PieceMovementTestScript : MonoBehaviour
{
	private BoardController bc;
	private HighlightManager hm;

	private void Start()
	{
		bc = FindObjectOfType<BoardController>();
		hm = FindObjectOfType<HighlightManager>();
	}

	public void Move_Pawn_SquaresHighlighted()
	{
		// Arrange
		bc.ResetPieces();

		// Act
		bc.HandlePieceClicked(bc.GetPieceFromPos(9).GetComponent<Collider2D>());

		// Assert
		Assert.IsTrue(hm.GetHighlightArray()[17].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[25].isActiveAndEnabled);
		Debug.Log("Pawn 1 move: " + hm.GetHighlightArray()[17].isActiveAndEnabled);
		Debug.Log("Pawn 2 move: " + hm.GetHighlightArray()[25].isActiveAndEnabled);
		bc.DisableAllUIElements();
	}

	public void Move_Knight_SquaresHighlighted()
	{
		bc.ResetPieces();
		bc.MovePiece(3, 4, bc.GetPieceFromPos(1));
		bc.HandlePieceClicked(bc.GetPieceFromPos(bc.ConvPos(3, 4)).GetComponent<Collider2D>());

		Assert.IsTrue(hm.GetHighlightArray()[25].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[18].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[29].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[20].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[52].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[45].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[50].isActiveAndEnabled);
		Assert.IsTrue(hm.GetHighlightArray()[41].isActiveAndEnabled);

		Debug.Log("Knight move (-1, -2): " + hm.GetHighlightArray()[25].isActiveAndEnabled);
		Debug.Log("Knight move (-2, -1): " + hm.GetHighlightArray()[18].isActiveAndEnabled);
		Debug.Log("Knight move (1, -2): " + hm.GetHighlightArray()[29].isActiveAndEnabled);
		Debug.Log("Knight move: (2, -1) " + hm.GetHighlightArray()[20].isActiveAndEnabled);
		Debug.Log("Knight move: (1, 2) " + hm.GetHighlightArray()[52].isActiveAndEnabled);
		Debug.Log("Knight move: (2, 1) " + hm.GetHighlightArray()[45].isActiveAndEnabled);
		Debug.Log("Knight move: (-1, 2) " + hm.GetHighlightArray()[50].isActiveAndEnabled);
		Debug.Log("Knight move: (-2, 1) " + hm.GetHighlightArray()[41].isActiveAndEnabled);

		bc.DisableAllUIElements();
	}

	public void Move_BlackPawnInFrontWhitePawn_UnableToMove()
	{
		bc.ResetPieces();
		bc.MovePiece(2, 2, bc.GetPieceFromPos(bc.ConvPos(0, 6)));
		bc.HandlePieceClicked(bc.GetPieceFromPos(bc.ConvPos(2, 1)).GetComponent<Collider2D>());

		bool test = !hm.GetHighlightArray()[bc.ConvPos(2, 3)].isActiveAndEnabled;

		Assert.IsTrue(test);
		Debug.Log("Unable to 2 move when opponent in front: " + test);
	}

	public void Move_KingsideCastling_SquaresHighlighted()
	{
		bc.ResetPieces();
		bc.DestroyPiece(1);
		bc.DestroyPiece(2);
		bc.HandlePieceClicked(bc.GetPieceFromPos(bc.ConvPos(3, 0)).GetComponent<Collider2D>());

		bool test = hm.GetHighlightArray()[0].isActiveAndEnabled;
		Assert.IsTrue(test);
		Debug.Log("Kingside castling: " + test);

		bc.DisableAllUIElements();
	}

	public void Move_QueensideCastling_SquaresHighlighted()
	{
		bc.ResetPieces();
		bc.DestroyPiece(4);
		bc.DestroyPiece(5);
		bc.DestroyPiece(6);
		bc.HandlePieceClicked(bc.GetPieceFromPos(bc.ConvPos(3, 0)).GetComponent<Collider2D>());

		bool test = hm.GetHighlightArray()[7].isActiveAndEnabled;
		Assert.IsTrue(test);
		Debug.Log("Queenside castling: " + test);

		bc.DisableAllUIElements();
	}

	public void Move_EnPassant_SquaresHighlighted()
	{
		bc.ResetPieces();
		bc.MovePiece(3, 3, bc.GetPieceFromPos(bc.ConvPos(3, 6)));
		bc.MovePiece(2, 3, bc.GetPieceFromPos(bc.ConvPos(2, 1)));
		Pawn pawn = bc.GetPieceFromPos(bc.ConvPos(2, 3)) as Pawn;
		pawn.HasMoved = true;
		pawn.JustMoved = true;
		pawn.TwoStep = true;

		bc.HandlePieceClicked(bc.GetPieceFromPos(bc.ConvPos(3, 3)).GetComponent<Collider2D>());

		bool test = hm.GetHighlightArray()[bc.ConvPos(2, 2)].isActiveAndEnabled;
		Assert.IsTrue(test);
		Debug.Log("En passant: " + test);

		bc.DisableAllUIElements();
	}

	public void TestAll()
	{
		Move_Pawn_SquaresHighlighted();
		Move_Knight_SquaresHighlighted();
		Move_BlackPawnInFrontWhitePawn_UnableToMove();
		Move_KingsideCastling_SquaresHighlighted();
		Move_QueensideCastling_SquaresHighlighted();
		Move_EnPassant_SquaresHighlighted();
	}
}
