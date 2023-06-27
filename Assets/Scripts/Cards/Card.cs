using UnityEngine;

public abstract class Card : MonoBehaviour, ITrigger
{
	public PlayerType player;
	protected BoardController bc;
	protected HighlightManager hm;
	public abstract void Trigger();

	private void Start()
	{
		bc = FindObjectOfType<BoardController>();
	}

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