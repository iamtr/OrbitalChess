using System.Collections.Generic;

public class Pawn : Piece, IPromotable
{
	/// <summary>
	/// A boolean of whether the pawn has moved from its initial position
	/// </summary>
	public bool HasMoved { get; set; } = false;

	public bool JustMoved { get; set; } = false;
	public bool TwoStep { get; set; } = false;

	protected override void Awake()
	{
		base.Awake();
		value = 10;
	}

	private void OnEnable()
	{
		OnAfterMove += CheckForPromotion;
		OnAfterMove += SetPawnBoolean;
		OnAfterMove += InvokeOnRoundEnd;
	}

	private void OnDisable()
	{
		OnAfterMove -= CheckForPromotion;
		OnAfterMove -= SetPawnBoolean;
		OnAfterMove -= InvokeOnRoundEnd;
	}

	public override void InitPiece(PlayerType p)
	{
		base.InitPiece(p);
	}

	public override List<Move> GetLegalMoves()
	{
		void GetEnPassantMoves(int direction)
		{
			int rightX = currX + 1;
			int leftX = currX - 1;
			int newY = currY + direction;
			Piece rightPiece = bc.GetPieceFromPos(bc.ConvPos(rightX, currY));
			Piece leftPiece = bc.GetPieceFromPos(bc.ConvPos(leftX, currY));

			if (bc.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player
				&& CheckEnPassant(rightPiece))
			{
				int pos = bc.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m) && !bc.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			if (bc.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player
				&& CheckEnPassant(leftPiece))
			{
				int pos = bc.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m) && !bc.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}
		}

