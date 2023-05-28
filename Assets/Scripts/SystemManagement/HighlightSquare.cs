using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSquare : MonoBehaviour
{
    [SerializeField] public Color Color { get; set; }
    [SerializeField] public BoxCollider2D Collider { get; set; }
    [SerializeField] public int Position { get; set; }

    public HighlightType Special { get; set; } = HighlightType.Play;
}

public enum HighlightType { Play, EnPassant, Castling}
