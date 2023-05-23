using System;
using UnityEngine;

public abstract class Piece : MonoBehaviour 
{
	[SerializeField] protected BoardController bc;
	[SerializeField] protected int currX;
	[SerializeField] protected int currY;

	/// <summary>
	/// Current position, from 0 - 63
	/// </summary>
	public int CurrPos { get; private set; }

	public event Action OnBeforeMove;
	public event Action OnAfterMove;

	/// <summary>
	/// Current player type
	/// </summary>
	public PlayerType Player => player;

	[SerializeField] protected PlayerType player;

	private void Start()
	{
		bc = GameObject.Find("Board").GetComponent<BoardController>();
		InitPiece(Player);

		OnAfterMove += GameController.i.InvokeOnRoundEnd;
	}

	/// <summary>
	/// Initializes the piece
	/// </summary>
	public virtual void InitPiece(PlayerType p)
	{
		SetPlayer(p);
	}

	/// <summary>
	/// Calculates all available moves for this piece and highlights them
	/// </summary>
	public abstract void GetAvailableMoves();

	/// <summary>
	/// Checks if the move is legal
	/// </summary>
	public abstract bool IsLegalMove(int x, int y, Piece p);

	/// <summary>
	/// Set the currX and currY values of this piece
	/// </summary>
	/// <param name="x"> currX </param>
	/// <param name="y"> currY </param>
	public void SetCoords(int x, int y) { 
		currX = x; 
		currY = y;
		CurrPos = y * 8 + x;
		transform.position = new Vector3(currX, currY, 0);

	}
	/// <summary>
	/// Set the player type for this piece
	/// </summary>
	/// <param name="p">Player type</param>
	public void SetPlayer(PlayerType p) => player = p;

	/// <summary>
	/// Calls the OnBeforeMove event
	/// </summary>
	public void InvokeOnBeforeMove()
	{
		OnBeforeMove?.Invoke();
	}

	/// <summary>
	/// Calls the OnAfterMove event
	/// </summary>
	public void InvokeOnAfterMove()
	{
		OnAfterMove?.Invoke();
	}
}
