using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : Piece, IPromotable
{
    private bool hasMoved = false;

    public bool JustMoved { get; set; } = false;
    public  bool TwoStep { get; set; } = false;

    // private TurnCountdown turnCountdown;
    

    public override void InitPiece(PlayerType p)
    {
        base.InitPiece(p);
        OnAfterMove += ShowPromotions;
        OnAfterMove += SetPawnBoolean;
        //turnCountdown = BoardController.i.InstantiateTurnCountdown();
    }

    //private void OnDestroy()
    //{
    //    //if (turnCountdown == null) return;
    //    //Destroy(turnCountdown.gameObject);
    //}

    public override void GetAvailableMoves()
    {
        int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        if (IsLegalMove(currX, newY, this))
        {
            BoardController.i.Highlight(currX, newY, this);

            if (!hasMoved && !BoardController.i.IsOccupied(BoardController.i.ConvertToPos(currX, newY + direction)))
            {
                BoardController.i.Highlight(currX, newY + direction, this);
            }
        }

        HighlightEnPassant(direction);
        HighlightPawnDiagonals(direction);
        
    }

    public void HighlightEnPassant(int direction)
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
            BoardController.i.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(rightX, newY);
        }

        if (BoardController.i.IsLegalMove(leftX, newY, this)
            && leftPiece != null
            && leftPiece.Player != this.Player
            && CheckEnPassant(leftPiece))
        {
            int pos = BoardController.i.ConvertToPos(leftX, newY);
            BoardController.i.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(leftX, newY);
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

    public void HighlightPawnDiagonals(int direction)
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
            BoardController.i.Highlight(rightX, newY, this);
        }

        if (BoardController.i.IsLegalMove(leftX, newY, this)
			&& leftPiece != null 
            && leftPiece.Player != this.Player)
        {
            BoardController.i.Highlight(leftX, newY, this);
        }
    }

    public void SetPawnBoolean()
    {
        JustMoved = true;
        hasMoved = true;
    }

    public void SetTwoStepMove(int y)
    {
        int difference = System.Math.Abs(this.currY - y);
        if (difference > 1)
        {
            TwoStep = true;
        }
    }

	public bool IsAvailableForPromotion()
	{
		return this.Player == PlayerType.Black ? currY >= 7 : currY <= 0;
	}


	public void ShowPromotions()
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

    public bool CheckEnPassant(Piece piece)
    {
        if (piece is Pawn pawn)
        {
            if (pawn.JustMoved && pawn.TwoStep) return true;
        }
        return false;
    }
}
