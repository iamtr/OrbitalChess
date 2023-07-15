using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls all the king logic and movement logic on the board
/// </summary>
public class BoardController : MonoBehaviour
{
	[Header("Board Setup")]
	/// <summary>
	/// Array of the pieces available on the board where
	/// the index of the array is the position of the king on the board
	/// </summary>
	[SerializeField] protected Piece[] pieces;

	[SerializeField] protected Card[] cards;

	[Header("Promotion Buttons")]
	/// <summary>
	/// Array of the king used for pawn promotion
	/// </summary>
	[SerializeField] protected Piece[] blackPieces;
	
	[SerializeField] protected Piece[] whitePieces;

	[Header("Special Mode")]
	[SerializeField] private GameObject mine;

	[SerializeField] protected GameObject[] mines;

	[SerializeField] protected int mineCount = 5;

	[SerializeField] private Explosion explosion;

	protected Transform pieceTransform;

	private PieceMoveAudioManager am;

	/// <summary>
	/// Array that is used to simulated if a move results in a check to own king
	/// </summary>
	protected Piece[] testArray;

	private int BlackKingPos = -1;
	private int WhiteKingPos = -1;
	protected List<Move> allMoves;
	protected UIManager um;
	protected HighlightManager hm;
	protected GameController gc;

	public static bool isBlackBelow = true;

	public Piece[] Pieces => pieces;

	[SerializeField] protected Piece currPiece;
	[SerializeField] protected Card currCard;

	[SerializeField] protected Piece[] defaultPieceArray;

	/// <summary>
	/// The current king that is being clicked by the player
	/// </summary>
	public Piece CurrPiece { get => currPiece; set => currPiece = value; }

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
		InitGame();

