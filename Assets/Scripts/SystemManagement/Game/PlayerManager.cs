using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager: MonoBehaviour
{
	[SerializeField] protected PlayerType player;
	public PlayerType Player { get => player; set => player = value; }
}
