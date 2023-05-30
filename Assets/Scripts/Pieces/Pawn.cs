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
        //turnCountdown = bc.InstantiateTurnCountdown();
    }

    private void OnDestroy()
    {
        //if (turnCountdown == null) return;
        //Destroy(turnCountdown.gameObject);
    }

    public override void GetAvailableMoves()
    {
        int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        if (IsLegalMove(currX, newY, this))
        {
            bc.Highlight(currX, newY, this);

            if (!hasMoved && !bc.IsOccupied(BoardController.ConvertToPos(currX, newY + direction)))
            {
                bc.Highlight(currX, newY + direction, this);
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
        Piece rightPiece = bc.GetPieceFromPos(BoardController.ConvertToPos(rightX, currY));
        Piece leftPiece = bc.GetPieceFromPos(BoardController.ConvertToPos(leftX, currY));

        if (bc.IsLegalMove(rightX, newY, this)
            && rightPiece != null
            && rightPiece.Player != this.Player
            && CheckEnPassant(rightPiece))
        {
            int pos = BoardController.ConvertToPos(rightX, newY);
            BoardController.i.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(rightX, newY);
        }

        if (bc.IsLegalMove(leftX, newY, this)
            && leftPiece != null
            && leftPiece.Player != this.Player
            && CheckEnPassant(leftPiece))
        {
            int pos = BoardController.ConvertToPos(leftX, newY);
            BoardController.i.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(leftX, newY);
        }
    }

    public override bool IsLegalMove(int x, int y, Piece p)
    {
        int pos = y * 8 + x;
        if (!BoardController.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos) || bc.IsOccupied(BoardController.ConvertToPos(x, y)))
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
        Piece rightPiece = bc.GetPieceFromPos(BoardController.ConvertToPos(rightX, newY));
        Piece leftPiece = bc.GetPieceFromPos(BoardController.ConvertToPos(leftX, newY));

        if (bc.IsLegalMove(rightX, newY, this)
            && rightPiece != null 
            && rightPiece.Player != this.Player)
        {
            bc.Highlight(rightX, newY, this);
        }

        if (bc.IsLegalMove(leftX, newY, this)
			&& leftPiece != null 
            && leftPiece.Player != this.Player)
        {
            bc.Highlight(leftX, newY, this);
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
			UIManager.ShowPromotionButtons(this.Player);
		}
	}

    public void Promote(Piece newPiece)
    {
        bc.InstantiatePiece(newPiece, CurrPos);
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
