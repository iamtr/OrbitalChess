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
		List<Move> GetLegalCastlingMoves()
		{
			if (hasMoved) return moves;
			int leftDirection = -1;
			int rightDirection = 1;
			if (IsAbleToCastle(leftDirection))
			{
				int pos = BoardController.i.ConvPos(0, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (!BoardController.i.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			if (IsAbleToCastle(rightDirection))
			{
				int pos = BoardController.i.ConvPos(7, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (BoardController.i.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			return moves;
		}

		moves.Clear();

		void GetMovesFromDirection(int dx, int dy, int maxDistance)
		{
			for (int i = 1; i <= maxDistance; i++)
			{
				int x = currX + i * dx;
				int y = currY + i * dy;
				if (x < 0 || x > 7 || y < 0 || y > 7) break;
				int pos = y * 8 + x;

				Move m = new Move(CurrPos, pos, this);

				if (BoardController.i.IsBeingCheckedAfterMove(m, Player) || BoardController.i.IsSamePlayer(CurrPos, pos)) break;
				moves.Add(m);
				if (BoardController.i.IsOccupied(pos))
				{
					break;
				}
			}
		}

		GetMovesFromDirection(1, 1, 1);
		GetMovesFromDirection(-1, 1, 1);
		GetMovesFromDirection(	1, -1, 1);
		GetMovesFromDirection(-1, -1, 1);
		GetMovesFromDirection(1, 0, 1); // Right
		GetMovesFromDirection(-1, 0, 1); // Left
		GetMovesFromDirection(0, 1, 1); // Up
		GetMovesFromDirection(0, -1, 1); // Down
		GetLegalCastlingMoves();

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		List<Move> GetAllCastlingMoves()
		{
			if (hasMoved) return moves;
			int leftDirection = -1;
			int rightDirection = 1;
			if (IsAbleToCastle(leftDirection))
			{
				int pos = BoardController.i.ConvPos(0, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (IsLegalMove(m)) moves.Add(m);
			}

			if (IsAbleToCastle(rightDirection))
			{
				int pos = BoardController.i.ConvPos(7, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (IsLegalMove(m)) moves.Add(m);
			}

			return moves;
		}

		moves.Clear();

		void GetMovesFromDirection(int dx, int dy, int maxDistance)
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
				if (BoardController.i.TestArrayIsOccupied(pos)) break;
			}
		}

		GetMovesFromDirection(1, 1, 1);
		GetMovesFromDirection(-1, 1, 1);
		GetMovesFromDirection(1, -1, 1);
		GetMovesFromDirection(-1, -1, 1);
		GetMovesFromDirection(1, 0, 1); // Right
		GetMovesFromDirection(-1, 0, 1); // Left
		GetMovesFromDirection(0, 1, 1); // Up
		GetMovesFromDirection(0, -1, 1); // Down
		GetAllCastlingMoves();

		return moves;
	}

	public bool IsAbleToCastle(int direction)
	{
		if (hasMoved) return false;

		int x = currX + direction;
		int pos;
		while (BoardController.i.IsInBounds(x, currY))
		{
			pos = BoardController.i.ConvPos(x, currY);
			Piece piece = BoardController.i.GetPieceFromPos(pos);
			if (piece != null)
			{
				if (piece is Rook rook && !rook.IsMoved())
				{
					return true;
				}
				break;
			}
			x += direction;
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
