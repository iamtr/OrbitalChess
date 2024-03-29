using UnityEngine;

public abstract class Card : MonoBehaviour, ITrigger
{
	public PlayerType player;

	protected BoardController bc;
	protected HighlightManager hm;
	protected GameController gc;
	public int currIndex { get; private set; }
	public abstract void Trigger();

	private void Start()
	{
		bc = FindObjectOfType<BoardController>();
		hm = FindObjectOfType<HighlightManager>();
		gc = FindObjectOfType<GameController>();
	}

	public void OnMouseOver()
	{
		transform.localScale = new Vector3(1, 1, 0);
	}

	public void OnMouseExit()
	{
		transform.localScale = new Vector3(0.5f, 0.5f, 0);
	}

	public void SetCardPlayer(PlayerType p)
	{
		player = p;
	}

	public void SetCurrIndex(int index)
	{
		currIndex = index;
	}
}