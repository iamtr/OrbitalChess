using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls all the piece logic and movement logic on the board
/// </summary>
public class BoardController : MonoBehaviour
{
	[Header("Board Setup")]
	/// <summary>
	/// Array of the pieces available on the board where
	/// the index of the array is the position of the piece on the board
	/// </summary>
	[SerializeField] protected Piece[] pieces;

	[SerializeField] protected Card[] cards;

	[Header("Promotion Buttons")]
	/// <summary>
	/// Array of the piece used for pawn promotion
	/// </summary>
	[SerializeField] protected Piece[] blackPieces;

	[SerializeField] protected Piece[] whitePieces;

	[Header("Special Mode")]
	[SerializeField] private GameObject mine;

	private GameObject[] mines;

	private Transform pieceTransform;

	/// <summary>
	/// Array that is used to simulated if a move results in a check to own king
	/// </summary>
	private Piece[] testArray;

	private int BlackKingPos = 3;
	private int WhiteKingPos = 59;
	private List<Move> allMoves;
	protected UIManager um;
	protected HighlightManager hm;
	protected GameController gc;

	public static bool isBlackBelow = true;

	public Piece[] Pieces => pieces;

	/// <summary>
	/// The current piece that is being clicked by the player
	/// </summary>
	public Piece CurrPiece { get; set; }

	// For buying pieces:
	public Piece pieceToInstantiate { get; private set; }

	private void OnEnable()
	{
		GameController.OnRoundEnd += DisableAllUIElements;
		GameController.OnRoundStart += SetPawnBooleansToFalse;
	}

	private void OnDisable()
	{
		GameController.OnRoundEnd -= DisableAllUIElements;
		GameController.OnRoundStart -= SetPawnBooleansToFalse;
	}

	public virtual void Start()
	{
		hm = FindObjectOfType<HighlightManager>();
		um = FindObjectOfType<UIManager>();
		gc = FindObjectOfType<GameController>();
		
		pieceTransform = GameObject.Find("Pieces")?.transform;

		allMoves = new List<Move>();

		InstantiatePieces();

		testArray = pieces.Clone() as Piece[];

		AssertAllReferenceIsNotNull();
	}

	private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(pieces);
		Assert.IsNotNull(blackPieces);
		Assert.IsNotNull(whitePieces);

