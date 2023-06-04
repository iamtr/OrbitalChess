using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
	/// <summary>
	/// A boolean to check whether the pawn has moved from its initial position
	/// The boolean is set to false by default
	/// </summary>
	private bool hasMoved = false;

	private void OnEnable()
	{
		OnAfterMove += SetRookBoolean;
	}

	private void OnDisable()
	{
		OnAfterMove -= SetRookBoolean;
	}

	public override void InitPiece(PlayerType p)
	{
		base.InitPiece(p);
	}

	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		void GetMovesFromDirection(int currX, int currY, int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				int pos = y * 8 + x;

				Move m = new Move(CurrPos, pos, this);

				if (!IsLegalMove(m)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		GetMovesFromDirection(currX, currY, 1, 0, 8);
		GetMovesFromDirection(currX, currY, -1, 0, 8);
		GetMovesFromDirection(currX, currY, 0, 1, 8);
		GetMovesFromDirection(currX, currY, 0, -1, 8);

		return moves;
	}

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		if (BoardController.i.IsBeingCheckedAfterMove(move)) return false;
		return true;
	}

	public bool IsMoved()
    {
		return hasMoved;
    }

	/// <summary>
	/// Sets the hasMoved boolean in its inital move
	/// </summary>
	public void SetRookBoolean()
    {
		hasMoved = true;
	}
}