		AssertAllReferenceIsNotNull();
	}

	public virtual void InitGame()
	{
		hm = FindObjectOfType<HighlightManager>();
		um = FindObjectOfType<UIManager>();
		gc = FindObjectOfType<GameController>();
		am = FindObjectOfType<PieceMoveAudioManager>();

		gc.ResetGame();

		pieceTransform = GameObject.Find("Pieces")?.transform;

		allMoves = new List<Move>();

		InstantiatePieces();
		InstantiateMines();

		testArray = pieces.Clone() as Piece[];
	}

	public void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(pieces);
		Assert.IsNotNull(blackPieces);
		Assert.IsNotNull(whitePieces);

		Assert.IsNotNull(pieceTransform);
	}

	/// <summary>
	/// Instantiates all pieces and highlight squares
	/// </summary>
	public virtual void InstantiatePieces()
	{
		for (var i = 0; i < 64; i++)
		{
			if (pieces[i] != null) InstantiatePiece(pieces[i], i);
		}

		// RotateAllPieces();
		testArray = pieces.Clone() as Piece[];
	}

	public virtual void InstantiateMines()
	{
		mines = new GameObject[64];
	}

	/// <summary>
	/// Instantiate a single king on the board
	/// </summary>
	/// <param name="piece">Type of king to be instantiated</param>
	/// <param name="pos">Position on board to be instantiated</param>
	/// <returns></returns>
	public virtual Piece InstantiatePiece(Piece piece, int pos)
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

		if (newPiece is King king)
		{
			if (king.Player == PlayerType.White)
			{
				WhiteKingPos = newPiece.CurrPos;
			} 
			else if (king.Player == PlayerType.Black)
			{
				BlackKingPos = newPiece.CurrPos;
			}
		}

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
		return IsInBounds(x, y) && testArray[pos]?.Player != p.Player;
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
	/// Sets the transform of king at a certain position and also modifies the pieces[] array
	/// </summary>
	/// <param name="piece">Piece to be moved</param>
	/// <param name="pos">New position on board</param>
	public virtual void SetPiecePos(int oldPos, int newPos)
	{
		if (pieces[oldPos] == null)
		{
			Debug.Log("Piece at position is null");
			return;
		}

		if (pieces[newPos] != null)
		{
			// Debug.Log("Sacrriiffiicce piece at index: " + newPos);
			CapturePiece(newPos);
		}
		else am.PlayMoveSelfAudio();

		pieces[oldPos].SetCoords(newPos);
		pieces[oldPos].SetTransform();
		pieces[newPos] = pieces[oldPos];
		pieces[oldPos] = null;
	}

	/// <summary>
	/// Removes the king at specified board position and destroys the gameobject
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

		am.PlayCaptureAudio();
		Piece destroyedPiece = pieces[pos];
		DestroyPiece(pos);

		if (destroyedPiece is King)
		{
			PlayerType p = destroyedPiece.Player == PlayerType.Black ? PlayerType.White : PlayerType.Black;
			gc.HandleGameOver(p);
			return;
		}

		if (gc.IsSpecialMode) HandleCapture(destroyedPiece);
	}

	/// <summary>
	/// Destroys the pieces and removes it from pieces[] array
	/// </summary>
	/// <param name="pos"></param>
	public virtual void DestroyPiece(int pos)
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
	/// For special game mode, handle capture of king
	/// </summary>
	/// <param name="capturedPiece"></param>
	public void HandleCapture(Piece capturedPiece)
	{
		if (gc.GetCurrPlayerManager() != null) gc.GetCurrPlayerManager().AddMoney(capturedPiece.Value * 2);
		DistributeRandomCard(gc.GetCurrPlayerManager());
	}

	/// <summary>
	/// Moves a king to position x, y on the board.
	/// </summary>
	/// <param name="currPiece">The current king chosen by player</param>
	public virtual void MovePiece(int x, int y, Piece piece)
	{
		int newPos = ConvPos(x, y);
		if (piece == null) Debug.Log("Piece at MovePiece() is null! Tried to move a null piece.");
		SetPiecePos(piece.CurrPos, newPos);

		// For special game mode
		if (gc.IsSpecialMode) TriggerMine(newPos);
	}

	public virtual void MoveEnPassantPiece(int x, int y, Piece piece)
	{
		int newPos = ConvPos(x, y);
		int enemyPos = ConvPos(x, ConvXY(piece.CurrPos)[1]);

		CapturePiece(enemyPos);
		SetPiecePos(piece.CurrPos, newPos);

		// For special game mode
		if (gc.IsSpecialMode) TriggerMine(newPos);
	}

	/// <summary>
	/// Move for castling
	/// </summary>
	/// <param name="targetX"> The x coordinate of rook to move</param>
	/// <param name="targetY">The y coordinate of rook to move</param>
	/// <param name="piece"></param>
	public virtual void MoveCastling(int targetX, int targetY, Piece king)
	{
		Piece rook = GetPieceFromPos(ConvPos(targetX, targetY));

		int oldX = ConvXY(king.CurrPos)[0];
		int kingNewX = oldX - 2;
		int rookNewX = ConvXY(rook.CurrPos + 2)[0];

		if (targetX != 0)
		{
			kingNewX = oldX + 2;
			rookNewX = ConvXY(rook.CurrPos - 3)[0];
		}

		am.PlayCastleAudio();
		MovePiece(kingNewX, targetY, king);
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
			TutorialManager.SetPawnHasMoved(pieces[i]);
		}
		for (int i = 24; i < 32; i++)
		{
			if (preset.rank4[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank4[i % 8], i);
			TutorialManager.SetPawnHasMoved(pieces[i]);
			TutorialManager.SetPawnTwoStep(pieces[i]);
		}
		for (int i = 32; i < 40; i++)
		{
			if (preset.rank5[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank5[i % 8], i);
			TutorialManager.SetPawnHasMoved(pieces[i]);
			TutorialManager.SetPawnTwoStep(pieces[i]);
		}
		for (int i = 40; i < 48; i++)
		{
			if (preset.rank6[i % 8] == null) continue;
			pieces[i] = InstantiatePiece(preset.rank6[i % 8], i);
			TutorialManager.SetPawnHasMoved(pieces[i]);
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
		for (var i = 0; i < 64; i++)
		{
			if (pieces[i] != null)
			{
				Destroy(pieces[i].gameObject);
			}
			pieces[i] = null;
		}
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
	/// Checks if the king at pos1 is the same player as the king at pos2
	/// If any one of the pieces is null, return false
	/// </summary>
	public bool IsSamePlayer(int pos1, int pos2)
	{
		Piece p1 = GetPieceFromPos(pos1);
		Piece p2 = GetPieceFromPos(pos2);
		return p1?.Player == p2?.Player;
	}

	/// <summary>
	/// Returns the king from a certain position on the board
	/// </summary>
	public Piece GetPieceFromPos(int pos)
	{
		try
		{
			return pieces[pos];
		}
		catch (IndexOutOfRangeException)
		{
			// Debug.Log("Cannot get piece from position");
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
	/// Handles the logic after a king of the current player is clicked
	/// </summary>
	/// <param name="col"></param>
	public virtual void HandlePieceClicked(Collider2D col)
	{
		DisableAllUIElements();
		um.UnhighlightAllPromotingButtons();
		CurrPiece = col.GetComponent<Piece>();
		List<Move> moves = CurrPiece.GetLegalMoves();

		foreach (Move move in moves) hm.Highlight(move);
	}

	/// <summary>
	/// Calls Promote() on the current king
	/// </summary>
	/// <param name="promotedPiece">The king type to be instantiated</param>
	public virtual void PromotePiece(Piece promotedPiece)
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
		am.PlayPromoteAudio();
		GameController.SetGameState(GameState.Play);
		GameController.InvokeOnRoundEnd();
	}

	/// <summary>
	/// Gets the promoted king type based on the id given
	/// </summary>
	/// <param name="id">The type of king to be promoted</param>
	/// <param name="player">Player type</param>
	/// <returns>The promoted king (Queen, Knight, Rook, Bishop)</returns>
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
	public virtual void SetPawnBooleansToFalse()
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

	public virtual void SetPawnBooleanToMoved(int pos)
	{
		Pawn p = GetPieceFromPos(pos) as Pawn;
		p.JustMoved = true;
		p.HasMoved = true;
	}

	public virtual void SetPawnBooleanToTwoStep(int pos)
	{
		Pawn p = GetPieceFromPos(pos) as Pawn;
		p.JustMoved = true;
		p.HasMoved = true;
		p.TwoStep = true;
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

			case Move.Flag.PromoteToKnight:
				testArray[oldPos] = null;
				testArray[newPos] = Instantiate(GetPromotionPiece(1, p));
				testArray[newPos].GetComponent<Renderer>().enabled = false;
				break;

			case Move.Flag.PromoteToBishop:
				testArray[oldPos] = null;
				testArray[newPos] = Instantiate(GetPromotionPiece(3, p));
				testArray[newPos].GetComponent<Renderer>().enabled = false;
				break;

			case Move.Flag.PromoteToRook:
				testArray[oldPos] = null;
				testArray[newPos] = Instantiate(GetPromotionPiece(2, p));
				testArray[newPos].GetComponent<Renderer>().enabled = false;
				break;

			case Move.Flag.PromoteToQueen:
				testArray[oldPos] = null;
				testArray[newPos] = Instantiate(GetPromotionPiece(0, p));
				testArray[newPos].GetComponent<Renderer>().enabled = false;
				break;

			default:
				if (testArray[oldPos] == null)
					Debug.Log($"UpdateTestArray: Piece at {oldPos} is null");

				MovePieceAndSetCoords(oldPos, newPos);
				break;
		}
	}

	public void ResetGame()
	{
		gc.ResetGame();
		ResetPieces();
	}

	public void ResetPieces()
	{
		for (var i = 0; i < 64; i++)
		{
			DestroyPiece(i);
			if (defaultPieceArray[i] != null) InstantiatePiece(defaultPieceArray[i], i);
		}
	}

	public bool TestArrayIsOccupied(int pos)
	{
		try
		{
			return testArray[pos] != null;
		}
		catch (IndexOutOfRangeException)
		{
			Debug.Log("TestArrayIsOccupied: Index out of range!");
			return true;
		}
	}

	public Piece GetPieceFromTestArrayPos(int pos)
	{
		try
		{
			return testArray[pos];
		}
		catch (IndexOutOfRangeException)
		{
			return null;
		}
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
		if (temp) am.PlayMoveCheckAudio();
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

	public void RotateAllPieces()
	{
		foreach(Piece piece in pieces)
		{
			if (piece == null) continue;
			piece.transform.eulerAngles = new Vector3(180f, 180f, 0f);
		}
	}

	public void RotatePiece(Piece piece)
	{
		piece.gameObject.transform.eulerAngles = new Vector3(180f, 180f, 0f);
	}

	public virtual void SyncCurrPiece(int piecePos)
	{
		CurrPiece = GetPieceFromPos(piecePos);
	}

	#region Special Moves
	// Special moves:
	public virtual void SyncCurrCard(Card card)
	{
		currCard = card;
	}

	public virtual void DestroyCurrentCard()
	{
		gc.GetCurrPlayerManager().RemoveCard(currCard);
    }


	/// <summary>
	/// Bombs a 3x3 area around a position
	/// </summary>
	/// <param name="pos"></param>
	public virtual void Bomb(int pos)
	{
		if (pos < 0 || pos > 63) Debug.Log("Bomb: pos out of range");
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				int x = ConvXY(pos)[0] + i;
				int y = ConvXY(pos)[1] + j;
				if (!IsInBounds(x, y) || GetPieceFromPos(ConvPos(x, y)) is King) continue;
				DestroyPiece(ConvPos(x, y));
			}
		}

		SetAndTriggerExplosionWithScale(pos, 3);
		testArray = pieces.Clone() as Piece[];
	}

	/// <summary>
	/// Special Game Mode: Randomizes pieces on the board for both sides
	/// </summary>
	public virtual void RandomizeAllPieces()
	{
		foreach (Piece piece in pieces)
		{
			// Only 50% chance of randomizing a piece
			bool coinflip =  UnityEngine.Random.Range(0, 2) == 0;
			if (piece == null || piece is King || coinflip) continue;

			int rand = UnityEngine.Random.Range(0, 16);
			PlayerType p = piece.Player;

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
	/// Special Game Mode: Burgle an opponent king, excluding king and queen
	/// </summary>
	/// <param name="p"></param>
	/// <param name="pos"></param>
	public virtual void StealOpponentPiece(int pos)
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
	/// Special Game Mode: Buy a king
	/// </summary>
	/// <param name="boughtPiece"></param>
	public void BuyPiece(Piece boughtPiece)
	{
		am.PlayPurchaseSuccessAudio();
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
	public virtual void PlantMine(int pos)
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
	/// Triggers a mine which will destroy the king on it
	/// </summary>
	/// <param name="pos"></param>
	public virtual void TriggerMine(int pos)
	{
		if (mines[pos] == null)
		{
			//Debug.Log("There is no mine here!");
			return;
		}

		if (pieces[pos] is King)
		{
			Debug.Log("King triggers pawn, mine removed!");
			return;
		}

		SetAndTriggerExplosionWithScale(pos, 1);
		Destroy(mines[pos]);
		DestroyPiece(pos);
	}

	private void SetAndTriggerExplosionWithScale(int pos, int scale)
    {
		int x = BoardController.ConvXY(pos)[0];
		int y = BoardController.ConvXY(pos)[1];
		Explosion exp = Instantiate(explosion, new Vector3(x, y, 2), Quaternion.identity);
		exp.transform.localScale = new Vector3(scale, scale, 1);
		exp.Explode();
	}

	/// <summary>
	/// Distribute a random card to a player (who has captured a king)
	/// </summary>
	/// <param name="player"></param>
	public virtual void DistributeRandomCard(SpecialPlayerManager player)
	{
		int rand = UnityEngine.Random.Range(0, cards.Length);
		Card card = cards[rand];
		player.AddCard(card);
	}

	public virtual void BurgleRandomPiece()
	{
		List<Piece> opponentPieces = new List<Piece>();

		foreach (Piece piece in pieces)
		{
			if (piece == null || piece is King || piece.Player == GameController.GetCurrPlayer()) continue;
			opponentPieces.Add(piece);
		}

		if (opponentPieces.Count == 0) return;
		int rand = UnityEngine.Random.Range(0, opponentPieces.Count);
		StealOpponentPiece(opponentPieces[rand].CurrPos);
	}

	public virtual void SetDoubleTurn(bool boolean)
	{
		gc.IsDoubleTurn = boolean;
	}

	public virtual void SacrificePiece(int pos)
	{
		DestroyPiece(pos);
	}

	public virtual void BuildPawnWall()
	{
		if (GameController.GetCurrPlayer() == PlayerType.Black)
		{
			for (int i = 8; i < 16; i++)
			{
				if (pieces[i] != null) continue;
				pieces[i] = InstantiatePiece(GetPromotionPiece(4, PlayerType.Black), i);
			}
		}

		else if (GameController.GetCurrPlayer() == PlayerType.White)
		{
			for (int i = 48; i < 56; i++)
			{
				if (pieces[i] != null) continue;
				pieces[i] = InstantiatePiece(GetPromotionPiece(4, PlayerType.White), i);
			}
		}
	}

	#endregion
}