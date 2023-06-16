using System;
using UnityEngine;

public class AdvancedBoardController : BoardController
{
	private Transform mineTransform;
	[SerializeField] private GameObject mine;

	[SerializeField] private GameObject[] mines;
	protected override void InstantiatePieces()
	{
		base.InstantiatePieces();

		mines = new GameObject[64];

		for (int i = 0; i < 64; i++)
		{
			var x = i % 8;
			var y = i / 8;

			mines[i] = Instantiate(mine, new Vector3(x, y, 0), Quaternion.identity);
			mines[i].transform.parent = mineTransform;
			// mines[i].gameObject.SetActive(false);
		}
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
		if (h.Special == SpecialMove.Bomb)
		{
			Bomb(h.Position);
		}
		if (h.Special == SpecialMove.Steal)
		{
			StealOpponentPiece(h.Position);
		}

		SetHighLightSpecial(h, SpecialMove.Play);
		UnhighlightAllSqaures();
		CurrPiece?.InvokeOnAfterMove();
	}
	public void Bomb(int pos)
	{
		if (pos < 0 || pos > 63) Debug.Log("Bomb: pos out of range");
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				int x = ConvXY(pos)[0] + i;
				int y = ConvXY(pos)[1] + j;
				if (!IsInBounds(x, y)) continue;
				DestroyPiece(ConvPos(x, y));
			}
		}

		testArray = pieces.Clone() as Piece[];
	}
	public void HighlightPawnBombs()
	{
		foreach (Piece piece in pieces)
		{
			if (piece?.Player == GameController.GetCurrPlayer() && piece is Pawn pawn)
			{
				Highlight(pawn.CurrPos, SpecialMove.Bomb);
			}
		}
	}

	/// <summary>
	/// Special Game Mode: Randomizes pieces on the board for both sides
	/// </summary>
	public void RandomizeAllPieces()
	{
		foreach (Piece piece in pieces)
		{
			if (piece == null) continue;
			int rand = UnityEngine.Random.Range(0, 16);
			PlayerType p = piece.Player;
			if (piece is King) continue;

			Piece newPiece;

			if (rand == 0) newPiece = GetPromotionPiece(0, p);
			else if (rand == 1 || rand == 2 || rand == 7) newPiece = GetPromotionPiece(1, p);
			else if (rand == 3 || rand == 4) newPiece = GetPromotionPiece(2, p);
			else if (rand == 5 || rand == 6) newPiece = GetPromotionPiece(3, p);
			else newPiece = GetPromotionPiece(4, p);

			DestroyPiece(piece.CurrPos);
			InstantiatePiece(newPiece, piece.CurrPos);
		}

		testArray = pieces.Clone() as Piece[];
	}
	public void HighlightSteal()
	{
		foreach (Piece piece in pieces)
		{
			if (piece == null) continue;
			if (piece.Player == GameController.GetCurrPlayer()) continue;
			Highlight(piece.CurrPos, SpecialMove.Steal);
		}
	}

	/// <summary>
	/// Special Game Mode: Steal an opponent piece, excluding king and queen
	/// </summary>
	/// <param name="p"></param>
	/// <param name="pos"></param>
	public void StealOpponentPiece(int pos)
	{
		Piece stealPiece = GetPieceFromPos(pos);

		if (stealPiece == null) Debug.Log("Piece trying to steal is null!");
		if (stealPiece?.Player == GameController.GetCurrPlayer()) Debug.Log("Cannot steal your own piece!");

		DestroyPiece(pos);
		Type t = stealPiece.GetType();
		Piece[] temp = GameController.GetCurrPlayer() == PlayerType.White ? whitePieces : blackPieces;

		if (t == typeof(King)) Debug.Log("Cannot steal a king!");
		else if (t == typeof(Queen)) InstantiatePiece(temp[0], pos);
		else if (t == typeof(Knight)) InstantiatePiece(temp[1], pos);
		else if (t == typeof(Rook)) InstantiatePiece(temp[2], pos);
		else if (t == typeof(Bishop)) InstantiatePiece(temp[3], pos);
		else if (t == typeof(Pawn)) InstantiatePiece(temp[4], pos);
		else Debug.Log("StealOpponentPiece: Piece type not found");
	}
	public void PlantMine(int pos)
	{
		if (mines[pos])
		{
			Debug.Log("Already has mine!");
			return;
		}

		if (pieces[pos] != null)
		{
			Debug.Log("Piece exists here");
			return;
		}
	}

	public override void MovePiece(int x, int y, Piece piece)
	{
		int newPos = ConvPos(x, y);
		if (piece == null) Debug.Log("Piece at MovePiece() is null! Tried to move a null piece.");
		SetPiecePos(piece.CurrPos, newPos);
	}
}
