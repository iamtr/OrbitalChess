using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    /// <summary>
    /// A boolean of whether the pawn has moved from its initial position
    /// </summary>
    private bool hasMoved = false;

    /// <summary>
    /// A boolean of whether the pawn is moved by two squares in its initial move
    /// </summary>
    private bool twoStep = false;

    /// <summary>
    /// A turn countdown to determine en passant movement of pawn
    /// </summary>
    private TurnCountdown turnCountdown;
    
    public override void InitPiece(PlayerType p)
    {
        base.InitPiece(p);
        OnAfterMove += CheckForPromotion;
        OnAfterMove += SetPawnBoolean;
        turnCountdown = BoardController.i.InstantiateTurnCountdown();
    }
    
    /// <summary>
    /// Destroys turn countdown of pawn if the pawn is destroyed
    /// </summary>
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

            if (!hasMoved && !bc.IsOccupied(BoardController.ConvertToPos(currX, newY + direction)))
            {
                bc.Highlight(currX, newY + direction, this);
            }
        }
        HighlightPawnDiagonals(direction);
        HighlightEnPassant(direction);
    }

    public override bool IsLegalMove(int x, int y, Piece p)
    {
        int pos = y * 8 + x;
        if (!BoardController.IsInBounds(x, y) || bc.IsSamePlayer(this.CurrPos, pos) 
            || bc.IsOccupied(BoardController.ConvertToPos(x, y)))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// If legal and available, highlights a pawn capturing an opponent piece diagonally
    /// </summary>
    /// <param name="direction"></param>
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

    /// <summary>
    /// If legal and available, highlights en passant movement of pawn
    /// </summary>
    /// <param name="direction"></param>
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

    public bool IsAvailableForPromotion()
    {
        return this.Player == PlayerType.Black ? currY >= 7 : currY <= 0;
    }

    /// <summary>
    /// Sets the hasMoved boolean and triggers the Turn Countdown 
    /// if movement is the initial move
    /// </summary>
    public void SetPawnBoolean()
    {
        if(!hasMoved) turnCountdown.TriggerTurnCountdown();
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
            twoStep = true;
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
        GameController.SetGameState(GameState.Promoting);
        UIManager.ShowPromotionButtons(this.Player);
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
            if (pawn.turnCountdown.getCountdownOngoing() && pawn.twoStep)
            {
                return true;
            }
        }
        return false;
    }
}
