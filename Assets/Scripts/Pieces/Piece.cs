using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour, ICloneable
{
	[SerializeField] protected int currX;
	[SerializeField] protected int currY;

	[SerializeField] protected int testX;
	[SerializeField] protected int testY;

	protected List<Move> moves = new List<Move>();

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

	//private void OnEnable()
	//{
	//	OnAfterMove += GameController.InvokeOnRoundEnd;
	//}

	//private void OnDisable()
	//{
	//	OnAfterMove -= GameController.InvokeOnRoundEnd;
	//}

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
		currX = BoardController.ConvertToXY(pos)[0];
		currY = BoardController.ConvertToXY(pos)[1];
		CurrPos = pos;
	}

	public void SetTransform()
	{
		transform.position = new Vector3(currX, currY, 2);
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
