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

	private GameController gc;
	private BoardController bc;
	private Transform promotionButtonTransform;
	private readonly int promotingNumber = 4;

	// Singleton instance
	public static PawnPromotion i { get; private set; }

	private void Start()
	{
		gc = GameObject.Find("Game Controller").GetComponent<GameController>();
		bc = GameObject.Find("Board").GetComponent<BoardController>();
		promotionButtonTransform = GameObject.Find("Promotion Buttons").transform;
		promotingBlack = new PromotionButton[promotingNumber];
		promotingWhite = new PromotionButton[promotingNumber];

		for (var i = 0; i < promotingNumber; i++)
		{
			promotingBlack[i] = Instantiate(promotionButton, new Vector3(8.5f, 5 - i, 0), Quaternion.identity);
			promotingBlack[i].id = i;
			promotingBlack[i].spriteRen.sprite = blackSprites[i];
			promotingBlack[i].gameObject.transform.parent = promotionButtonTransform;
			promotingBlack[i].gameObject.SetActive(false);

		}

		for (var i = 0; i < promotingNumber; i++)
		{
			promotingWhite[i] = Instantiate(promotionButton, new Vector3(9.5f, 5 - i, 0), Quaternion.identity);
			promotingWhite[i].id = i;
			promotingWhite[i].spriteRen.sprite = whiteSprites[i];
			promotingBlack[i].gameObject.transform.parent = promotionButtonTransform;
			promotingWhite[i].gameObject.SetActive(false);
		}

		// Singleton initialization
		if (i != null && i != this) Destroy(this);
		else i = this;
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

	public void MovePromotedPiece(int x, int y, Piece oldPiece, Piece newPiece)
	{
		var newPos = bc.ConvertToPos(x, y);
		var oldPos = oldPiece.CurrPos;

		bc.RemovePiece(oldPos);
		Destroy(oldPiece.gameObject);

		bc.DestroyOpponentPiece(oldPiece, newPos);
		var temp = bc.InstantiatePiece(newPiece, newPos);

		bc.SetPiecePos(temp, newPos);
		bc.InvokeOnAfterMove(newPos);
		gc.RoundEnd();
	}
}