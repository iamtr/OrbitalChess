using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMove : MonoBehaviour
{
	// Start is called before the first frame update
	public List<MoveSimulator> moves;
	public int moveIndex = 0;
	public BoardController bc;

	[SerializeField] private Piece[] respawnPiece;

	// If no piece is captured, then piece stored is null	
	public Stack<int> capturedPieceStack;

    private void Start()
    {
		bc = FindObjectOfType<BoardController>();
		capturedPieceStack = new Stack<int>(moves.Count);
	}

    public void ExecuteMove()
	{
		MoveSimulator m = moves[moveIndex];

		Piece p = bc.GetPieceFromPos(m.end);
		PushStack(p);

		if (m.flag == MoveFlag.KingsideCastling || m.flag == MoveFlag.QueensideCastling)
		{
			// Handle castling
		}

		bc.CurrPiece = bc.GetPieceFromPos(m.start);
		int x = BoardController.ConvXY(m.end)[0];
		int y = BoardController.ConvXY(m.end)[1];
		bc.MovePiece(x, y, bc.GetPieceFromPos(m.start));

		moveIndex++;
		Debug.Log(capturedPieceStack.Peek());
	}

	public void PreviousMove()
	{
		if (moveIndex == 0) return;
		moveIndex--;
		MoveSimulator m = moves[moveIndex];

		bc.CurrPiece = bc.GetPieceFromPos(m.start);
		int x = BoardController.ConvXY(m.start)[0];
		int y = BoardController.ConvXY(m.start)[1];

		bc.MovePiece(x, y, bc.GetPieceFromPos(m.end));


		if (m.flag == MoveFlag.KingsideCastling)
		{
			bc.MovePiece(x + 3, y, bc.GetPieceFromPos(m.end + 1));
		}

		else if (m.flag == MoveFlag.QueensideCastling)
		{
			bc.MovePiece(x - 4, y, bc.GetPieceFromPos(m.end - 1));
		}

		Debug.Log(capturedPieceStack.Peek());
		Piece p = respawnPiece[capturedPieceStack.Pop()];
		bc.InstantiatePiece(p, m.end);
		//if (p == null) return;
		//else bc.InstantiatePiece(p, m.end);
	}

	public void UndoAllPrevMoves()
	{
        for (int i = 0; i < moves.Count; i++)
		{
            PreviousMove();
		}
	}

	public void ExecuteAllMoves()
    {
		for (int i = 0; i < moves.Count; i++)
		{
			ExecuteMove();
		}
	}

	public void PushStack(Piece piece)
    {
        if (piece == null)
        {
			capturedPieceStack.Push(0);
			return;
		}

		int index;
        if (piece is Pawn)
        {
			if (piece.Player == PlayerType.White) index = 1;
			else index = 2;
        }
		else if (piece is Knight)
		{
			if (piece.Player == PlayerType.White) index = 3;
			else index = 4;
		}
		else if (piece is Rook)
		{
			if (piece.Player == PlayerType.White) index = 5;
			else index = 6;
		}
		else if (piece is Bishop)
		{
			if (piece.Player == PlayerType.White) index = 7;
			else index = 8;
		}
		else if (piece is Queen)
		{
			if (piece.Player == PlayerType.White) index = 9;
			else index = 10;
		}
		else
        {
			//This line should not reached
			index = 0;
        }

		capturedPieceStack.Push(index);
	}
}
