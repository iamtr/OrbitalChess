using System;
using UnityEngine;


/// <summary>
/// Controls all the piece logic and movement logic on the board
/// </summary>
public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] pieces;
	[SerializeField] private HighlightSquare[] highlights;
	[SerializeField] private HighlightSquare highlightSquare;

	[SerializeField] private Piece[] promotionBlackList;
	[SerializeField] private Piece[] promotionWhiteList;

	//[SerializeField] private TurnCountdown[] turnCountdowns;
	//[SerializeField] private TurnCountdown TurnCountdown;

	private int id;
	private int numOfPawns = 16;

	private Transform TurnCountdownTransform;
	/// <summary>
	/// The current piece that is being clicked by the player
	/// </summary>
	public Piece CurrPiece { get; set; }

	private Transform highlightTransform;
	private Transform pieceTransform;

	public static BoardController i { get; private set; }

	private void OnEnable()
	{
		GameController.OnRoundEnd += UnhighlightAllSqaures;
		//GameController.OnRoundEnd += InvokeEveryTimer;
		GameController.OnRoundStart += SetJustMovedToFalse;
	}

	private void OnDisable()
	{
		GameController.OnRoundEnd -= UnhighlightAllSqaures;
		//GameController.OnRoundEnd -= InvokeEveryTimer;
		GameController.OnRoundStart -= SetJustMovedToFalse;
	}

	private void Start()
	{
		highlightTransform = GameObject.Find("Highlight Squares")?.transform;
		pieceTransform = GameObject.Find("Pieces")?.transform;
		TurnCountdownTransform = GameObject.Find("TurnCountdowns")?.transform;

		InstantiatePieces();

		if (i != null && i != this) Destroy(this);
		else i = this;
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

		Piece newPiece = Instantiate(piece, new Vector3(x, y, 2), Quaternion.identity);
		pieces[pos] = newPiece;
		newPiece.transform.parent = pieceTransform;
		newPiece.SetCoords(x, y);

		return newPiece;
	}

	//public TurnCountdown InstantiateTurnCountdown()
	//{
	//	if (id == numOfPawns)
	//	{
	//		id = 0;
	//	}
	//	TurnCountdown turnCountdown = Instantiate(TurnCountdown);
	//	turnCountdowns[id] = turnCountdown;
	//	turnCountdowns[id].transform.parent = TurnCountdownTransform;
	//	turnCountdowns[id].gameObject.SetActive(false);
	//	id += 1;
	//	return turnCountdown;
	//}

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
		if (color == Color.yellow) highlights[pos].Special = SpecialMove.EnPassant;
		if (color == Color.green) highlights[pos].Special = SpecialMove.Castling;
	}

	/// <summary>
	/// Highlights a certain sqaure on the board
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="currPiece">The current piece chosen by player</param>
	public void Highlight(int x, int y, Piece currPiece)
	{
		int pos = i.ConvertToPos(x, y);
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
		int newPos = i.ConvertToPos(x, y);
		int oldPos = piece.CurrPos;

		piece.InvokeOnBeforeMove();
		piece.SetCoords(x, y);
		DestroyOpponentPiece(piece, newPos);
		SetPiecePos(piece, newPos);
		pieces[oldPos] = null;
		piece.InvokeOnAfterMove();
	}

	/// <summary>
	/// Moves two pieces on the board simutaneously into their respectively positions.
	/// Acts similarly to MovePiece(int,int,Piece)
	/// </summary>
	/// <param name="piece1"></param>
	/// <param name="piece2"></param>
	public void MoveTwoPieceSimutaneously(int x1, int y1, Piece piece1, int x2, int y2, Piece piece2)
    {
		int newPos = i.ConvertToPos(x1, y1);
		int oldPos = piece1.CurrPos;

		piece1.InvokeOnBeforeMove();
		piece1.SetCoords(x1, y1);
		DestroyOpponentPiece(piece1, newPos);
		SetPiecePos(piece1, newPos);
		pieces[oldPos] = null;
		MovePiece(x2, y2, piece2);
	}

	public void MoveEnPassantPiece(int x, int y, Piece piece)
	{
		int newPos = i.ConvertToPos(x, y);
		int oldPos = piece.CurrPos;
		int enemyPos = i.ConvertToPos(x, ConvertToXY(oldPos)[1]);

		piece.InvokeOnBeforeMove();
		piece.SetCoords(x, y);
		DestroyOpponentPiece(piece, enemyPos);
		pieces[enemyPos] = null; 
		SetPiecePos(piece, newPos);
		pieces[oldPos] = null;
		piece.InvokeOnAfterMove();
	}

	public void MoveCastling(int x, int y, Piece piece)
	{
		Piece piece1 = GetPieceFromPos(i.ConvertToPos(x, y));
		int oldPos = piece.CurrPos;
		int[] oldXY = ConvertToXY(oldPos);
		int newX;
		int rookDirection;
		if (x == 0)
		{
			newX = oldXY[0] - 2;
			rookDirection = 1;
		}
		else
		{
			newX = oldXY[0] + 2;
			rookDirection = -1;
		}
		MoveTwoPieceSimutaneously(newX, y, piece, newX + rookDirection, y, piece1);
	}

	/// <summary>
	/// Unhighlights all squares on the board
	/// </summary>
	public void UnhighlightAllSqaures()
	{
		foreach (var square in highlights) square.gameObject.SetActive(false);
	}

	//public void InvokeEveryTimer()
	//{
	//	foreach (TurnCountdown timer in turnCountdowns)
	//	{
	//		timer.InvokeTimer();
	//	}
	//}

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
	public static int[] ConvertToXY(int pos)
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
		SetHighLightSpecial(h, SpecialMove.Play);
		// UnhighlightAllSqaures();
	}

	/// <summary>
	/// Handles the logic after a piece of the current player is clicked
	/// </summary>
	/// <param name="col"></param>
	public void HandlePieceClicked(Collider2D col)
	{
		UnhighlightAllSqaures();
		UIManager.i.UnhighlightAllPromotingButtons();
		CurrPiece = col.GetComponent<Piece>();
		CurrPiece.GetAvailableMoves();
	}

	/// <summary>
	/// Calls Promote() on the current piece
	/// </summary>
	/// <param name="promotedPiece">The piece type to be instantiated</param>
	public void PromotePiece(Piece promotedPiece)
	{
		try
		{
			IPromotable pawnToPromote = CurrPiece as IPromotable;
			pawnToPromote.Promote(promotedPiece);
		}
		catch (NullReferenceException)
		{
			Debug.Log("Tried to promote a non-promotable piece!");
		}
	}

	/// <summary>
	/// Handles the promotion button clicked functionality
	/// </summary>
	/// <param name="col"></param>
	public void HandlePromotionButtonClicked(Collider2D col)
	{
		int id = col.GetComponent<PromotionButton>().id;
		Piece promotedPiece = GetPromotionPiece(id, BoardController.i.CurrPiece.Player);
		PromotePiece(promotedPiece);
		UIManager.i.UnhighlightAllPromotingButtons();
		GameController.SetGameState(GameState.Play);
	}

	/// <summary>
	/// Gets the promoted piece type based on the id given
	/// </summary>
	/// <param name="id">The type of piece to be promoted</param>
	/// <param name="player">Player type</param>
	/// <returns>The promoted piece (Queen, Knight, Rook, Bishop)</returns>
	public Piece GetPromotionPiece(int id, PlayerType player)
	{
		return player == PlayerType.Black ? promotionBlackList[id] : promotionWhiteList[id];
	}

	public void SetHighLightSpecial(HighlightSquare highlight, SpecialMove specialMove)
	{
		highlight.Special = specialMove;
	}

	public void SetJustMovedToFalse()
	{
		foreach (Piece piece in pieces)
		{
			if (piece?.Player == GameController.GetCurrPlayer())
			{
				if (piece is Pawn pawn)
				{
					pawn.JustMoved = false;
					pawn.TwoStep = false;
				}
			}
		}
	}
}