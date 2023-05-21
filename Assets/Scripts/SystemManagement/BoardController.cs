using UnityEngine;

public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] pieces;
	[SerializeField] private HighlightSquare[] highlights;
	[SerializeField] private HighlightSquare highlightSquare;
	[SerializeField] private Piece currPiece;
	[SerializeField] private PawnPromotion pp;

	private GameController gc;
	private Transform highlightTransform;
	private Transform pieceTransform;
	private int[] newXY;

	private void Start()
	{
		gc = GameObject.Find("Game Controller").GetComponent<GameController>();
		highlightTransform = GameObject.Find("Highlight Squares").transform;
		pieceTransform = GameObject.Find("Pieces").transform;
		InstantiatePieces();
	}

	private void InstantiatePieces()
	{
		highlights = new HighlightSquare[64];

		for (var i = 0; i < 64; i++)
		{
			var x = i % 8;
			var y = i / 8;

			highlights[i] = Instantiate(highlightSquare, new Vector3(x, y, 0), Quaternion.identity);
			highlights[i].Position = i;
			highlights[i].transform.parent = highlightTransform;
			highlights[i].gameObject.SetActive(false);

			if (pieces[i] != null) InstantiatePiece(pieces[i], i);
		}
	}

	public Piece InstantiatePiece(Piece piece, int pos)
	{
		var x = ConvertToXY(pos)[0];
		var y = ConvertToXY(pos)[1];

		var newPiece = Instantiate(piece, new Vector3(x, y, 0), Quaternion.identity);
		pieces[pos] = newPiece;
		newPiece.transform.parent = pieceTransform;
		newPiece.SetCoords(x, y);

		return newPiece;
	}

	public bool IsLegalMove(int x, int y, Piece p)
	{
		var pos = y * 8 + x;
		return IsInBounds(x, y) && pieces[pos]?.Player != p.Player;
	}

	public bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < 8 && y >= 0 && y < 8;
	}

	private void SetHighlightColor(int pos, Color color)
	{
		highlights[pos].GetComponent<SpriteRenderer>().color = color;
		highlights[pos].gameObject.SetActive(true);
	}

	public void Highlight(int x, int y, Piece currPiece)
	{
		this.currPiece = currPiece;
		var pos = ConvertToPos(x, y);

		if (pieces[pos] == null)
			SetHighlightColor(pos, Color.blue);
		else if (pieces[pos]?.Player != currPiece.Player) SetHighlightColor(pos, Color.red);
	}

	public void MovePiece(int x, int y, Piece piece)
	{
		var newPos = ConvertToPos(x, y);
		var oldPos = piece.CurrPos;

		// RemovePiece(oldPos);
		piece.SetCoords(x, y);

		DestroyOpponentPiece(piece, newPos);

		SetPiecePos(piece, newPos);
		RemovePiece(oldPos);
		InvokeMove(newPos);

		gc.RoundEnd();
	}

	public void UnhighlightAllSqaures()
	{
		foreach (var square in highlights) square.gameObject.SetActive(false);
	}

	// Converts (x, y) coords to 0 - 63
	public int ConvertToPos(int x, int y)
	{
		return y * 8 + x;
	}

	// Converts 0 - 63 to (x, y)
	public int[] ConvertToXY(int pos)
	{
		return new int[] { pos % 8, pos / 8 };
	}

	public void HandleColliderClicked(Collider2D collider)
	{
		if (collider.gameObject.CompareTag("Highlight Square"))
		{
			var h = collider.GetComponent<HighlightSquare>();
			var temp = ConvertToXY(h.Position);
			newXY = temp;
			if (pp.IsPromoting(currPiece, temp[1]))
			{
				pp.ShowPromotion((Pawn)currPiece);
				UnhighlightAllSqaures();
			}
			else
			{
				MovePiece(temp[0], temp[1], currPiece);
				UnhighlightAllSqaures();
			}
		}

		if (collider.gameObject.CompareTag("Piece") && collider.GetComponent<Piece>().Player == gc.CurrPlayer)
		{
			UnhighlightAllSqaures();
			pp.UnhighlightAllPromotingButtons();
			currPiece = collider.GetComponent<Piece>();
			currPiece.GetAvailableMoves();
		}

		if (collider.gameObject.CompareTag("Promotion Button"))
		{
			var id = collider.GetComponent<PromotionButton>().id;
			var PromotedPiece = pp.FindPromotion(id, currPiece.Player);
			pp.MovePromotedPiece(newXY[0], newXY[1], currPiece, PromotedPiece);
			pp.UnhighlightAllPromotingButtons();
		}
	}

	public bool IsSamePlayer(int pos1, int pos2)
	{
		Piece p1 = GetPieceFromPos(pos1);
		Piece p2 = GetPieceFromPos(pos2);
		return p1?.Player == p2?.Player;
	}

	public Piece GetPieceFromPos(int pos)
	{
		return pieces[pos];
	}

	public bool IsOccupied(int pos)
	{
		return pieces[pos] != null;
	}

	public void SetPiecePos(Piece piece, int pos)
	{
		pieces[pos] = piece;
	}

	public void RemovePiece(int pos)
	{
		pieces[pos] = null;
	}

	public void DestroyOpponentPiece(Piece piece, int pos)
	{
		if (pieces[pos] != null && pieces[pos].Player != piece.Player) Destroy(pieces[pos].gameObject);
	}

	public void InvokeMove(int pos)
	{
		pieces[pos].OnMove?.Invoke();
	}

	public Piece[] GetPieces()
	{
		return pieces;
	}
}