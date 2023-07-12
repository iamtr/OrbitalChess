using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
	[Header("Players")]
	[SerializeField] private SpecialPlayerManager blackPlayer;

	[SerializeField] private SpecialPlayerManager whitePlayer;

	[Header("Text")]
	[SerializeField] private TMP_Text checkText;

	[SerializeField] protected TMP_Text turnText;

	[SerializeField] private bool isSpecialMode = false;

	[SerializeField] private GameObject replayButton;

	[SerializeField] private PlayerType currPlayerRef;

	[SerializeField] private bool isDoubleTurn = false;

	protected static PlayerType currPlayer = PlayerType.White;
	private static GameState gameState;
	
	public bool IsCheck { get; private set; }

	protected BoardController bc;
	protected InputManager im;

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

	public static event Action OnGameOver;

	protected virtual void OnEnable()
	{
		OnRoundEnd += HandleCheckAndCheckmate;
		OnRoundEnd += SetPlayer;
		OnRoundEnd += InvokeOnRoundStart;
	}

	protected virtual void OnDisable()
	{
		OnRoundEnd -= HandleCheckAndCheckmate;
		OnRoundEnd -= SetPlayer;
		OnRoundEnd -= InvokeOnRoundStart;
	}

	protected virtual void Start()
	{
		bc = FindObjectOfType<BoardController>();
		im = FindObjectOfType<InputManager>();

		currPlayer = PlayerType.White;
		gameState = GameState.Play;

		AssertAllReferenceIsNotNull();
	}

	private void AssertAllReferenceIsNotNull()
    {
		Assert.IsNotNull(checkText);
		// Assert.IsNotNull(replayButton);
		Assert.IsNotNull(turnText);
	}

	/// <summary>
	/// Sets the current player to the opposite player
	/// </summary>
	public virtual void SetPlayer()
	{
		if (isDoubleTurn)
		{
			isDoubleTurn = false;
			return;
		}

		currPlayer = currPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
		turnText.text = currPlayer.ToString() + " Turn";
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

	public virtual void HandleCheckAndCheckmate()
	{
		if (bc.IsCheckmate())
		{
			SetGameState(GameState.GameOver);
			checkText.gameObject.SetActive(true);
			checkText.text = "Checkmate!";
		}
		else if (bc.IsCheck())
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

	public SpecialPlayerManager GetCurrPlayerManager()
	{
		return GetCurrPlayer() == PlayerType.Black ? blackPlayer : whitePlayer;
	}

	public SpecialPlayerManager GetOpponentPlayerManager()
	{
		return GetCurrPlayer() == PlayerType.Black ? whitePlayer : blackPlayer;
	}

	public void HandleGameOver(PlayerType winner)
	{
		SetGameState(GameState.GameOver);
		checkText.gameObject.SetActive(true);
		checkText.text = winner.ToString() + " Wins!";
		replayButton.SetActive(true);
	}

	/// <summary>
	/// Reset the game to the initial state
	/// </summary>
	public virtual void ResetGame()
	{	
		blackPlayer?.ResetPlayerManager();
		whitePlayer?.ResetPlayerManager();
		SetGameState(GameState.Play);
		SetPlayer(PlayerType.White);
		checkText.gameObject.SetActive(false);
		turnText.text = currPlayer.ToString() + " Turn";
	}

	public void SetCheckText(string text)
	{
		checkText.text = text;
	}
	/// <summary>
	/// For card mode: Allows the player to move double turns for one round
	/// Sets isDoubleTurn boolean to be true
	/// </summary>
	/// <param name="boolean"></param>
	public void SetDoubleTurn(bool boolean)
	{
		isDoubleTurn = boolean;
	}
}

public enum PlayerType
{ Black, White }

public enum GameState
{ Play, Promoting, GameOver, Pause }