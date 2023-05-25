using UnityEngine;

public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] pieces;
	[SerializeField] private HighlightSquare[] highlights;
	[SerializeField] private HighlightSquare highlightSquare;
	/// <summary>
	/// The current piece that is being clicked by the player
	/// </summary>
	[SerializeField] public Piece CurrPiece { get; set; }

	private Transform highlightTransform;
	private Transform pieceTransform;

	public static BoardController i { get; private set; }

	private void Start()
	{
		highlightTransform = GameObject.Find("Highlight Squares").transform;
		pieceTransform = GameObject.Find("Pieces").transform;

		if (i != null && i != this) Destroy(this);
		else i = this;
		
		InstantiatePieces();
	}

	/// <summary>
	/// Instantiates all pieces and highlight squares
	/// </summary>
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

	/// <summary>
	/// Instantiate a single piece on the board
	/// </summary>
	/// <param name="piece">Type of piece to be instantiated</param>
	/// <param name="pos">Position on board to be instantiated</param>
	/// <returns></returns>
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

	public void SetHighlightColor(int pos, Color color)
	{
		highlights[pos].GetComponent<SpriteRenderer>().color = color;
		highlights[pos].gameObject.SetActive(true);
		if (color == Color.yellow) highlights[pos].Special = "EnPassant";
		if (color == Color.green) highlights[pos].Special = "Castling";
	}

	/// <summary>
	/// Highlights a certain sqaure on the board
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="currPiece">The current piece chosen by player</param>
	public void Highlight(int x, int y, Piece currPiece)
	{
		int pos = ConvertToPos(x, y);
		if (pieces[pos] == null)
			SetHighlightColor(pos, Color.blue);
		else if (pieces[pos]?.Player != currPiece.Player) 
			SetHighlightColor(pos, Color.red);
	}

	/// <summary>
	/// Moves a piece to position x, y on the board.
	/// Before move, InvokeOnBeforeMove() is called on the piece.
	/// After move, InvokeOnAfterMove() is called on the piece.
	/// </summary>
	/// <param name="currPiece">The current piece chosen by player</param>
	public void MovePiece(int x, int y, Piece piece)
	{
		int newPos = ConvertToPos(x, y);
		int oldPos = piece.CurrPos;

		piece.InvokeOnBeforeMove();
		piece.SetCoords(x, y);
		DestroyOpponentPiece(piece, newPos);
		SetPiecePos(piece, newPos);
		pieces[oldPos] = null;
		piece.InvokeOnAfterMove();
	}

    /// <summary>
    /// Unhighlights all squares on the board
    /// </summary>
    public void UnhighlightAllSqaures()
	{
		foreach (var square in highlights) square.gameObject.SetActive(false);
	}

	/// <summary>
	/// Converts x, y coordinates to 0 - 63
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public int ConvertToPos(int x, int y)
	{
		return y * 8 + x;
	}

	/// <summary>
	/// Converts 0 - 63 position numbers to x, y coordinates 
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public int[] ConvertToXY(int pos)
	{
		return new int[] { pos % 8, pos / 8 };
	}

	/// <summary>
	/// Checks if the piece at pos1 is the same player as the piece at pos2
	/// If any one of the pieces is null, return false
	/// </summary>
	public bool IsSamePlayer(int pos1, int pos2)
	{
		Piece p1 = GetPieceFromPos(pos1);
		Piece p2 = GetPieceFromPos(pos2);
		return p1?.Player == p2?.Player;
	}

	/// <summary>
	/// Returns the piece from a certain position on the board
	/// </summary>	
	public Piece GetPieceFromPos(int pos)
	{
		return pieces[pos];
	}

	/// <summary>
	/// Checks if a certain position on the board is occupied
	/// </summary>
	public bool IsOccupied(int pos)
	{
		return pieces[pos] != null;
	}

	/// <summary>
	/// Sets the piece at a certain position on the board
	/// Also calls SetCoords() on the piece
	/// </summary>
	/// <param name="piece">Piece to be moved</param>
	/// <param name="pos">New position on board</param>
	public void SetPiecePos(Piece piece, int pos)
	{
		pieces[pos] = piece;
		int x = ConvertToXY(pos)[0];
		int y = ConvertToXY(pos)[1];
		pieces[pos].SetCoords(x, y);
	}

	/// <summary>
	/// Removes the piece at specified board position and destroys the gameobject
	/// </summary>
	/// <param name="pos"></param>
	public void DestroyPiece(int pos)
	{
		Destroy(pieces[pos]?.gameObject);
		pieces[pos] = null;
	}

	/// <summary>
	/// Destroys the opponent piece at a certain position on the board, and destroys the gameobject
	/// </summary>
	/// <param name="piece"></param>
	/// <param name="pos"></param>
	public void DestroyOpponentPiece(Piece piece, int pos)
	{
		if (pieces[pos] != null && pieces[pos].Player != piece.Player)
			Destroy(pieces[pos].gameObject);
	}

	/// <summary>
	/// Handles the logic after a highlight square is clicked
	/// </summary>
	/// <param name="col"></param>
	public void HandleHighlightSquareClicked(Collider2D col)
	{
		var h = col.GetComponent<HighlightSquare>();
		var temp = ConvertToXY(h.Position);
        if (h.Special == "EnPassant")
        {
			EnPassant.i.MoveEnPassantPiece(temp[0], temp[1], CurrPiece);
			SetHighLightToPlay(h);
        }
		//else if (h.Special == "Castling")
        //{
			//SetHighLightToPlay(h);
        //}
		else
        {
			MovePiece(temp[0], temp[1], CurrPiece);
		}
		UnhighlightAllSqaures();
	}

	/// <summary>
	/// Handles the logic after a piece of the current player is clicked
	/// </summary>
	/// <param name="col"></param>
	public void HandlePieceClicked(Collider2D col)
	{
		UnhighlightAllSqaures();
		PawnPromotion.i.UnhighlightAllPromotingButtons();
		CurrPiece = col.GetComponent<Piece>();
		CurrPiece.GetAvailableMoves();
	}
	
	public void SetPieceNull(int pos)
    {
		pieces[pos] = null;
    }
	public void SetHighLightToPlay(HighlightSquare highlight)
	{
		highlight.Special = "Play";
	}
}