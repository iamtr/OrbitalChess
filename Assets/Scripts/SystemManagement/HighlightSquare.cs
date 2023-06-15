using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A square on the board that handles the final position of the piece during movement
/// </summary>
public class HighlightSquare : MonoBehaviour
{
    [SerializeField] public Color Color { get; set; }
    [SerializeField] public BoxCollider2D Collider { get; set; }
    [SerializeField] public int Position { get; set; }

    /// <summary>
    /// Each square have a special that indicates special moves 
    /// that defers from a regular move.
    /// </summary>
    public SpecialMove Special = SpecialMove.Play;
}

public enum SpecialMove {Play, EnPassant, Castling, Bomb}