		Assert.IsNotNull(pieceTransform);
	}

	/// <summary>
	/// Instantiates all pieces and highlight squares
	/// </summary>
	protected virtual void InstantiatePieces()
	{
		for (var i = 0; i < 64; i++)
		{
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

		if (piece == null) return null;

		Piece newPiece = Instantiate(piece, new Vector3(x, y, 2), Quaternion.identity);
		newPiece.transform.localScale = new Vector3(4.55f, 4.55f, 1);
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
	/// Checks if a certain move is in bounds of the board
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < 8 && y >= 0 && y < 8;
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
	/// Handles capture if in special mode
	/// </summary>
	/// <param name="pos"></param>
	public void CapturePiece(int pos)
	{
		if (pieces[pos] == null)
		{
			Debug.Log("Piece to capture is null!");
			return;
		}

		Piece destroyedPiece = pieces[pos];
		DestroyPiece(pos);
		if (gc.IsSpecialMode) HandleCapture(destroyedPiece);
	}

	/// <summary>
	/// Destroys the pieces and removes it from pieces[] array
	/// </summary>
	/// <param name="pos"></param>
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

	/// <summary>
	/// For special game mode, handle capture of piece
	/// </summary>
	/// <param name="capturedPiece"></param>
	public void HandleCapture(Piece capturedPiece)
	{
		if (gc.GetCurrPlayerManager() != null) gc.GetCurrPlayerManager().AddMoney(capturedPiece.Value * 2);
		DistributeRandomCard(gc.GetCurrPlayerManager());
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
		if (gc.IsSpecialMode) TriggerMine(newPos);
	}

	public void MoveEnPassantPiece(int x, int y, Piece piece)
	{
		int newPos = ConvPos(x, y);
		int enemyPos = ConvPos(x, ConvXY(piece.CurrPos)[1]);

		CapturePiece(enemyPos);
		SetPiecePos(piece.CurrPos, newPos);

		// For special game mode
		if (gc.IsSpecialMode) TriggerMine(newPos);
	}

	public void MoveCastling(int targetX, int targetY, Piece piece)
	{
		Piece rook = GetPieceFromPos(ConvPos(targetX, targetY));

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
		if (gc.IsSpecialMode) TriggerMine(targetY);
		MovePiece(rookNewX, targetY, rook);
		// For special game mode
		if (gc.IsSpecialMode) TriggerMine(targetY);
	}

	/// <summary>
	/// Deactivates all unnecessary UI elements
	/// </summary>
	public void DisableAllUIElements()
	{
		hm?.UnhighlightAllSquares();
		um?.DisableBuyOptions();
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
		catch (IndexOutOfRangeException)
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
	/// Updates the position of the king after move to the position specified
	/// </summary>
	/// <param name="p"></param>
	/// <param name="newPos"></param>
	public void UpdateKingPosition(PlayerType p, int newPos)
	{
		if (p == PlayerType.White)
			WhiteKingPos = newPos;
		else BlackKingPos = newPos;
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
			um.DisableBuyOptions();
			BuyPiece(pieceToInstantiate);
			PlaceBoughtPiece(h.Position);
			DisableAllUIElements();
			return;
		}
		if (h.Special == SpecialMove.Mine)
		{
			PlantMine(h.Position);
		}

		SetHighLightSpecial(h, SpecialMove.Play);
		DisableAllUIElements();
		CurrPiece?.InvokeOnAfterMove();
	}

	/// <summary>
	/// Handles the logic after a piece of the current player is clicked
	/// </summary>
	/// <param name="col"></param>
	public void HandlePieceClicked(Collider2D col)
	{
		DisableAllUIElements();
		um.UnhighlightAllPromotingButtons();
		CurrPiece = col.GetComponent<Piece>();
		List<Move> moves = CurrPiece.GetLegalMoves();

		foreach (Move move in moves) hm.Highlight(move);
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
			return;
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
		um.UnhighlightAllPromotingButtons();
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
		UpdateTestArrayForMove(move, GameController.GetCurrPlayer());

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
	/// Updates test array
	/// </summary>
	public void UpdateTestArray()
	{
		for (int i = 0; i < pieces.Length; i++)
		{
			if (pieces[i] != null) testArray[i] = pieces[i].Clone() as Piece;
		}
	}

	/// <summary>
	/// Updates testArray, and simulate if a move will result in a check for testing purposes
	/// </summary>
	/// <param name="move"></param>
	/// <param name="p"></param>
	public void UpdateTestArrayForMove(Move move, PlayerType p)
	{
		void MovePieceAndSetCoords(int from, int to)
		{
			testArray[to] = testArray[from];
			testArray[to]?.SetCoords(to);
			testArray[from] = null;
		}

		Array.Clear(testArray, 0, testArray.Length);
		UpdateTestArray();

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

	public void SetPieceToInstantiate(Piece piece)
	{
		pieceToInstantiate = piece;
	}

	// Special moves:

	/// <summary>
	/// Bombs a 3x3 area around a position
	/// </summary>
	/// <param name="pos"></param>
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

	/// <summary>
	/// Special Game Mode: Steal an opponent piece, excluding king and queen
	/// </summary>
	/// <param name="p"></param>
	/// <param name="pos"></param>
	public void StealOpponentPiece(int pos)
	{
        try
        {
            Piece stealPiece = GetPieceFromPos(pos);

            if (stealPiece == null) Debug.Log("Piece trying to steal is null!");
            if (stealPiece?.Player == GameController.GetCurrPlayer()) Debug.Log("Cannot steal your own piece!");

            DestroyPiece(pos);
            Type t = stealPiece?.GetType();
            Piece[] temp = GameController.GetCurrPlayer() == PlayerType.White ? whitePieces : blackPieces;

            if (t == typeof(King)) Debug.Log("Cannot steal a king!");
            else if (t == typeof(Queen)) InstantiatePiece(temp[0], pos);
            else if (t == typeof(Knight)) InstantiatePiece(temp[1], pos);
            else if (t == typeof(Rook)) InstantiatePiece(temp[2], pos);
            else if (t == typeof(Bishop)) InstantiatePiece(temp[3], pos);
            else if (t == typeof(Pawn)) InstantiatePiece(temp[4], pos);
            else Debug.Log("StealOpponentPiece: Piece type not found");
		}
        catch (NullReferenceException)
        {
            Debug.Log("Piece Type Not Found!");
        }
		
	}

	/// <summary>
	/// Special Game Mode: Buy a piece
	/// </summary>
	/// <param name="boughtPiece"></param>
	public void BuyPiece(Piece boughtPiece)
	{
		gc.GetCurrPlayerManager().AddMoney(-boughtPiece.Value);
	}

	public void PlaceBoughtPiece(int pos)
	{
		var temp = InstantiatePiece(pieceToInstantiate, pos);
		temp.tag = "Piece";
	}

	/// <summary>
	/// Special Game Mod: Plant a mine on the board
	/// </summary>
	/// <param name="pos"></param>
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

		if (!isBlackBelow)
		{
			x = 7 - x;
			y = 7 - y;
		}

		mines[pos] = Instantiate(mine, new Vector3(x, y, 2), Quaternion.identity);
	}

	/// <summary>
	/// Triggers a mine which will destroy the piece on it
	/// </summary>
	/// <param name="pos"></param>
	public void TriggerMine(int pos)
	{
		if (mines[pos] == null)
		{
			Debug.Log("There is no mine here!");
			return;
		}

		if (pieces[pos] is King)
		{
			Debug.Log("King triggers pawn, mine removed!");
			return;
		}

		Destroy(mines[pos]);
		DestroyPiece(pos);
	}

	/// <summary>
	/// Distribute a random card to a player (who has captured a piece)
	/// </summary>
	/// <param name="player"></param>
	public void DistributeRandomCard(PlayerManager player)
	{
		int rand = UnityEngine.Random.Range(0, cards.Length);
		Card card = cards[rand];
		player.AddCard(card);
	}

	public virtual void LoadPositionPresets(PositionSO preset)
	{
		UnloadAllPieces();

		for (int i = 0; i < 8; i++)
		{
			if (preset.rank1[i] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank1[i], i);
		}

		for (int i = 8; i < 16; i++)
		{
			if (preset.rank2[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank2[i % 8], i);
		}

		for (int i = 16; i < 24; i++)
		{
			if (preset.rank3[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank3[i % 8], i);
		}
		for (int i = 24; i < 32; i++)
		{
			if (preset.rank4[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank4[i % 8], i);
		}
		for (int i = 32; i < 40; i++)
		{
			if (preset.rank5[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank5[i % 8], i);
		}
		for (int i = 40; i < 48; i++)
		{
			if (preset.rank6[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank6[i % 8], i);
		}
		for (int i = 48; i < 56; i++)
		{
			if (preset.rank7[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank7[i % 8], i);
		}
		for (int i = 56; i < 64; i++)
		{
			if (preset.rank8[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank8[i % 8], i);
		}
	}

	public void UnloadAllPieces()
	{
		foreach(Piece piece in pieces) 
			if (piece != null) Destroy(piece.gameObject);
	}
}