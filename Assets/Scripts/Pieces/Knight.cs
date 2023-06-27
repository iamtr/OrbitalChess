using System.Collections.Generic;

public class Knight : Piece
{
	protected override void Awake()
	{
		base.Awake();
		value = 30;
	}

	private int[,] deltas = new int[,] { { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 }, { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 } };

	private void OnEnable()
	{
		OnAfterMove += GameController.InvokeOnRoundEnd;
	}

	private void OnDisable()
	{
		OnAfterMove -= GameController.InvokeOnRoundEnd;
	}

	public override List<Move> GetLegalMoves()
	{
		moves.Clear();

		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];
			if (currX + deltaX < 0 || currY + deltaY < 0 || currX + deltaX > 7 || currY + deltaY > 7) continue;
			int newPos = bc.ConvPos(currX + deltaX, currY + deltaY);

			Move m = new Move(CurrPos, newPos, this);

			if (IsLegalMove(m) && !bc.IsBeingCheckedAfterMove(m, Player))
			{
				moves.Add(m);
			}
		}

		return moves;
	}

	public override List<Move> GetAllMoves()
	{
		moves.Clear();

		for (int i = 0; i < deltas.GetLength(0); i++)
		{
			int deltaX = deltas[i, 0];
			int deltaY = deltas[i, 1];
			if (currX + deltaX < 0 || currY + deltaY < 0 || currX + deltaX > 7 || currY + deltaY > 7) continue;
			int newPos = bc.ConvPos(currX + deltaX, currY + deltaY);

			Move m = new Move(CurrPos, newPos, this);

			if (IsLegalMove(m)) moves.Add(m);
		}

		return moves;
	}

	public override bool IsLegalMove(Move move)
	{
		if (move.TargetSquare < 0 || move.TargetSquare > 63 || bc.IsSamePlayer(CurrPos, move.TargetSquare)) return false;
		return true;
	}
}