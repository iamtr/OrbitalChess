using System.Collections.Generic;

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
			const int leftDirection = -1;
			const int rightDirection = 1;
			if (IsAbleToCastle(leftDirection))
			{
				int pos = bc.ConvPos(1, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (!bc.IsBeingCheckedAfterMove(m, Player)) moves.Add(m);
			}

			if (IsAbleToCastle(rightDirection))
			{
				int pos = bc.ConvPos(5, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (!bc.IsBeingCheckedAfterMove(m, Player))
				{
					moves.Add(m);
				}
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

				if (bc.IsBeingCheckedAfterMove(m, Player) || bc.IsSamePlayer(CurrPos, pos)) break;
				moves.Add(m);
				if (bc.IsOccupied(pos))
				{
					break;
				}
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
		GetLegalCastlingMoves();

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		List<Move> GetAllCastlingMoves()
		{
			if (hasMoved) return moves;
			const int leftDirection = -1;
			const int rightDirection = 1;
			if (IsAbleToCastle(leftDirection))
			{
				int pos = bc.ConvPos(1, currY);
				Move m = new Move(CurrPos, pos, this, Move.Flag.Castling);
				if (IsLegalMove(m)) moves.Add(m);
			}

			if (IsAbleToCastle(rightDirection))
			{
				int pos = bc.ConvPos(5, currY);
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
				if (bc.TestArrayIsOccupied(pos)) break;
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
        while (bc.IsInBounds(x, currY))
		{
            int pos;
			pos = bc.ConvPos(x, currY);
			Piece piece = bc.GetPieceFromPos(pos);
			if (piece != null)
			{
				if (piece is Rook rook && !rook.IsMoved() && rook.Player == this.Player)
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
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || bc.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}

	/// <summary>
	/// Sets the HasMoved boolean in its initial move
	/// </summary>
	public void SetKingBoolean()
	{
		hasMoved = true;
	}

	public void UpdateKingPosition()
	{
		bc.UpdateKingPosition(GameController.GetCurrPlayer(), CurrPos);
	}
}