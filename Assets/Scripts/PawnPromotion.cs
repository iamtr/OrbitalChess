using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPromotion : MonoBehaviour
{
    [SerializeField] private Sprite[] promotingBlack;
    [SerializeField] private Sprite[] promotingWhite;
	[SerializeField] private PromotionButton[] promotionButtons;
	[SerializeField] private Piece[] promotionBlackList;
	[SerializeField] private Piece[] promotionWhiteList;
	[SerializeField] private PromotionButton promotionButton;

    private GameController gc;

    void Start()
    {
		gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        for (int i = 0; i < promotionButtons.Length; i++)
        {
			promotionButtons[i] = Instantiate(promotionButton, new Vector3(8.5f, 5 - i, 0), Quaternion.identity);
			promotionButtons[i].id = i;
			promotionButtons[i].gameObject.SetActive(false);
		}
	}

    public Piece FindPromotion(int id, PlayerType player)
	{
		if(player == PlayerType.Black)
        {
			return promotionBlackList[id];
        } 
		else
        {
			return promotionWhiteList[id];
		}
	}

	public void ShowPromotion(Pawn pawn)
	{
		if (pawn.Player == PlayerType.Black)
		{
			ShowPromotionHelper(promotingBlack);
		}
		else
		{
			ShowPromotionHelper(promotingWhite);
		}
	}

	public void ShowPromotionHelper(Sprite[] sprites)
    {
		for (int i = 0; i < sprites.Length; i++)
		{
			Instantiate(sprites[i], new Vector3(8.5f, 5 - i, 0), Quaternion.identity);
			promotionButtons[i].gameObject.SetActive(true);
		}
	}

	public bool IsPromoting(Piece piece, int y)
    {
		if (piece is Pawn)
		{
			Pawn temp = (Pawn)piece;
			if (temp.IsPromoted(y))
			{
				return true;
			}
		}
		return false;
	}

    public void UnhighlightAllPromotingButtons()
    {
		foreach (var square in promotionButtons)
		{
			square.gameObject.SetActive(false);
		}
	}
}
