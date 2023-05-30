using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private static PlayerType currPlayer;
	[SerializeField] private static GameState gameState;

	public static GameController i;

    /// <summary>
    /// Current player type (Black, White)
    /// </summary>
    public PlayerType CurrPlayer => currPlayer;
    /// <summary>
    /// Current game state (Play, Promoting, Check, etc)
    /// </summary>
	public GameState GameState => gameState;

    public static event Action OnRoundEnd;
    public static event Action OnGameEnd;

	private void OnEnable()
	{
		OnRoundEnd += SetPlayer;
	}

	private void OnDisable()
	{
        OnRoundEnd -= SetPlayer;
	}

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

            if (collider == null) return;
            InputManager.i.HandleColliderClicked(collider);
        }
    }

    /// <summary>
    /// Sets the current player to the opposite player
    /// </summary>
    public void SetPlayer()
    {
        currPlayer = currPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
    }

    /// <summary>
    /// Changes the game state according to the parameter. Is static.
    /// </summary>
    /// <param name="newState">The state to set</param>
	public static void SetGameState(GameState newState)
	{
		gameState = newState;
	}

    public static void InvokeOnRoundEnd() 
    {
        OnRoundEnd?.Invoke();
    }

    public static GameState GetGameState()
    {
        return gameState;
    }

    public static PlayerType GetCurrPlayer()
    {
        return currPlayer;
    }
}

public enum PlayerType { Black, White }

public enum GameState { Play, Promoting, GameOver, Pause }
