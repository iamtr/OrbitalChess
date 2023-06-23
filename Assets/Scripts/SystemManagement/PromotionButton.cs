using UnityEngine;

/// <summary>
/// A button that is exclusively for promoting a piece
/// </summary>
public class PromotionButton : MonoBehaviour
{
	[SerializeField] public SpriteRenderer spriteRen;
	[SerializeField] public BoxCollider2D Collider { get; set; }
	[SerializeField] public int id { get; set; }
}