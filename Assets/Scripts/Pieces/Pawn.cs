using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece, IPromotable
{
    /// <summary>
    /// A boolean of whether the pawn has moved from its initial position
    /// </summary>
    private bool hasMoved = false;

    public bool JustMoved { get; set; } = false;
    public  bool TwoStep { get; set; } = false;

    // private TurnCountdown turnCountdown;
    
    public override void InitPiece(PlayerType p)
    {
        base.InitPiece(p);
        OnAfterMove += CheckForPromotion;
        OnAfterMove += SetPawnBoolean;
        //turnCountdown = BoardController.i.InstantiateTurnCountdown();
    }

    //private void OnDestroy()
    //{
    //    //if (turnCountdown == null) return;
    //    //Destroy(turnCountdown.gameObject);
    //}

    public override List<Move> GetLegalMoves()
    {
		moves.Clear();

		int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        if (IsLegalMove(currX, newY, this))
        {
            moves.Add(new Move(CurrPos, BoardController.i.ConvertToPos(currX, newY), this));
        }

		if (!hasMoved && !BoardController.i.IsOccupied(BoardController.i.ConvertToPos(currX, newY + direction)))
		{
			moves.Add(new Move(CurrPos, BoardController.i.ConvertToPos(currX, newY + direction), this, Move.Flag.PawnTwoForward));
		}

		GetEnPassantMoves(direction);
        GetPawnDiagonalMoves(direction);

        return moves;
    }

    public void GetEnPassantMoves(int direction)
    {
        int rightX = currX + 1;
        int leftX = currX - 1;
        int newY = currY + direction;
        Piece rightPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvertToPos(rightX, currY));
        Piece leftPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvertToPos(leftX, currY));

        if (BoardController.i.IsLegalMove(rightX, newY, this)
            && rightPiece != null
            && rightPiece.Player != this.Player
            && CheckEnPassant(rightPiece))
        {
            int pos = BoardController.i.ConvertToPos(rightX, newY);

            // TODO
            // BoardController.i.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(rightX, newY);

            moves.Add(new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture));
        }

        if (BoardController.i.IsLegalMove(leftX, newY, this)
            && leftPiece != null
            && leftPiece.Player != this.Player
            && CheckEnPassant(leftPiece))
        {
            int pos = BoardController.i.ConvertToPos(leftX, newY);
            //BoardController.i.SetHighlightColor(pos, Color.yellow);
			//ep.SetHighlightEnPassant(leftX, newY);
			moves.Add(new Move(CurrPos, pos, this, Move.Flag.EnPassantCapture));
		}
    }

	public void GetPawnDiagonalMoves(int direction)
	{
		int rightX = currX + 1;
		int leftX = currX - 1;
		int newY = currY + direction;
		Piece rightPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvertToPos(rightX, newY));
		Piece leftPiece = BoardController.i.GetPieceFromPos(BoardController.i.ConvertToPos(leftX, newY));

		if (BoardController.i.IsLegalMove(rightX, newY, this)
			&& rightPiece != null
			&& rightPiece.Player != this.Player)
		{
            // BoardController.i.Highlight(rightX, newY, this);
            int pos = BoardController.i.ConvertToPos(rightX, newY);
            moves.Add(new Move(CurrPos, pos, this));
		}

		if (BoardController.i.IsLegalMove(leftX, newY, this)
			&& leftPiece != null
			&& leftPiece.Player != this.Player)
		{
            // BoardController.i.Highlight(leftX, newY, this);
            int pos = BoardController.i.ConvertToPos(leftX, newY);
			moves.Add(new Move(CurrPos, pos, this));
		}
	}

	public override bool IsLegalMove(int x, int y, Piece p)
    {
        int pos = y * 8 + x;
        if (!BoardController.i.IsInBounds(x, y) || BoardController.i.IsSamePlayer(this.CurrPos, pos) || BoardController.i.IsOccupied(BoardController.i.ConvertToPos(x, y)))
        {
            return false;
        }

        return true;
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
        Destroy(this.gameObject);
    }

	public void Promote(Piece newPiece)
	{
		BoardController.i.InstantiatePiece(newPiece, CurrPos);
		Destroy(this.gameObject);
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
}
