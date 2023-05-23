using UnityEngine;

public class PawnPromotion : MonoBehaviour
{
	[SerializeField] private Sprite[] blackSprites;
	[SerializeField] private Sprite[] whiteSprites;
	[SerializeField] private PromotionButton[] promotingBlack;
	[SerializeField] private PromotionButton[] promotingWhite;
	[SerializeField] private Piece[] promotionBlackList;
	[SerializeField] private Piece[] promotionWhiteList;
	[SerializeField] private PromotionButton promotionButton;

	private Transform promotionButtonTransform;
	private readonly int promotingNumber = 4;

	public static PawnPromotion i { get; private set; }

	private void Start()
	{
		promotionButtonTransform = GameObject.Find("Promotion Buttons").transform;
		promotingBlack = new PromotionButton[promotingNumber];
		promotingWhite = new PromotionButton[promotingNumber];

		InstantiatePromotionButtons(blackSprites, promotingBlack);
		InstantiatePromotionButtons(whiteSprites, promotingWhite);

		// Singleton initialization
		if (i != null && i != this) Destroy(this);
		else i = this;
	}

	private void InstantiatePromotionButtons(Sprite[] sprites, PromotionButton[] buttons)
	{
		for (var i = 0; i < promotingNumber; i++)
		{
			buttons[i] = Instantiate(promotionButton, new Vector3(8.5f, 5 - i, 0), Quaternion.identity);
			buttons[i].id = i;
			buttons[i].spriteRen.sprite = sprites[i];
			buttons[i].gameObject.transform.parent = promotionButtonTransform;
			buttons[i].gameObject.transform.localScale = new Vector3(4.55f, 4.55f, 1f);
			buttons[i].GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.2f);
			buttons[i].gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Gets the promoted piece type based on the id given
	/// </summary>
	/// <param name="id">The type of piece to be promoted</param>
	/// <param name="player">Player type</param>
	/// <returns>The promoted piece (Queen, Knight, Rook, Bishop)</returns>
	public Piece GetPromotionPiece(int id, PlayerType player)
	{
		return player == PlayerType.Black ? promotionBlackList[id] : promotionWhiteList[id];
	}

	/// <summary>
	/// Shows the promotion buttons
	/// </summary>
	/// <param name="p">Player Type</param>
	public void ShowPromotionButtons(PlayerType p)
	{
		if (p == PlayerType.Black)
			for (var i = 0; i < promotingNumber; i++) promotingBlack[i].gameObject.SetActive(true);
		else
			for (var i = 0; i < promotingNumber; i++) promotingWhite[i].gameObject.SetActive(true);
	}


	/// <summary>
	/// Hides the promotion buttons
	/// </summary>
	public void UnhighlightAllPromotingButtons()
	{
		foreach (var square in promotingBlack) square.gameObject.SetActive(false);
		foreach (var square in promotingWhite) square.gameObject.SetActive(false);
	}

	/// <summary>
	/// Destroys the pawn and instantiates the promoted piece
	/// </summary>
	/// <param name="promotedPiece">The piece type to be instantiated</param>
	public void PromotePiece(Piece promotedPiece)
	{
		BoardController.i.DestroyPiece(BoardController.i.CurrPiece.CurrPos);
		BoardController.i.InstantiatePiece(promotedPiece, BoardController.i.CurrPiece.CurrPos);
	}

	/// <summary>
	/// Handles the promotion button clicked functionality
	/// </summary>
	/// <param name="col"></param>
	public void HandlePromotionButtonClicked(Collider2D col)
	{
		int id = col.GetComponent<PromotionButton>().id;
		Piece promotedPiece = GetPromotionPiece(id, BoardController.i.CurrPiece.Player);
		PromotePiece(promotedPiece);
		UnhighlightAllPromotingButtons();
		GameController.i.SetGameState(GameState.Play);
	}
}