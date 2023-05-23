using UnityEngine;

public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] pieces;
	[SerializeField] private HighlightSquare[] highlights;
	[SerializeField] private HighlightSquare highlightSquare;
	[SerializeField] public Piece CurrPiece { get; set; }

	private Transform highlightTransform;
	private Transform pieceTransform;
	private int[] newXY;

	public static BoardController i { get; private set; }

	private void Start()
	{
		highlightTransform = GameObject.Find("Highlight Squares").transform;
		pieceTransform = GameObject.Find("Pieces").transform;

		if (i != null && i != this) Destroy(this);
		else i = this;
		
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
		int x = ConvertToXY(pos)[0];
		int y = ConvertToXY(pos)[1];

		Piece newPiece = Instantiate(piece, new Vector3(x, y, 0), Quaternion.identity);
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
		int pos = ConvertToPos(x, y);
		if (pieces[pos] == null)
			SetHighlightColor(pos, Color.blue);
		else if (pieces[pos]?.Player != currPiece.Player) 
			SetHighlightColor(pos, Color.red);
	}

	public void MovePiece(int x, int y, Piece piece)
	{

		int newPos = ConvertToPos(x, y);
		int oldPos = piece.CurrPos;

		piece.InvokeOnBeforeMove(oldPos);
		piece.SetCoords(x, y);
		DestroyOpponentPiece(piece, newPos);
		SetPiecePos(piece, newPos);
		RemovePiece(oldPos);
		piece.InvokeOnAfterMove(newPos);
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
		Destroy(pieces[pos]?.gameObject);
	}

	public void DestroyOpponentPiece(Piece piece, int pos)
	{
		if (pieces[pos] != null && pieces[pos].Player != piece.Player) 
			Destroy(pieces[pos].gameObject);
	}

	public Piece[] GetPieces()
	{
		return pieces;
	}

	public void HandleHighlightSquareClicked(Collider2D col)
	{
		var h = col.GetComponent<HighlightSquare>();
		var temp = ConvertToXY(h.Position);
		MovePiece(temp[0], temp[1], CurrPiece);
		UnhighlightAllSqaures();
	}

	public void HandlePieceClicked(Collider2D col)
	{
		UnhighlightAllSqaures();
		PawnPromotion.i.UnhighlightAllPromotingButtons();
		CurrPiece = col.GetComponent<Piece>();
		CurrPiece.GetAvailableMoves();
	}
}