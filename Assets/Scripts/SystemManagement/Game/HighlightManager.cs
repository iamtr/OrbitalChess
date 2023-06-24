using UnityEngine;

/// <summary>
/// Handles all highlighting behaviour
/// </summary>
public class HighlightManager : MonoBehaviour
{
	public static HighlightManager i;

	/// <summary>
	/// Array of the highlights on the board where
	/// the index of the array is the position of the square on the board
	/// </summary>
	[SerializeField] protected HighlightSquare[] highlights;

	[SerializeField] protected HighlightSquare highlightSquare;

	public static bool isBlackBelow = true;

	private void Awake()
	{
		if (i != null && i != this) Destroy(this);
		else i = this;
	}

	private void Start()
	{
		InstantiateHighlights();
	}

	public void InstantiateHighlights()
	{
		highlights = new HighlightSquare[64];

		Transform highlightTransform = GameObject.Find("Highlight Squares")?.transform;

		for (int i = 0; i < 64; i++)
		{
			var x = i % 8;
			var y = i / 8;

			if (!isBlackBelow)
			{
				x = 7 - x;
				y = 7 - y;
			}

			highlights[i] = Instantiate(highlightSquare, new Vector3(x, y, 0), Quaternion.identity);
			highlights[i].Position = i;
			highlights[i].transform.parent = highlightTransform;
			highlights[i].gameObject.SetActive(false);
		}
	}

	public virtual void HandleHighlightSquareClicked(Collider2D col)
	{
		var h = col.GetComponent<HighlightSquare>();
		var temp = BoardController.ConvXY(h.Position);
		BoardController.i.CurrPiece?.InvokeOnBeforeMove();

		if (h.Special == SpecialMove.Play && BoardController.i.CurrPiece is Pawn pawn)
		{
			pawn.SetTwoStepMove(temp[1]);
		}
		if (h.Special == SpecialMove.EnPassant)
		{
			BoardController.i.MoveEnPassantPiece(temp[0], temp[1], BoardController.i.CurrPiece);
		}
		if (h.Special == SpecialMove.Castling)
		{
			BoardController.i.MoveCastling(temp[0], temp[1], BoardController.i.CurrPiece);
		}
		if (h.Special == SpecialMove.Play)
		{
			BoardController.i.MovePiece(temp[0], temp[1], BoardController.i.CurrPiece);
		}

		// Below are special moves, they return early to prevent execution of unwanted code
		if (h.Special == SpecialMove.Bomb)
		{
			BoardController.i.Bomb(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Steal)
		{
			BoardController.i.StealOpponentPiece(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Spawn)
		{
			UIManager.i.DisableBuyOptions();
			BoardController.i.BuyPiece(BoardController.i.pieceToInstantiate);
			BoardController.i.PlaceBoughtPiece(h.Position);
			BoardController.i.DisableAllUIElements();
			return;
		}
		if (h.Special == SpecialMove.Mine)
		{
			BoardController.i.PlantMine(h.Position);
		}

		BoardController.i.SetHighLightSpecial(h, SpecialMove.Play);
		BoardController.i.DisableAllUIElements();
		BoardController.i.CurrPiece?.InvokeOnAfterMove();
	}

	/// <summary>
	/// Set the highlight color and activate the highlight to be active
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="color"></param>
	public void SetHighlightColor(int pos, Color color)
	{
		highlights[pos].GetComponent<SpriteRenderer>().color = color;
		highlights[pos].gameObject.SetActive(true);
	}

	public void SetHighlightSpecial(int pos, SpecialMove sp)
	{
		highlights[pos].Special = sp;
	}

	public void Highlight(int pos, SpecialMove sp)
	{
		SetHighlightColor(pos, Color.magenta);
		SetHighlightSpecial(pos, sp);
	}

	/// <summary>
	/// Highlights a certain square on the board
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="currPiece">The current piece chosen by player</param>
	public void Highlight(Move move)
	{
		int pos = move.TargetSquare;
		int flag = move.MoveFlag;

		switch (flag)
		{
			case Move.Flag.Castling:

				if (pos == 1 || pos == 57)
				{
					SetHighlightSpecial(pos - 1, SpecialMove.Castling);
					SetHighlightColor(pos - 1, Color.green);
				}
				else if (pos == 5 || pos == 61)
				{
					SetHighlightSpecial(pos + 2, SpecialMove.Castling);
					SetHighlightColor(pos + 2, Color.green);
				}
				else
				{
					Debug.Log("Error");
				}
				break;

			case Move.Flag.EnPassantCapture:
				SetHighlightSpecial(pos, SpecialMove.EnPassant);
				SetHighlightColor(pos, Color.yellow);
				break;

			default:
				if (BoardController.i.Pieces[pos] == null)
				{
					SetHighlightSpecial(pos, SpecialMove.Play);
					SetHighlightColor(pos, Color.blue);
				}
				else if (BoardController.i.Pieces[pos]?.Player != BoardController.i.CurrPiece.Player)
				{
					SetHighlightSpecial(pos, SpecialMove.Play);
					SetHighlightColor(pos, Color.red);
				}

				break;
		}
	}

	public void UnhighlightAllSquares()
	{
		foreach (var square in highlights) square.gameObject.SetActive(false);
	}

	public void HighlightSpawnPiece(Piece piece)
	{
		for (int i = 0; i < 16; i++)
		{
			if (GameController.GetCurrPlayer() == PlayerType.Black)
			{
				if (BoardController.i.Pieces[i] != null) continue;
				Highlight(i, SpecialMove.Spawn);
			}
			else
			{
				if (BoardController.i.Pieces[63 - i] != null) continue;
				Highlight(63 - i, SpecialMove.Spawn);
			}
		}
	}

	public void HighlightPawnBombs()
	{
		foreach (Piece piece in BoardController.i.Pieces)
		{
			if (piece?.Player == GameController.GetCurrPlayer() && piece is Pawn pawn)
			{
				Highlight(pawn.CurrPos, SpecialMove.Bomb);
			}
		}
	}

	public void HighlightSteal()
	{
		foreach (Piece piece in BoardController.i.Pieces)
		{
			if (piece == null) continue;
			if (piece.Player == GameController.GetCurrPlayer()) continue;
			Highlight(piece.CurrPos, SpecialMove.Steal);
		}
	}

	public void HighlightPlantMinePositions()
	{
		for (int i = 16; i < 48; i++)
		{
			if (BoardController.i.Pieces[i] != null) continue;
			Highlight(i, SpecialMove.Mine);
		}
	}
}