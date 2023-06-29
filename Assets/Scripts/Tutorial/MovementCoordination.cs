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

	private void Awake()
	{
		bc = FindObjectOfType<BoardController>();
	}

	public void ExecuteMove()
    {
		MoveSimulator m = moves[moveIndex];

		if (m.flag == MoveFlag.Castling)
		{
			bc.MoveCastling(m.start, m.end, bc.GetPieceFromPos(moveIndex));
		}
		
		bc.CurrPiece = bc.GetPieceFromPos(m.start);
		int x = BoardController.ConvXY(m.end)[0];
		int y = BoardController.ConvXY(m.end)[1];
		bc.MovePiece(x, y, bc.GetPieceFromPos(m.start));
    }
}

[System.Serializable]
public class MoveSimulator
{
    [SerializeField] public int start;
	[SerializeField] public int end;
	[SerializeField] public MoveFlag flag;
}