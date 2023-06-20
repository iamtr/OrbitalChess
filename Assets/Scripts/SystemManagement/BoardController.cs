using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Controls all the piece logic and movement logic on the board
/// </summary>
public class BoardController : MonoBehaviour
{
	/// <summary>
	/// Array of the pieces available on the board where
	/// the index of the array is the position of the piece on the board
	/// </summary>
	[SerializeField] protected Piece[] pieces;
	[SerializeField] protected Card[] cards;
	/// <summary>
	/// Array of the highlights on the board where
	/// the index of the array is the position of the square on the board
	/// </summary>
	[SerializeField] protected HighlightSquare[] highlights;
	[SerializeField] protected HighlightSquare highlightSquare;

	/// <summary>
	/// Array of the piece used for pawn promotion
	/// </summary>
	[SerializeField] protected Piece[] blackPieces;
	[SerializeField] protected Piece[] whitePieces;



	private Transform highlightTransform;
	private Transform pieceTransform;

	/// <summary>
	/// Array that is used to simulated if a move results in a check to own king
	/// </summary>
	protected Piece[] testArray;

	[SerializeField] public int BlackKingPos = 3;
	[SerializeField] public int WhiteKingPos = 59;

	private List<Move> allMoves;

	public static bool isBlackBelow = true;

	[SerializeField] private bool isSpecialMode = true;
	public static event Action OnEnemyCaptured;

	/// <summary>
	/// The current piece that is being clicked by the player
	/// </summary>
	public Piece CurrPiece { get; set; }

	public static BoardController i { get; private set; }

	[SerializeField] private GameObject mine;
	[SerializeField] private GameObject[] mines;

	private void OnEnable()
	{
		GameController.OnRoundEnd += UnhighlightAllSqaures;
		GameController.OnRoundStart += SetPawnBooleansToFalse;
	}

	private void OnDisable()
	{
		GameController.OnRoundEnd -= UnhighlightAllSqaures;
		GameController.OnRoundStart -= SetPawnBooleansToFalse;
	}

	public virtual void Start()
	{
		highlightTransform = GameObject.Find("Highlight Squares")?.transform;
		pieceTransform = GameObject.Find("Pieces")?.transform;

		allMoves = new List<Move>();

		InstantiatePieces();

		if (i != null && i != this) Destroy(this);
		else i = this;

		testArray = pieces.Clone() as Piece[];
	}



	/// <summary>
	/// Instantiates all pieces and highlight squares
	/// </summary>
	protected virtual void InstantiatePieces()
	{
		highlights = new HighlightSquare[64];

		for (var i = 0; i < 64; i++)
		{
			int pos;
			if (isBlackBelow)
			{
				pos = i;
			}
			else
			{
				pos = 63 - i;
			}
			var x = pos % 8;
			var y = pos / 8;

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
		int x = ConvXY(pos)[0];
		int y = ConvXY(pos)[1];

		Piece newPiece = Instantiate(piece, new Vector3(x, y, 2), Quaternion.identity);
		pieces[pos] = newPiece;
		newPiece.transform.parent = pieceTransform;
		newPiece.SetCoords(pos);
		newPiece.SetTransform();

		return newPiece;
	}

	/// <summary>
	/// Check if a certain move is legal
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="p"></param>
	/// <returns></returns>
	public bool IsLegalMove(int x, int y, Piece p)
	{
		var pos = y * 8 + x;
		return IsInBounds(x, y) && pieces[pos]?.Player != p.Player;
	}

	/// <summary>
	/// Checks if a certain move is in bouds of the board
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
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
	}

