/*
To preserve memory during search, moves are stored as 16 bit numbers.
The format is as follows:

bit 0-5: from square (0 to 63)
bit 6-11: to square (0 to 63)
bit 12-15: flag
*/

[System.Serializable]
public readonly struct Move
{
	public readonly struct Flag
	{
		public const int None = 0;
		public const int EnPassantCapture = 1;
		public const int Castling = 2;
		public const int PromoteToQueen = 3;
		public const int PromoteToKnight = 4;
		public const int PromoteToRook = 5;
		public const int PromoteToBishop = 6;
		public const int PawnTwoForward = 7;
	}

	private readonly ushort moveValue;

	private const ushort startSquareMask = 0b000000000001111111;
	private const ushort targetSquareMask = 0b000011111110000000;
	private const ushort flagMask = 0b1111000000000000;

	private readonly Piece piece;

	public Move(int startSquare, int targetSquare, Piece piece)
	{
		moveValue = (ushort)(startSquare | targetSquare << 7);
		this.piece = piece;
	}

	public Move(int startSquare, int targetSquare, Piece piece, int flag)
	{
		moveValue = (ushort)(startSquare | targetSquare << 7 | flag << 14);
		this.piece = piece;
	}

	public int StartSquare
	{
		get
		{
			return moveValue & startSquareMask;
		}
	}

	public int TargetSquare
	{
		get
		{
			return (moveValue & targetSquareMask) >> 7;
		}
	}

	public bool IsPromotion
	{
		get
		{
			int flag = MoveFlag;
			return flag == Flag.PromoteToQueen || flag == Flag.PromoteToRook || flag == Flag.PromoteToKnight || flag == Flag.PromoteToBishop;
		}
	}

	public int MoveFlag
	{
		get
		{
			return moveValue >> 14;
		}
	}

	public static Move InvalidMove
	{
		get
		{
			return new Move(0, 0, null);
		}
	}

	public static bool SameMove(Move a, Move b)
	{
		return a.moveValue == b.moveValue;
	}

	public ushort Value
	{
		get
		{
			return moveValue;
		}
	}

	public bool IsInvalid
	{
		get
		{
			return moveValue == 0;
		}
	}

	public Piece Piece => piece;
}

public enum MoveFlag
{
	None,
	EnPassantCapture,
	KingsideCastling,
	QueensideCastling,
	PromoteToQueen,
	PromoteToKnight,
	PromoteToRook,
	PromoteToBishop,
	PawnTwoForward
}	