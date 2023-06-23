using UnityEngine;

/// <summary>
/// A square on the board that handles the final position of the piece during movement
/// </summary>
public class HighlightSquare : MonoBehaviour
{
	public Color Color { get; set; }
	public int Position { get; set; }

	/// <summary>
	/// Each square have a special that indicates special moves
	/// that defers from a regular move.
	/// </summary>
	public SpecialMove Special = SpecialMove.Play;
}

public enum SpecialMove
{ Play, EnPassant, Castling, Bomb, Steal, Spawn, Mine }