	public void SetHighlightSpecial(int pos, SpecialMove sp)
	{
		highlights[pos].Special = sp;
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
				if (pieces[pos] == null)
				{
					SetHighlightSpecial(pos, SpecialMove.Play);
					SetHighlightColor(pos, Color.blue);
				}
					
				else if (pieces[pos]?.Player != CurrPiece.Player)
				{
					SetHighlightSpecial(pos, SpecialMove.Play);
					SetHighlightColor(pos, Color.red);
				}
					
				break;
		}
	}

	public void Highlight(int pos, SpecialMove sp)
	{
		SetHighlightColor(pos, Color.magenta);
		SetHighlightSpecial(pos, sp);
	}

	/// <summary>
	/// Sets the transform of piece at a certain position and also modifies the pieces[] array
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
			CapturePiece(newPos);
		}

		pieces[oldPos].SetCoords(newPos);
		pieces[oldPos].SetTransform();
		pieces[newPos] = pieces[oldPos];
		pieces[oldPos] = null;
	}

	/// <summary>
	/// Removes the piece at specified board position and destroys the gameobject
	/// </summary>
	/// <param name="pos"></param>
	public void CapturePiece(int pos)
	{
		if (pieces[pos] == null)
		{
			Debug.Log("Piece to capture is null!");
			return;
		}

		PlayerType p = pieces[pos].Player;
		Piece destroyedPiece = pieces[pos];
		DestroyPiece(pos);
		
		if (p != GameController.GetCurrPlayer())
		{
			HandleCapture(destroyedPiece);
		}
	}

	public void DestroyPiece(int pos)
	{
		if (pieces[pos] == null)
		{
			Debug.Log("Piece to destroy is null!");
			return;
		}

		Destroy(pieces[pos]?.gameObject);
		pieces[pos] = null;
	}

	public void HandleCapture(Piece capturedPiece)
	{
		GameController.i.GetCurrPlayerManager().AddMoney(capturedPiece.Value);
		DistributeRandomCard(GameController.i.GetCurrPlayerManager());
	}

	/// <summary>
	/// Moves a piece to position x, y on the board.
	/// </summary>
	/// <param name="currPiece">The current piece chosen by player</param>
	public virtual void MovePiece(int x, int y, Piece piece)
	{
		int newPos = ConvPos(x, y);
		if (piece == null) Debug.Log("Piece at MovePiece() is null! Tried to move a null piece.");
		SetPiecePos(piece.CurrPos, newPos);
		// For special game mode
		TriggerMine(newPos);
	}

	public void MoveEnPassantPiece(int x, int y, Piece piece)
	{
		int newPos = ConvPos(x, y);
		int enemyPos = ConvPos(x, ConvXY(piece.CurrPos)[1]);

		CapturePiece(enemyPos);
		SetPiecePos(piece.CurrPos, newPos);

		// For special game mode
		TriggerMine(newPos);
	}

	public void MoveCastling(int targetX, int targetY, Piece piece)
	{
		Piece rook = GetPieceFromPos(i.ConvPos(targetX, targetY));

		int oldX = ConvXY(piece.CurrPos)[0];
		int kingNewX = oldX - 2;
		int rookNewX = ConvXY(rook.CurrPos + 2)[0];

		if (targetX != 0)
		{
			kingNewX = oldX + 2;
			rookNewX = ConvXY(rook.CurrPos - 3)[0];
		}

		MovePiece(kingNewX, targetY, piece);
		// For special game mode
		TriggerMine(targetY);
		MovePiece(rookNewX, targetY, rook);
		// For special game mode
		TriggerMine(targetY);
	}

	/// <summary>
	/// Deactivates all squares on the board
	/// </summary>
	public void UnhighlightAllSqaures()
	{
		foreach (var square in highlights) square.gameObject.SetActive(false);
	}

	public void DisableBuyOption()
	{
		UIManager.i.whiteBuyOptions.SetActive(false);
		UIManager.i.blackBuyOptions.SetActive(false);
	}

	/// <summary>
	/// Converts x, y coordinates to 0 - 63
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public int ConvPos(int x, int y)
	{
		return y * 8 + x;
	}

	/// <summary>
	/// Converts 0 - 63 position numbers to x, y coordinates 
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public static int[] ConvXY(int pos)
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
		try
		{
			return pieces[pos];
		}
		catch (IndexOutOfRangeException e)
		{
			Debug.Log("Cannot get piece from position");
			return null;
		}
		
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
	public virtual void HandleHighlightSquareClicked(Collider2D col)
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

		// Below are special moves, they return early to prevent execution of unwanted code
		if (h.Special == SpecialMove.Bomb)
		{
			Bomb(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Steal)
		{
			StealOpponentPiece(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Spawn)
		{
			UIManager.i.whiteBuyOptions.SetActive(false);
			UIManager.i.blackBuyOptions.SetActive(false);
			BuyPiece(pieceToInstantiate);
			PlaceBoughtPiece(h.Position);
			GameController.InvokeOnRoundEnd();
			return;
		}
		if (h.Special == SpecialMove.Mine)
		{
			PlantMine(h.Position);
		}

		SetHighLightSpecial(h, SpecialMove.Play);
		UnhighlightAllSqaures();
		CurrPiece?.InvokeOnAfterMove();
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

		foreach (Move move in moves) Highlight(move);

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
		Piece promotedPiece = GetPromotionPiece(id, CurrPiece.Player);
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
		return player == PlayerType.Black ? blackPieces[id] : whitePieces[id];
	}

	/// <summary>
	/// Sets the "special" property of the highlight
	/// </summary>
	/// <param name="highlight"></param>
	/// <param name="specialMove"></param>
	public void SetHighLightSpecial(HighlightSquare highlight, SpecialMove specialMove)
	{
		highlight.Special = specialMove;
	}

	/// <summary>
	/// Sets the pawn JustMoved and TwoStep booleans to false
	/// </summary>
	public void SetPawnBooleansToFalse()
	{
		foreach (Piece piece in pieces)
		{
			if (piece?.Player == GameController.GetCurrPlayer() && piece is Pawn pawn)
			{
				pawn.JustMoved = false;
				pawn.TwoStep = false;
			}
		}
	}

	/// <summary>
	/// Checks if a certain move will result in a check to selfs
	/// </summary>
	/// <param name="move"></param>
	/// <returns></returns>
	public bool IsBeingCheckedAfterMove(Move move, PlayerType p)
	{
		UpdateTestArray(move, GameController.GetCurrPlayer());

		allMoves.Clear();

		int tempKingPos = -1;

		tempKingPos = move.Piece is King && move.Piece.Player == p
			? move.TargetSquare 
			: GetKingPosition(p);

		foreach (Piece piece in testArray)
		{
			if (piece == null || piece.Player == p) continue;
			allMoves.AddRange(piece.GetAllMoves());

		}

		bool temp = allMoves.Any(move => move.TargetSquare == tempKingPos);
		return temp;
	}

	/// <summary>
	/// Updates testArray, which is an array that is used to simulate if a move will result in a check for testing purposes
	/// </summary>
	/// <param name="move"></param>
	/// <param name="p"></param>
	public void UpdateTestArray(Move move, PlayerType p)
	{
		void MovePieceAndSetCoords(int from, int to)
		{
			testArray[to] = testArray[from];
			testArray[to]?.SetCoords(to);
			testArray[from] = null;
		}

		Array.Clear(testArray, 0, testArray.Length);
		for (int i = 0; i < pieces.Length; i++)
		{
			if (pieces[i] != null) testArray[i] = pieces[i].Clone() as Piece;
		}

		int oldPos = move.StartSquare;
		int newPos = move.TargetSquare;
		int flag = move.MoveFlag;

		switch (flag)
		{
			case Move.Flag.EnPassantCapture:
				int temp = p == PlayerType.Black ? newPos - 8 : newPos + 8;
				MovePieceAndSetCoords(oldPos, newPos);
				testArray[temp] = null;
				break;

			case Move.Flag.Castling:
				if (newPos == 1 || newPos == 57)
				{
					MovePieceAndSetCoords(oldPos, newPos);
					MovePieceAndSetCoords(newPos - 1, newPos + 1);
				}
				else if (newPos == 5 || newPos == 61)
				{
					MovePieceAndSetCoords(oldPos, newPos);
					MovePieceAndSetCoords(newPos + 2, newPos - 1);
				}
				else
				{
					Debug.Log("Error on PieceArrayAfterSimulatedMove");
				}
				break;

			default:
				if (testArray[oldPos] == null)
					Debug.Log($"UpdateTestArray: Piece at {oldPos} is null");

				MovePieceAndSetCoords(oldPos, newPos);
				break;
		}
	}

	public bool TestArrayIsOccupied(int pos)
	{
		return testArray[pos] != null;
	}

	public int GetKingPosition(PlayerType p)
	{
		return p == PlayerType.White ? WhiteKingPos : BlackKingPos;
	}

	public bool IsCheckmate()
	{
		// List of all opponent moves
		List<Move> moves = new List<Move>();

		foreach (Piece piece in pieces)
		{
			if (piece == null || piece.Player == GameController.GetCurrPlayer()) continue;
			moves.AddRange(piece.GetLegalMoves());
		}

		if (moves.Count == 0)
		{
			Debug.Log("Checkmate");
			return true;
		} 

		return false;
	}

	public bool IsCheck()
	{
		List<Move> moves = new List<Move>();

		foreach (Piece piece in pieces)
		{
			if (piece == null || piece.Player != GameController.GetCurrPlayer()) continue;
			moves.AddRange(piece.GetLegalMoves());
		}

		bool temp = moves.Any(move => move.TargetSquare == GetKingPosition(GameController.GetOpponent()));
		return temp;
	}

	public bool IsSamePlayerAtTestArray(int pos1, int pos2)
	{
		Piece p1 = testArray[pos1];
		Piece p2 = testArray[pos2];
		return p1?.Player == p2?.Player;
	}

	private Transform mineTransform;
	// For buying pieces:
	private Piece pieceToInstantiate;

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

	public void HighlightSpawnPiece(Piece piece)
	{
		pieceToInstantiate = piece;

		for (int i = 0; i < 16; i++)
		{
			if (GameController.GetCurrPlayer() == PlayerType.Black)
			{
				if (pieces[i] != null) continue;
				Highlight(i, SpecialMove.Spawn);
			}
			else
			{
				if (pieces[63 - i] != null) continue;
				Highlight(63 - i, SpecialMove.Spawn);
			}
		}
	}

	public void BuyPiece(Piece boughtPiece)
	{
		GameController.i.GetCurrPlayerManager().AddMoney(-boughtPiece.Value);
	}

	public void PlaceBoughtPiece(int pos)
	{
		var temp = InstantiatePiece(pieceToInstantiate, pos);
		temp.tag = "Piece";
	}

	public void HighlightPlantMinePositions()
	{
		for (int i = 16; i < 48; i++)
		{
			if (pieces[i] != null) continue;
			Highlight(i, SpecialMove.Mine);
		}
	}

	public void PlantMine(int pos)
	{
		if (mines[pos] != null)
		{
			Debug.Log("Already has mine!");
			return;
		}

		if (pieces[pos] != null)
		{
			Debug.Log("Piece exists here");
			return;
		}

		int x = ConvXY(pos)[0];
		int y = ConvXY(pos)[1];
		mines[pos] = Instantiate(mine, new Vector3(x, y, 2), Quaternion.identity);
	}

	public void TriggerMine(int pos)
	{
		if (mines[pos] == null)
		{
			Debug.Log("There is no mine here!");
			return;
		}

		Destroy(mines[pos]);
		DestroyPiece(pos);
	}

	public void DistributeRandomCard(PlayerManager player)
	{
		int rand = UnityEngine.Random.Range(0, cards.Length);
		Card card = Instantiate(cards[rand]);
		player.AddCard(card);
	}
}