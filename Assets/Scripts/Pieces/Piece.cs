using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour, ICloneable
{
	[SerializeField] protected int currX;
	[SerializeField] protected int currY;

	[SerializeField] protected List<Move> moves = new List<Move>();

	/// <summary>
	/// Current position, from 0 - 63
	/// </summary>
	public int CurrPos;

	/// <summary>
	/// Events that is called before and after a movement is made respectively
	/// </summary>
	public event Action OnBeforeMove;
	public event Action OnAfterMove;

	/// <summary>
	/// Current player type
	/// </summary>
	public PlayerType Player => player;

	[SerializeField] protected PlayerType player;

	public static bool isBlackBelow = true;

	private void Awake()
	{
		InitPiece(Player);
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
	public abstract List<Move> GetLegalMoves();

	/// <summary>
	/// Calculates all available moves for this piece, regardless if it results on a check to own king.
	/// </summary>
	/// <returns></returns>
	public abstract List<Move> GetAllMoves();

	/// <summary>
	/// Checks if the move is legal
	/// </summary>
	public abstract bool IsLegalMove(Move m);


	/// <summary>
	/// Set the currX and currY values of this piece
	/// </summary>
	/// <param name="pos"></param>
	public void SetCoords(int pos)
	{
		currX = BoardController.ConvXY(pos)[0];
		currY = BoardController.ConvXY(pos)[1];
		CurrPos = pos;
	}

	public void SetTransform()
	{
		int xPosition;
		int yPosition;
        if (isBlackBelow)
        {
			xPosition = currX;
			yPosition = currY;
        } else
        {
			xPosition = 7 - currX;
			yPosition = 7 - currY;
		}
		transform.position = new Vector3(xPosition, yPosition, 2);
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

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
