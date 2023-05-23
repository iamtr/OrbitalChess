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

	// Singleton instance
	public static PawnPromotion i { get; private set; }

	private void Start()
	{
		promotionButtonTransform = GameObject.Find("Promotion Buttons").transform;
		promotingBlack = new PromotionButton[promotingNumber];
		promotingWhite = new PromotionButton[promotingNumber];

		InstantiatePromotionButtons();

		// Singleton initialization
		if (i != null && i != this) Destroy(this);
		else i = this;
	}

	public void InstantiatePromotionButtons()
	{
		InstantiatePromotionButtons(blackSprites, promotingBlack);
		InstantiatePromotionButtons(whiteSprites, promotingWhite);
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

	public Piece FindPromotion(int id, PlayerType player)
	{
		return player == PlayerType.Black ? promotionBlackList[id] : promotionWhiteList[id];
	}

	public void ShowPromotion(Pawn pawn)
	{
		if (pawn.Player == PlayerType.Black)
			for (var i = 0; i < promotingNumber; i++) promotingBlack[i].gameObject.SetActive(true);
		else
			for (var i = 0; i < promotingNumber; i++) promotingWhite[i].gameObject.SetActive(true);
	}

	public void UnhighlightAllPromotingButtons()
	{
		foreach (var square in promotingBlack) square.gameObject.SetActive(false);
		foreach (var square in promotingWhite) square.gameObject.SetActive(false);
	}

	public void PromotePiece(Piece promotedPiece)
	{
		Destroy(BoardController.i.GetPieces()[BoardController.i.CurrPiece.CurrPos].gameObject);
		BoardController.i.InstantiatePiece(promotedPiece, BoardController.i.CurrPiece.CurrPos);
	}
}