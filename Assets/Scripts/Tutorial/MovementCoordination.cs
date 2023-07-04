using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementCoordination : MonoBehaviour
{
    // Start is called before the first frame update
    public List<MoveSimulator> moves;
    public int moveIndex = 0;
	public BoardController bc;

	// If no piece is captured, then piece stored is null	
	public Stack<Piece> capturedPieceStack = new Stack<Piece>();

	private void Awake()
	{
		bc = FindObjectOfType<BoardController>();
		//ColorOptionDropdown.Dropdown(1);
	}

	public void ExecuteMove()
    {
		MoveSimulator m = moves[moveIndex];

		Piece p = bc.GetPieceFromPos(m.end);
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


		Piece p = capturedPieceStack.Pop();
		if (p == null) return;
		else bc.InstantiatePiece(p, m.end);

	}
}

[System.Serializable]
public class MoveSimulator
{
    [SerializeField] public int start;
	[SerializeField] public int end;
	[SerializeField] public MoveFlag flag;
}