		void GetPawnDiagonalMoves(int direction)
		{
			int rightX = currX + 1;
			int leftX = currX - 1;
			int newY = currY + direction;
			Piece rightPiece = bc.GetPieceFromPos(bc.ConvPos(rightX, newY));
			Piece leftPiece = bc.GetPieceFromPos(bc.ConvPos(leftX, newY));

			if (bc.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player)
			{
				int pos = bc.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m) && !bc.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			if (bc.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player)
			{
				int pos = bc.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m) && !bc.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}
		}
		moves.Clear();

		int direction = (Player == PlayerType.Black) ? 1 : -1;
		int newY = currY + direction;

		Move m1 = new Move(CurrPos, bc.ConvPos(currX, newY), this);

		if (IsLegalMove(m1) 
			&& !bc.IsOccupied(m1.TargetSquare) 
			&& !bc.IsBeingCheckedAfterMove(m1, Player)
			&& (newY == 7 || newY == 0))
		{
			Move promoteKnight = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToKnight);
			Move promoteBishop = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToBishop);
			Move promoteRook = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToRook);
			Move promoteQueen = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToQueen);

			moves.Add(promoteKnight);
			moves.Add(promoteBishop);
			moves.Add(promoteRook);
			moves.Add(promoteQueen);
		}

		if (IsLegalMove(m1) && !bc.IsOccupied(m1.TargetSquare) && !bc.IsBeingCheckedAfterMove(m1, Player))
		{
			moves.Add(m1);
		}

		Move m2 = new Move(CurrPos, bc.ConvPos(currX, newY + direction), this);

		if (!HasMoved && IsLegalMove(m2) && !bc.IsOccupied(m2.TargetSquare) && !bc.IsBeingCheckedAfterMove(m2, Player) && !bc.IsOccupied(m1.TargetSquare))
		{
			moves.Add(m2);
		}

		GetEnPassantMoves(direction);
		GetPawnDiagonalMoves(direction);

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		void GetAllEnPassantMoves(int direction)
		{
			int rightX = currX + 1;
			int leftX = currX - 1;
			int newY = currY + direction;

			Piece rightPiece = bc.GetPieceFromTestArrayPos(bc.ConvPos(rightX, currY));
			Piece leftPiece = bc.GetPieceFromTestArrayPos(bc.ConvPos(leftX, currY));

			if (bc.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player
				&& CheckEnPassant(rightPiece))
			{
				int pos = bc.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m)) moves.Add(m);
			}

			if (bc.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player
				&& CheckEnPassant(leftPiece))
			{
				int pos = bc.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m)) moves.Add(m);
			}
		}
		void GetAllPawnDiagonalMoves(int direction)
		{
			int rightX = currX + 1;
			int leftX = currX - 1;
			int newY = currY + direction;

			Piece rightPiece = bc.GetPieceFromTestArrayPos(bc.ConvPos(rightX, newY));
			Piece leftPiece = bc.GetPieceFromTestArrayPos(bc.ConvPos(leftX, newY));

			if (bc.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player)
			{
				int pos = bc.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m)) moves.Add(m);
			}

			if (bc.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player)
			{
				int pos = bc.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m)) moves.Add(m);
			}
		}

		moves.Clear();

		int direction = (Player == PlayerType.Black) ? 1 : -1;
		int newY = currY + direction;

		Move m1 = new Move(CurrPos, bc.ConvPos(currX, newY), this);

		if (IsLegalMove(m1) && !bc.TestArrayIsOccupied(m1.TargetSquare) && (newY == 7 || newY  == 0))
		{
			Move promoteKnight = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToKnight);
			Move promoteBishop = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToBishop);
			Move promoteRook = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToRook);
			Move promoteQueen = new Move(CurrPos, bc.ConvPos(currX, newY), this, Move.Flag.PromoteToQueen);

			moves.Add(promoteKnight);
			moves.Add(promoteBishop);
			moves.Add(promoteRook);
			moves.Add(promoteQueen);
		}

		else if (IsLegalMove(m1) && !bc.TestArrayIsOccupied(m1.TargetSquare))
		{
			moves.Add(m1);
		}

		Move m2 = new Move(CurrPos, bc.ConvPos(currX, newY + direction), this);

		if (!HasMoved && IsLegalMove(m1) && !bc.TestArrayIsOccupied(m2.TargetSquare) && !bc.TestArrayIsOccupied(m1.TargetSquare))
		{
			moves.Add(m2);
		}

		GetAllEnPassantMoves(direction);
		GetAllPawnDiagonalMoves(direction);

		return moves;
	}

	/// <summary>
	/// Sets the HasMoved boolean and triggers the Turn Countdown
	/// if movement is the initial move
	/// </summary>
	public void SetPawnBoolean()
	{
		JustMoved = true;
		HasMoved = true;
	}

	/// <summary>
	/// Detects and sets the twoStep boolean accordingly
	/// </summary>
	/// <param name="y"></param>
	public void SetTwoStepMove(int y)
	{
		int difference = System.Math.Abs(this.currY - y);
		if (difference > 1)
		{
			TwoStep = true;
		}
	}

	/// <summary>
	/// If available, activates the promotion buttons
	/// to allow player to promote their pawn
	/// </summary>
	public void CheckForPromotion()
	{
		if (IsAvailableForPromotion()) ChoosePromotion();
	}

	public void ChoosePromotion()
	{
		if (IsAvailableForPromotion())
		{
			GameController.SetGameState(GameState.Promoting);
			um.ShowPromotionButtons(this.Player);
		}
	}

	public void Promote(Piece newPiece)
	{
		bc.InstantiatePiece(newPiece, CurrPos);
		Destroy(gameObject);
	}

	/// <summary>
	/// Checks whether the opponent piece is able to be en passant-ed
	/// </summary>
	/// <param name="piece">The opponent piece</param>
	/// <returns></returns>
	public bool CheckEnPassant(Piece piece)
	{
		if (piece is Pawn pawn)
		{
			if (pawn.JustMoved && pawn.TwoStep) return true;
		}
		return false;
	}

	public bool IsAvailableForPromotion()
	{
		return (this.Player == PlayerType.Black && this.currY == 7) || (this.Player == PlayerType.White && this.currY == 0);
	}

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63) return false;
		return true;
	}

	public void InvokeOnRoundEnd()
	{
		if (!IsAvailableForPromotion()) GameController.InvokeOnRoundEnd();
	}
}