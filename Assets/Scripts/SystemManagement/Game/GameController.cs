using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
	[Header("Players")]
	[SerializeField] private SpecialPlayerManager blackPlayer;

	[SerializeField] private SpecialPlayerManager whitePlayer;

	[Header("Text")]
	[SerializeField] private TMP_Text checkText;

	[SerializeField] private TMP_Text turnText;

	[SerializeField] private bool isSpecialMode = false;

	[SerializeField] private GameObject replayButton;

	private static PlayerType currPlayer = PlayerType.White;
	private static GameState gameState;
	public bool IsCheck { get; private set; }

	[SerializeField] protected BoardController bc;
	[SerializeField] protected InputManager im;

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

	public virtual void Start()
	{
		bc = FindObjectOfType<BoardController>();
		im = FindObjectOfType<InputManager>();

		// currPlayer = PlayerType.White;

		AssertAllReferenceIsNotNull();
	}

	private void AssertAllReferenceIsNotNull()
    {
		Assert.IsNotNull(checkText);
		Assert.IsNotNull(replayButton);
        if (isSpecialMode)
        {
			Assert.IsNotNull(turnText);
		}
	}

	private void Update()
	{
		if (GetGameState() == GameState.GameOver)
		{
			replayButton.gameObject.SetActive(true);
			return;
		}

        replayButton.gameObject.SetActive(false);

        if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D collider = Physics2D.OverlapPoint(mousePosition);

			im.HandleColliderClicked(collider);
		}
	}

	/// <summary>
	/// Sets the current player to the opposite player
	/// </summary>
	public virtual void SetPlayer()
	{
		currPlayer = currPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
		if (IsSpecialMode) turnText.text = currPlayer.ToString() + " Turn";
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

	public void ResetPlayer()
	{
		blackPlayer?.ResetPlayerManager();
		whitePlayer?.ResetPlayerManager();
	}
}

public enum PlayerType
{ Black, White }

public enum GameState
{ Play, Promoting, GameOver, Pause }