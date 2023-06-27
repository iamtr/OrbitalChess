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
	 
	public PieceType[] rank1 = new PieceType[8];
	public PieceType[] rank2 = new PieceType[8];
	public PieceType[] rank3 = new PieceType[8];
	public PieceType[] rank4 = new PieceType[8];
	public PieceType[] rank5 = new PieceType[8];
	public PieceType[] rank6 = new PieceType[8];
	public PieceType[] rank7 = new PieceType[8];
	public PieceType[] rank8 = new PieceType[8];
}
