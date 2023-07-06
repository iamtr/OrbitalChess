using System.Collections.Generic;

public class Rook : Piece
{
	protected override void Awake()
	{
		base.Awake();
		value = 50;
	}

	/// <summary>
	/// A boolean to check whether the pawn has moved from its initial position
	/// The boolean is set to false by default
	/// </summary>
	private bool hasMoved = false;

	private void OnEnable()
	{
		OnAfterMove += SetRookBoolean;
		OnAfterMove += GameController.InvokeOnRoundEnd;
	}

	private void OnDisable()
	{
		OnAfterMove -= SetRookBoolean;
		OnAfterMove -= GameController.InvokeOnRoundEnd;
	}

	public override void InitPiece(PlayerType p)
	{
		base.InitPiece(p);
	}

	public override List<Move> GetLegalMoves()
	{
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

				if (!IsLegalMove(m) || bc.IsBeingCheckedAfterMove(m, Player)) break;
				moves.Add(m);
				if (bc.IsOccupied(pos)) break;
			}
		}

		GetMovesFromDirection(1, 0, 8);
		GetMovesFromDirection(-1, 0, 8);
		GetMovesFromDirection(0, 1, 8);
		GetMovesFromDirection(0, -1, 8);

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
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

				if (bc.IsSamePlayerAtTestArray(CurrPos, pos)) break;
				moves.Add(m);
				if (bc.TestArrayIsOccupied(pos)) break;
			}
		}

		GetMovesFromDirection(1, 0, 8);
		GetMovesFromDirection(-1, 0, 8);
		GetMovesFromDirection(0, 1, 8);
		GetMovesFromDirection(0, -1, 8);

		return moves;
	}

	public override bool IsLegalMove(Move move)
	{
		if (bc.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}

	public bool IsMoved()
	{
		return hasMoved;
	}

	/// <summary>
	/// Sets the HasMoved boolean in its initial move
	/// </summary>
	public void SetRookBoolean()
	{
		hasMoved = true;
	}
}