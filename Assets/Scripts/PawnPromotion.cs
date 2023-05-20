using System;
using System.Collections;
using System.Collections.Generic;
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

	void Start()
    {
		gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        for (int i = 0; i < promotingBlack.Length; i++)
        {
			promotingBlack[i] = Instantiate(promotionButton, new Vector3(8.5f, 5 - i, 0), Quaternion.identity);
			promotingBlack[i].id = i;
			promotingBlack[i].spriteRen.sprite = blackSprites[i];
			promotingBlack[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < promotingWhite.Length; i++)
		{
			promotingWhite[i] = Instantiate(promotionButton, new Vector3(9.5f, 5 - i, 0), Quaternion.identity);
			promotingWhite[i].id = i;
			promotingBlack[i].spriteRen.sprite = whiteSprites[i];
			promotingWhite[i].gameObject.SetActive(false);
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

	public void ShowPromotionHelper(PromotionButton[] list)
    {
		for (int i = 0; i < list.Length; i++)
		{
			list[i].gameObject.SetActive(true);
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
		foreach (var square in promotingBlack)
		{
			square.gameObject.SetActive(false);
		}
		foreach (var square in promotingWhite)
		{
			square.gameObject.SetActive(false);
		}
	}
}
