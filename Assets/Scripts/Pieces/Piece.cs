using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Piece : MonoBehaviour 
{


	[SerializeField] protected BoardController bc;
	protected int currX;
	protected int currY;
	public int CurrPos { get; private set; }
	protected int[,] delta;

	public Action OnMove;

	// Do not change to property! We want this to be serializable
	[SerializeField] protected PlayerType player;

	private void Start()
	{
		bc = GameObject.Find("Board").GetComponent<BoardController>();
		InitPiece(Player);
	}

	public virtual void InitPiece(PlayerType p)
	{
		player = p;
	}

	public abstract void GetAvailableMoves();

	public abstract bool IsLegalMove(int x, int y, Piece p);

	

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
