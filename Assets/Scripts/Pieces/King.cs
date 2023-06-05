using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class King : Piece
{
	/// <summary>
	/// A boolean to check whether the pawn has moved from its initial position
	/// The boolean is set to false by default
	/// </summary>
	private bool hasMoved = false;

	private void OnEnable()
	{
		OnAfterMove += SetKingBoolean;
		OnAfterMove += UpdateKingPosition;
		OnAfterMove += GameController.InvokeOnRoundEnd;
	}

	private void OnDisable()
	{
		OnAfterMove -= SetKingBoolean;
		OnAfterMove -= UpdateKingPosition;
		OnAfterMove -= GameController.InvokeOnRoundEnd;
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
				if (x < 0 || x > 7 || y < 0 || y > 7) break;
				int pos = y * 8 + x;

				Move m = new Move(CurrPos, pos, this);

				if (!IsLegalMove(m) || BoardController.i.IsBeingCheckedAfterMove(m)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		GetMovesFromDirection(currX, currY, 1, 1, 1);
		GetMovesFromDirection(currX, currY, -1, 1, 1);
		GetMovesFromDirection(currX, currY, 1, -1, 1);
		GetMovesFromDirection(currX, currY, -1, -1, 1);
		GetMovesFromDirection(currX, currY, 1, 0, 1); // Right
		GetMovesFromDirection(currX, currY, -1, 0, 1); // Left
		GetMovesFromDirection(currX, currY, 0, 1, 1); // Up
		GetMovesFromDirection(currX, currY, 0, -1, 1); // Down
		GetCastlingMoves();

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		moves.Clear();

		void GetMovesFromDirection(int currX, int currY, int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				if (x < 0 || x > 7 || y < 0 || y > 7) break;
				int pos = y * 8 + x;

				Move m = new Move(CurrPos, pos, this);

				if (!IsLegalMove(m)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos) && !BoardController.i.IsSamePlayer(this.CurrPos, pos)) break;
			}
		}

		GetMovesFromDirection(currX, currY, 1, 1, 1);
		GetMovesFromDirection(currX, currY, -1, 1, 1);
		GetMovesFromDirection(currX, currY, 1, -1, 1);
		GetMovesFromDirection(currX, currY, -1, -1, 1);
		GetMovesFromDirection(currX, currY, 1, 0, 1); // Right
		GetMovesFromDirection(currX, currY, -1, 0, 1); // Left
		GetMovesFromDirection(currX, currY, 0, 1, 1); // Up
		GetMovesFromDirection(currX, currY, 0, -1, 1); // Down
		GetCastlingMoves();

		return moves;
	}

	public List<Move> GetCastlingMoves()
	{
		if (hasMoved) return moves;
		int leftDirection = -1;
		int rightDirection = 1;
		if (IsAbleToCastling(leftDirection))
        {
			int pos = BoardController.i.ConvertToPos(0, currY);
			Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
			if (IsLegalMove(m)) moves.Add(m);
		}
			
		if (IsAbleToCastling(rightDirection))
		{
			int pos = BoardController.i.ConvertToPos(7, currY);
			Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
			if (IsLegalMove(m)) moves.Add(m);
		}

		return moves;
	}

	public bool IsAbleToCastling(int direction)
	{
		if (hasMoved) return false;
		int x = currX;
		Piece foundPiece;
		while (true)
		{
			x += direction;
			int pos = BoardController.i.ConvertToPos(x, currY);
			if (!BoardController.i.IsInBounds(x, currY)) return false;
			Piece piece = BoardController.i.GetPieceFromPos(pos);
			if (piece != null)
			{
				foundPiece = piece;
				break;
			}
		}
		if (foundPiece is Rook rook)
		{
			return !rook.IsMoved();
		}
		return false;
	}

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || BoardController.i.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}

	/// <summary>
	/// Sets the hasMoved boolean in its inital move
	/// </summary>
	public void SetKingBoolean()
	{
		hasMoved = true;
	}

	public void UpdateKingPosition()
	{
		if (GameController.GetCurrPlayer() == PlayerType.Black) BoardController.i.BlackKingPos = CurrPos;
		else BoardController.i.WhiteKingPos = CurrPos;
	}
}
