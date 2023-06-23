using UnityEngine;

public abstract class Card : MonoBehaviour
{
	public PlayerType player;

	public abstract void Trigger();

	public void OnMouseOver()
	{
		transform.localScale = new Vector3(1, 1, 0);
	}

	public void OnMouseExit()
	{
		transform.localScale = new Vector3(0.5f, 0.5f, 0);
	}

	public void OnMouseUp()
	{
		if (GameController.GetCurrPlayer() == player) Trigger();
	}

	public void SetCardPlayer(PlayerType p)
	{
		player = p;
	}
}