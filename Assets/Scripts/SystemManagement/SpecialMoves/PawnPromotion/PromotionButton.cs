using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionButton : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRen;
    [SerializeField] public BoxCollider2D Collider { get; set; }
    [SerializeField] public int id { get; set; }
}
