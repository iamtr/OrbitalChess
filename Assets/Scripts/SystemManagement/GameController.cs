using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerType currPlayer;
	[SerializeField] private GameState gameState;

    public EnPassant ep;
	public static GameController i;

    /// <summary>
    /// Current player type (Black, White)
    /// </summary>
    public PlayerType CurrPlayer => currPlayer;
    /// <summary>
    /// Current game state (Play, Promoting, Check, etc)
    /// </summary>
	public GameState GameState => gameState;
    public event Action OnRoundEnd;
	private void Start()
	{
        if (i != null && i != this) Destroy(this);
        else i = this;

        BoardController.i.InstantiatePieces();

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

    /// <summary>
    /// Sets the current player to the opposite player
    /// </summary>
    public void SetPlayer()
    {
        currPlayer = currPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
    }

    /// <summary>
    /// Changes the game state according to the parameter
    /// </summary>
    /// <param name="newState">The state to set</param>
	public void SetGameState(GameState newState)
	{
		gameState = newState;
	}

    /// <summary>
    /// Parameterless method to set game state to play
    /// </summary>
    public void SetGameStateToPlay()
	{
		gameState = GameState.Play;
	}

    public void InvokeOnRoundEnd() 
    {
        OnRoundEnd?.Invoke();
        EnPassant.i.InvokeEveryTimer();
    }
}

public enum PlayerType { Black, White }

public enum GameState { Play, Promoting, GameOver, Pause }
