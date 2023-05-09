using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Piece : MonoBehaviour 
{
	[SerializeField] protected BoardController boardController;
	protected int currX;
	protected int currY;

	public int[,] delta;
	private void Start()
	{
		boardController = GameObject.Find("Board").GetComponent<BoardController>();
	}

	public abstract void GetAvailableMoves();
	public void SetCoords(int x, int y)
	{
		currX = x;
		currY = y;
	}
	
}
