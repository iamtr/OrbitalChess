using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private bool hasMoved = false;
    private bool twoStep = false;
	public PawnPromotion pp;
    private TurnCountdown turnCountdown;

	public override void InitPiece(PlayerType p)
    {
        base.InitPiece(p);
        OnAfterMove += CheckForPromotion;
        OnAfterMove += SetPawnBoolean;
        turnCountdown = BoardController.i.InstantiateTurnCountdown();
    }

    private void OnDestroy()
    {
        if (turnCountdown == null) return;
        Destroy(turnCountdown.gameObject);
    }

    public override void GetAvailableMoves()
    {
        int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        if (IsLegalMove(currX, newY, this))
        {
            bc.Highlight(currX, newY, this);

            if (!hasMoved && !bc.IsOccupied(bc.ConvertToPos(currX, newY + direction)))
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
        Piece rightPiece = bc.GetPieceFromPos(bc.ConvertToPos(rightX, currY));
        Piece leftPiece = bc.GetPieceFromPos(bc.ConvertToPos(leftX, currY));

        if (bc.IsLegalMove(rightX, newY, this)
            && rightPiece != null
            && rightPiece.Player != this.Player
            && CheckEnPassant(rightPiece))
        {
            int pos = bc.ConvertToPos(rightX, newY);
            bc.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(rightX, newY);
        }

        if (bc.IsLegalMove(leftX, newY, this)
            && leftPiece != null
            && leftPiece.Player != this.Player
            && CheckEnPassant(leftPiece))
        {
            int pos = bc.ConvertToPos(leftX, newY);
            bc.SetHighlightColor(pos, Color.yellow);
            //ep.SetHighlightEnPassant(leftX, newY);
        }
    }

    public override bool IsLegalMove(int x, int y, Piece p)
    {
        int pos = y * 8 + x;
        if (!bc.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos) || bc.IsOccupied(bc.ConvertToPos(x, y)))
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
        Piece rightPiece = bc.GetPieceFromPos(bc.ConvertToPos(rightX, newY));
        Piece leftPiece = bc.GetPieceFromPos(bc.ConvertToPos(leftX, newY));

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

    public bool IsAvailableForPromotion()
    {
        return this.Player == PlayerType.Black ? currY >= 7 : currY <= 0;
    }

    public void SetPawnBoolean()
    {
        if(!hasMoved) turnCountdown.TriggerTurnCountdown(this);
        hasMoved = true;
    }

    public void SetTwoStepMove(int y)
    {
        int difference = System.Math.Abs(this.currY - y);
        if (difference > 1)
        {
            twoStep = true;
        }
    }

    public void CheckForPromotion()
    {
        if (IsAvailableForPromotion()) ChoosePromotion();
	}

    public void ChoosePromotion()
    {
        GameController.i.SetGameState(GameState.Promoting);
        PawnPromotion.i.ShowPromotionButtons(this.Player);
    }

    public bool CheckEnPassant(Piece piece)
    {
        if (piece is Pawn)
        {
            Pawn pawn = (Pawn)piece;
            if (pawn.turnCountdown.IsJustMoved() && pawn.twoStep)
            {
                return true;
            }
        }
        return false;
    }
}
