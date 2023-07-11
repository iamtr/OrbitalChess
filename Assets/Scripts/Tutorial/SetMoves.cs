using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoves : MonoBehaviour
{
	// Start is called before the first frame update
	public List<MoveSimulator> moves;
	public int moveIndex = 0;
	public BoardController bc;

	// If no piece is captured, then piece stored is null	
	public Stack<Piece> capturedPieceStack;

    private void Start()
    {
		bc = FindObjectOfType<BoardController>();
		capturedPieceStack = new Stack<Piece>(moves.Count);
	}

    public void ExecuteMove()
	{
		MoveSimulator m = moves[moveIndex];

		Piece p = bc.GetPieceFromPos(m.end);
		//capturedPieceStack.Push(p);
		if (p != null) capturedPieceStack.Push(p);
		else capturedPieceStack.Push(null);

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
		Piece p = capturedPieceStack.Pop();
		bc.InstantiatePiece(p, m.end);
		Debug.Log(capturedPieceStack.Contains(p));
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
}
