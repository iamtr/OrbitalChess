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
	private readonly int promotingNumber = 4;

	private void Start()
	{
		gc = GameObject.Find("Game Controller").GetComponent<GameController>();
		bc = GameObject.Find("Board").GetComponent<BoardController>();
		promotingBlack = new PromotionButton[promotingNumber];
		promotingWhite = new PromotionButton[promotingNumber];
		for (var i = 0; i < promotingNumber; i++)
		{
			promotingBlack[i] = Instantiate(promotionButton, new Vector3(8.5f, 5 - i, 0), Quaternion.identity);
			promotingBlack[i].id = i;
			promotingBlack[i].spriteRen.sprite = blackSprites[i];
			promotingBlack[i].gameObject.SetActive(false);
		}

		for (var i = 0; i < promotingNumber; i++)
		{
			promotingWhite[i] = Instantiate(promotionButton, new Vector3(9.5f, 5 - i, 0), Quaternion.identity);
			promotingWhite[i].id = i;
			promotingWhite[i].spriteRen.sprite = whiteSprites[i];
			promotingWhite[i].gameObject.SetActive(false);
		}
	}

	public Piece FindPromotion(int id, PlayerType player)
	{
		if (player == PlayerType.Black)
			return promotionBlackList[id];
		return promotionWhiteList[id];
	}

	public void ShowPromotion(Pawn pawn)
	{
		if (pawn.Player == PlayerType.Black)
			ShowPromotionHelper(promotingBlack);
		else
			ShowPromotionHelper(promotingWhite);
	}

	public void ShowPromotionHelper(PromotionButton[] list)
	{
		for (var i = 0; i < promotingNumber; i++) list[i].gameObject.SetActive(true);
	}

	public bool IsPromoting(Piece piece, int y)
	{
		if (piece is Pawn)
		{
			Pawn temp = (Pawn)piece;
			if (temp.IsPromoted(y)) return true;
		}

		return false;
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
		bc.InvokeMove(newPos);
		gc.RoundEnd();
	}
}