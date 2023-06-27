using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PositionSO : ScriptableObject
{
	public enum PieceType
	{
		None, Pawn, Knight, Bishop, Rook, Queen, King
	}

	public PieceType[] rank1;
	public PieceType[] rank2;
	public PieceType[] rank3;
	public PieceType[] rank4;
	public PieceType[] rank5;
	public PieceType[] rank6;
	public PieceType[] rank7;
	public PieceType[] rank8;
}
