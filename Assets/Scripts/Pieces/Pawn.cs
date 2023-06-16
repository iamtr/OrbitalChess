using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class Pawn : Piece, IPromotable
{
    /// <summary>
    /// A boolean of whether the pawn has moved from its initial position
    /// </summary>
    private bool hasMoved = false;
    public bool JustMoved { get; set; } = false;
    public  bool TwoStep { get; set; } = false;

	private void Awake()
	{
		value = 10;
	}

	private void OnEnable()
	{
		OnAfterMove += CheckForPromotion;
		OnAfterMove += SetPawnBoolean;
		OnAfterMove += GameController.InvokeOnRoundEnd;
	}

	private void OnDisable()
	{
		OnAfterMove -= CheckForPromotion;
		OnAfterMove -= SetPawnBoolean;
		OnAfterMove -= GameController.InvokeOnRoundEnd;
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
			Piece rightPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(rightX, currY));
			Piece leftPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(leftX, currY));

			if (BoardController.i.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player
				&& CheckEnPassant(rightPiece))
			{
				int pos = BoardController.i.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m) && !BoardController.i.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			if (BoardController.i.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player
				&& CheckEnPassant(leftPiece))
			{
				int pos = BoardController.i.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m) && !BoardController.i.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}
		}

		void GetPawnDiagonalMoves(int direction)
		{
			int rightX = currX + 1;
			int leftX = currX - 1;
			int newY = currY + direction;
			Piece rightPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(rightX, newY));
			Piece leftPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(leftX, newY));

			if (BoardController.i.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player)
			{
				int pos = BoardController.i.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m) && !BoardController.i.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			if (BoardController.i.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player)
			{
				int pos = BoardController.i.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m) && !BoardController.i.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}
		}
		moves.Clear();

		int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        Move m = new Move(CurrPos, BoardController.i.ConvPos(currX, newY), this);

        if (IsLegalMove(m) && !BoardController.i.IsOccupied(m.TargetSquare) && !BoardController.i.IsBeingCheckedAfterMove(m, Player))
        {
            moves.Add(m);
        }

        m = new Move(CurrPos, BoardController.i.ConvPos(currX, newY + direction), this);

		if (!hasMoved && IsLegalMove(m) && !BoardController.i.IsOccupied(m.TargetSquare) && !BoardController.i.IsBeingCheckedAfterMove(m, Player))
		{
			moves.Add(m);
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

			Piece rightPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(rightX, currY));
			Piece leftPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(leftX, currY));

			if (BoardController.i.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player
				&& CheckEnPassant(rightPiece))
			{
				int pos = BoardController.i.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m)) moves.Add(m);
			}

			if (BoardController.i.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player
				&& CheckEnPassant(leftPiece))
			{
				int pos = BoardController.i.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture);
				if (IsLegalMove(m)) moves.Add(m);
			}
		}
		void GetAllPawnDiagonalMoves(int direction)
		{
			int rightX = currX + 1;
			int leftX = currX - 1;
			int newY = currY + direction;
			Piece rightPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(rightX, newY));
			Piece leftPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvPos(leftX, newY));

			if (BoardController.i.IsLegalMove(rightX, newY, this)
				&& rightPiece != null
				&& rightPiece.Player != this.Player)
			{
				int pos = BoardController.i.ConvPos(rightX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m)) moves.Add(m);
			}

			if (BoardController.i.IsLegalMove(leftX, newY, this)
				&& leftPiece != null
				&& leftPiece.Player != this.Player)
			{
				int pos = BoardController.i.ConvPos(leftX, newY);
				Move m = new Move(CurrPos, pos, this);
				if (IsLegalMove(m)) moves.Add(m);
			}
		}
		moves.Clear();

		int direction = (Player == PlayerType.Black) ? 1 : -1;
		int newY = currY + direction;

		Move m = new Move(CurrPos, BoardController.i.ConvPos(currX, newY), this);

		if (IsLegalMove(m) && !BoardController.i.TestArrayIsOccupied(m.TargetSquare))
		{
			moves.Add(m);
		}

		m = new Move(CurrPos, BoardController.i.ConvPos(currX, newY + direction), this);

		if (!hasMoved && IsLegalMove(m) && !BoardController.i.TestArrayIsOccupied(m.TargetSquare))
		{
			moves.Add(m);
		}

		GetAllEnPassantMoves(direction);
		GetAllPawnDiagonalMoves(direction);

		return moves;
	}

	/// <summary>
	/// Sets the hasMoved boolean and triggers the Turn Countdown 
	/// if movement is the initial move
	/// </summary>
	public void SetPawnBoolean()
    {
        JustMoved = true;
        hasMoved = true;
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
			UIManager.i.ShowPromotionButtons(this.Player);
		}
	}

    public void Promote(Piece newPiece)
    {
        BoardController.i.InstantiatePiece(newPiece, CurrPos);
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
}
