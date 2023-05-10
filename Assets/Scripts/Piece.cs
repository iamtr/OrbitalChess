using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Piece : MonoBehaviour 
{
	public enum PlayerType { Black, White }

	[SerializeField] protected BoardController boardController;
	protected int currX;
	protected int currY;
	public int CurrPos { get; private set; }
	protected int[,] delta;
	[SerializeField] protected PlayerType player;

	private void Start()
	{
		boardController = GameObject.Find("Board").GetComponent<BoardController>();
		InitPiece(Player);
	}

	public abstract void InitPiece(PlayerType p);

	public void GetAvailableMoves()
	{
		for (int i = 0; i < delta.Length - 1; i++)
		{
			int x = delta[i, 0];
			int y = delta[i, 1];

			if (player == PlayerType.Black)
			{
				if (boardController.IsLegalMove(currX + x, currY + y))
				{
					boardController.Highlight(currX + x, currY + y, this);
				}
			}
			else if (player == PlayerType.White)
			{
				if (boardController.IsLegalMove(currX - x, currY - y))
				{
					boardController.Highlight(currX - x, currY - y, this);
				}
			}

		}
	}

	// Getters
	public PlayerType Player => player;

	// Setters
	public void SetCoords(int x, int y) { 
		currX = x; 
		currY = y;
		CurrPos = y * 8 + x;
		transform.position = new Vector3(currX, currY, 0);

	}
	public void SetPlayer(PlayerType p) => player = p;



	
	
}
