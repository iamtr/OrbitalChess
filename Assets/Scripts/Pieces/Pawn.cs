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
        turnCountdown = EnPassant.i.InstantiateTurnCountdown();
    }

    private void OnDestroy()
    {
        if (turnCountdown == null) return;
        Destroy(turnCountdown.gameObject);
    }

    public override List<int> GetAvailableMoves()
    {
        List<int> availableMovesArray = new List<int>();

        int direction = (Player == PlayerType.Black) ? 1 : -1;
        int newY = currY + direction;

        if (IsLegalMove(currX, newY, this))
        {
            bc.Highlight(currX, newY, this);
            availableMovesArray.Add(bc.ConvertToPos(currX, newY));

            if (!hasMoved && !bc.IsOccupied(bc.ConvertToPos(currX, newY + direction)))
            {
                bc.Highlight(currX, newY + direction, this);
                availableMovesArray.Add(bc.ConvertToPos(currX, newY + direction));
            }
        }

        HighlightEnPassant(direction);
        HighlightPawnDiagonals(direction);

        return availableMovesArray;
    }

    public List<int> HighlightEnPassant(int direction)
    {
        List<int> temp = new List<int>();

        int rightX = currX + 1;
        int leftX = currX - 1;
        int newY = currY + direction;
        Piece rightPiece = bc.GetPieceFromPos(bc.ConvertToPos(rightX, currY));
        Piece leftPiece = bc.GetPieceFromPos(bc.ConvertToPos(leftX, currY));

        if (bc.IsLegalMove(rightX, newY, this)
            && rightPiece != null
            && rightPiece.Player != this.Player
            && EnPassant.i.CheckEnPassant(rightPiece))
        {
            int pos = bc.ConvertToPos(rightX, newY);
            bc.SetHighlightColor(pos, Color.yellow);
            temp.Add(pos);
            //ep.SetHighlightEnPassant(rightX, newY);
        }

        if (bc.IsLegalMove(leftX, newY, this)
            && leftPiece != null
            && leftPiece.Player != this.Player
            && EnPassant.i.CheckEnPassant(leftPiece))
        {
            int pos = bc.ConvertToPos(leftX, newY);
            bc.SetHighlightColor(pos, Color.yellow);
            temp.Add(pos);
            //ep.SetHighlightEnPassant(leftX, newY);
        }

        return temp;
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

    public List<int> HighlightPawnDiagonals(int direction)
    {
        List<int> temp = new List<int>();

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
            temp.Add(bc.ConvertToPos(rightX, newY));
        }

        if (bc.IsLegalMove(leftX, newY, this)
			&& leftPiece != null 
            && leftPiece.Player != this.Player)
        {
            bc.Highlight(leftX, newY, this);
            temp.Add(bc.ConvertToPos(leftX, newY));
        }

        return temp;
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

    public bool GetTwoStepBool()
    {
        return twoStep;
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

    public TurnCountdown getTurnCountdown()
    {
        return turnCountdown;
    }
}
