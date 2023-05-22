using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerType currPlayer;
	[SerializeField] private GameState gameState;

	public static GameController i;

    public PlayerType CurrPlayer => currPlayer;
	public GameState GameState => gameState;

	private void Start()
	{
        if (i != null && i != this) Destroy(this);
        else i = this;
    }
	private void Update()
	{
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            if (collider != null)
            {
                InputManager.i.HandleColliderClicked(collider);
            }
        }
    }

    public void RoundEnd()
	{
        if (currPlayer == PlayerType.Black)
		{
            currPlayer = PlayerType.White;
		} 
        else if (currPlayer == PlayerType.White)
		{
            currPlayer = PlayerType.Black;
		}
	}

	public void SetGameState(GameState newState)
	{
		gameState = newState;
	}
}

public enum PlayerType { Black, White }

public enum GameState { Play, Promoting, GameOver, Pause }
