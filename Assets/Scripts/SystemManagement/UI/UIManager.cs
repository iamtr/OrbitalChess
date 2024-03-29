using UnityEngine;
using UnityEngine.Assertions;

public class UIManager : MonoBehaviour
{
	[SerializeField] private Sprite[] blackSprites;
	[SerializeField] private Sprite[] whiteSprites;
	[SerializeField] private PromotionButton[] promotingBlack;
	[SerializeField] protected PromotionButton[] promotingWhite;
	[SerializeField] private PromotionButton promotionButton;
	[SerializeField] private GameObject whiteBuyOptions;
	[SerializeField] private GameObject blackBuyOptions;
	[SerializeField] private Transform promotionButtonTransform;

	private readonly int promotingNumber = 4;

	[SerializeField] private Piece[] defaultPieceSetup;
	public GameObject checkText;

	protected BoardController bc;
	protected GameController gc;

	protected virtual void Start()
	{
		bc = FindObjectOfType<BoardController>();
		gc = FindObjectOfType<GameController>();


		InstantiatePromotionButtons(blackSprites, promotingBlack);
		InstantiatePromotionButtons(whiteSprites, promotingWhite);

		//AssertAllReferenceIsNotNull();
	}

	protected virtual void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(blackSprites);
		Assert.IsNotNull(whiteSprites);
		Assert.IsNotNull(promotingBlack);
		Assert.IsNotNull(promotingWhite);
		Assert.IsNotNull(promotionButton);
		Assert.IsNotNull(promotionButtonTransform);
		Assert.IsNotNull(whiteBuyOptions);
		Assert.IsNotNull(blackBuyOptions);
		Assert.IsNotNull(defaultPieceSetup);
		Assert.IsNotNull(checkText);
	}

	protected virtual void InstantiatePromotionButtons(Sprite[] sprites, PromotionButton[] buttons)
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
	/// Hides the promotion buttons
	/// </summary>
	public void UnhighlightAllPromotingButtons()
	{
		foreach (var square in promotingBlack) square?.gameObject.SetActive(false);
		foreach (var square in promotingWhite) square?.gameObject.SetActive(false);
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

	//public void ShowPromotionButtonsForMultiplayer()
	//{
	//	if (GameController.GetCurrPlayer() != PlayerManager
	//}

	public void ShowBuyOptions()
	{
		PlayerType p = GameController.GetCurrPlayer();

		// Should not allow to buy if the player is in check
		if (gc.IsCheck) return;

		if (p == PlayerType.White)
		{
			whiteBuyOptions.SetActive(true);
		}
		else
		{
			blackBuyOptions.SetActive(true);
		}
	}

	public void DisableBuyOptions()
	{
		whiteBuyOptions.SetActive(false);
		blackBuyOptions.SetActive(false);
	}



	
}