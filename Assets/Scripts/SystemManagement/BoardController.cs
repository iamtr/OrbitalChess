using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Controls all the piece logic and movement logic on the board
/// </summary>
public class BoardController : MonoBehaviour
{
	/// <summary>
	/// Array of the pieces available on the board where
	/// the index of the array is the position of the piece on the board
	/// </summary>
	[SerializeField] private Piece[] pieces;

	/// <summary>
	/// Array of the highlights on the board where
	/// the index of the array is the position of the square on the board
	/// </summary>
	[SerializeField] private HighlightSquare[] highlights;
	[SerializeField] private HighlightSquare highlightSquare;

	/// <summary>
	/// Array of the piece used for pawn promotion
	/// </summary>
	[SerializeField] private Piece[] promotionBlackList;
	[SerializeField] private Piece[] promotionWhiteList;

	//[SerializeField] private TurnCountdown[] turnCountdowns;
	//[SerializeField] private TurnCountdown TurnCountdown;

	private int turnCountdownID;
	private int numOfPawns = 16;

	private Transform TurnCountdownTransform;
	private Transform highlightTransform;
	private Transform pieceTransform;

	public int BlackKingPos { get;  set; }	
	public int WhiteKingPos { get;  set; }

	/// <summary>
	/// The current piece that is being clicked by the player
	/// </summary>
	public Piece CurrPiece { get; set; }

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

	/// <summary>
	/// Set the highlight color and activate the highlight to be active
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="color"></param>
	public void SetHighlightColor(int pos, Color color)
	{
		highlights[pos].GetComponent<SpriteRenderer>().color = color;
		highlights[pos].gameObject.SetActive(true);
		if (color == Color.blue || color == Color.red) highlights[pos].Special = SpecialMove.Play;
		if (color == Color.yellow) highlights[pos].Special = SpecialMove.EnPassant;
		if (color == Color.green) highlights[pos].Special = SpecialMove.Castling;
	}

	/// <summary>
	/// Highlights a certain sqaure on the board
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
				SetHighlightColor(CurrPiece.CurrPos, Color.green);
				SetHighlightColor(pos, Color.green);
				break;
			case Move.Flag.EnPassantCapture:
				SetHighlightColor(pos, Color.yellow);
				break;
			default:
				if (pieces[pos] == null)
					SetHighlightColor(pos, Color.blue);
				else if (pieces[pos]?.Player != CurrPiece.Player)
					SetHighlightColor(pos, Color.red);
				break;
		}
	}

	/// <summary>
	/// Sets the piece at a certain position on the board
	/// Also calls SetCoords() on the piece
	/// </summary>
	/// <param name="piece">Piece to be moved</param>
	/// <param name="pos">New position on board</param>
	public void SetPiecePos(int oldPos, int newPos)
	{
		if (pieces[oldPos] == null)
		{
			Debug.Log("Piece at position is null");
			return;
		}

		if (pieces[newPos] != null)
		{
			Debug.Log("Destroy piece at index: " + newPos);
			DestroyPiece(newPos);
		}

		pieces[oldPos].SetCoords(newPos);
		pieces[newPos] = pieces[oldPos];
		pieces[oldPos] = null;
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
	/// Moves a piece to position x, y on the board.
	/// </summary>
	/// <param name="currPiece">The current piece chosen by player</param>
	public void MovePiece(int x, int y, Piece piece)
	{
		int newPos = i.ConvertToPos(x, y);
		if (piece == null) Debug.Log("Piece at MovePiece() is null! Tried to move a null piece.");
		SetPiecePos(piece.CurrPos, newPos);
	}

	public void MovePiece(Move move)
	{
		int oldPos = move.StartSquare;
		int newPos = move.TargetSquare;
		Piece piece = pieces[oldPos];

		if (piece == null) Debug.Log($"Piece at MovePiece() at {oldPos} is null! Tried to move a null piece.");
		SetPiecePos(piece.CurrPos, newPos);
	}


	public void MoveEnPassantPiece(int x, int y, Piece piece)
	{
		int newPos = i.ConvertToPos(x, y);
		int enemyPos = i.ConvertToPos(x, ConvertToXY(piece.CurrPos)[1]);

		DestroyPiece(enemyPos);
		SetPiecePos(piece.CurrPos, newPos);

	}

	public void MoveCastling(int x, int y, Piece piece)
	{
		Piece rook = GetPieceFromPos(i.ConvertToPos(x, y));
		PlayerType player = rook.Player;

		int oldX = ConvertToXY(piece.CurrPos)[0];
		int kingNewX;
		int rookNewX;

		if (x == 0)
		{
			if (player == PlayerType.Black)
			{
				kingNewX = oldX - 2;
				rookNewX = ConvertToXY(rook.CurrPos + 2)[0];
			} 
			else
			{
				kingNewX = oldX - 2;
				rookNewX = ConvertToXY(rook.CurrPos + 2)[0];
			}

		}
		else
		{
			if (player == PlayerType.Black)
			{
				kingNewX = oldX + 2;
				rookNewX = ConvertToXY(rook.CurrPos - 3)[0];
			}
			else
			{
				kingNewX = oldX + 2;
				rookNewX = ConvertToXY(rook.CurrPos - 3)[0];
			}
		}
		
		MovePiece(kingNewX, y, piece);
		MovePiece(rookNewX, y, rook);
	}

	/// <summary>
	/// Deactivates all squares on the board
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
	/// Handles the logic after a highlight square is clicked
	/// </summary>
	/// <param name="col"></param>
	public void HandleHighlightSquareClicked(Collider2D col)
	{
		var h = col.GetComponent<HighlightSquare>();
		var temp = ConvertToXY(h.Position);

		CurrPiece.InvokeOnBeforeMove();

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

		CurrPiece.InvokeOnAfterMove();
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
		List<Move> moves = CurrPiece.GetLegalMoves();

		foreach(Move move in moves) Highlight(move);
		
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

	public bool IsCheckAfterMove(PlayerType p)
	{
		List<Move> allMoves = new List<Move>();

		foreach(Piece piece in pieces)
		{
			if (piece.Player == p)
			{
				allMoves.AddRange(piece.GetLegalMoves());
			}
		}

		return allMoves.Any(move => move.TargetSquare == GetOpponentKingPosition());
	}

	public void SimulateMovePiece(Move move, PlayerType p)
	{
		int oldPos = move.StartSquare;
		int newPos = move.TargetSquare;
		Piece piece = pieces[oldPos];

		Piece[] testArray = pieces;

		if (piece == null) Debug.Log($"Piece at {oldPos} is null");

		piece.InvokeOnBeforeMove();
		piece.SetCoords(newPos);
		DestroyPiece(newPos);
		SetPiecePos(piece.CurrPos, newPos);
		pieces[oldPos] = null;
		piece.InvokeOnAfterMove();
	}

	public int GetOpponentKingPosition()
	{
		return GameController.GetCurrPlayer() == PlayerType.Black ? WhiteKingPos : BlackKingPos;
	}


}