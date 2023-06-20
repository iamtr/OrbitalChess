using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
	public abstract void Trigger();

	public void OnMouseOver()
	{
		transform.localScale = new Vector3(1, 1, 0);
	}

	public void OnMouseExit()
	{
		transform.localScale = new Vector3(0.5f, 0.5f, 0);
	}
}
