using UnityEngine;
using System;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Players")]
	[SerializeField] private PlayerManager blackPlayer;
	[SerializeField] private PlayerManager whitePlayer;

    [Header("Text")]
	[SerializeField] private TMP_Text checkText;
	[SerializeField] private TMP_Text turnText;

    [SerializeField] private bool isSpecialMode = false;

    [SerializeField] private GameObject replayButton;

    private static PlayerType currPlayer = PlayerType.White;
	private static GameState gameState;
    public bool IsCheck { get; private set; } 

	public static GameController i;

    public bool IsSpecialMode => isSpecialMode;

    /// <summary>
    /// Current player type (Black, White)
    /// </summary>
    public PlayerType CurrPlayer => currPlayer;
    /// <summary>
    /// Current game state (Play, Promoting, Check, etc)
    /// </summary>
	public GameState GameState => gameState;

    public static event Action OnRoundStart;
    public static event Action OnRoundEnd;
    //public static event Action OnGameEnd;

    

	private void OnEnable()
	{
		OnRoundEnd += HandleCheckAndCheckmate;
		OnRoundEnd += SetPlayer;
        OnRoundEnd += InvokeOnRoundStart;
	}

	private void OnDisable()
    {
		OnRoundEnd -= HandleCheckAndCheckmate;
		OnRoundEnd -= SetPlayer;
		OnRoundEnd -= InvokeOnRoundStart;   
	}

	private void Start()
	{
        if (i != null && i != this) Destroy(this);
        else i = this;
        currPlayer = PlayerType.White;
    }
	private void Update()
	{
        if (GameController.GetGameState() == GameState.GameOver)
        {
            replayButton.gameObject.SetActive(true);
            return;
        } else
        {
            replayButton.gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            InputManager.HandleColliderClicked(collider);
        }
    }

    /// <summary>
    /// Sets the current player to the opposite player
    /// </summary>
    public void SetPlayer()
    {
        currPlayer = currPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
        if(IsSpecialMode) turnText.text = currPlayer.ToString() + " Turn";
    }

    /// <summary>
    /// Changes the game state according to the parameter. Is static.
    /// </summary>
    /// <param name="newState">The state to set</param>
	public static void SetGameState(GameState newState)
	{
		gameState = newState;
	}

    public static void SetPlayer(PlayerType player)
    {
        currPlayer = player;
    }

	public static void InvokeOnRoundStart()
	{
		OnRoundStart?.Invoke();
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

    public void HandleCheckAndCheckmate()
    {
        if (BoardController.i.IsCheckmate())
        {
			SetGameState(GameState.GameOver);
            checkText.gameObject.SetActive(true);
            checkText.text = "Checkmate!";
		}
		else if (BoardController.i.IsCheck())
        {
            IsCheck = true;
			checkText.gameObject.SetActive(true);
			checkText.text = "Check!";
		}
        else
        {
            IsCheck = false;
			checkText.gameObject.SetActive(false);
		}
    }

    public static PlayerType GetOpponent()
    {
        return PlayerType.Black == currPlayer ? PlayerType.White : PlayerType.Black;
    }

    public PlayerManager GetCurrPlayerManager()
    {
        return GetCurrPlayer() == PlayerType.Black ? blackPlayer : whitePlayer;
    }

    public PlayerManager GetOpponentPlayerManager()
    {
		return GetCurrPlayer() == PlayerType.Black ? whitePlayer : blackPlayer;
	}
}

public enum PlayerType { Black, White }

public enum GameState { Play, Promoting, GameOver, Pause }
