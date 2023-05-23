using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerType currPlayer;
	[SerializeField] private GameState gameState;

	public static GameController i;

    public PlayerType CurrPlayer => currPlayer;
	public GameState GameState => gameState;
    public event Action OnRoundEnd;
	private void Start()
	{
        if (i != null && i != this) Destroy(this);
        else i = this;

        OnRoundEnd += SetPlayer;
    }
	private void Update()
	{
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            if (collider == null) return;
            InputManager.i.HandleColliderClicked(collider);
        }
    }

    public void SetPlayer()
    {
        currPlayer = currPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
    }

	public void SetGameState(GameState newState)
	{
		gameState = newState;
	}

    public void SetGameStateToPlay()
	{
		gameState = GameState.Play;
	}

    public void InvokeOnRoundEnd() 
    {
        OnRoundEnd?.Invoke();
    }
}

public enum PlayerType { Black, White }

public enum GameState { Play, Promoting, GameOver, Pause }
