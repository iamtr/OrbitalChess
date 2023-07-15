using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class SpecialPlayerManager : PlayerManager
{
	[SerializeField] private int money;
	[SerializeField] private TMP_Text moneyText;
	[SerializeField] private List<Card> playerCards;
	[SerializeField] private Transform cardTransform;

	public List<Card> PlayerCards { get => playerCards; set => playerCards = value; }	
	public int Money { get => money; set => money = value; }


	private void Start()
	{
		moneyText.text = "Coin: " + money;

		AssertAllReferenceIsNotNull();
	}

	private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(moneyText);
		Assert.IsNotNull(cardTransform);
	}

	public void AddMoney(int amount)
	{
		money += amount;
		moneyText.text = "Coin: " + money;
	}

	public void AddCard(Card c)
	{
		if (playerCards.Count > 4)
		{
			Debug.Log("More than 5 cards!");
			return;
		}

		Card card = Instantiate(c, cardTransform);

		playerCards.Add(card);
		card.SetCurrIndex(playerCards.IndexOf(card));
		//card.transform.SetParent(cardTransform);
		card.SetCardPlayer(GameController.GetCurrPlayer());
	}

	public void RemoveCard(Card c)
	{
		playerCards.Remove(c);
		Destroy(c.gameObject);

		foreach (Card card in playerCards)
		{
			card.SetCurrIndex(playerCards.IndexOf(card));
		}
	}

	public void ResetPlayerManager()
	{
		ResetMoney();
		ResetCards();
	}

	public void ResetMoney()
	{
		money = 500;
		moneyText.text = "Coin: " + money;
	}

	public void ResetCards()
	{
		foreach (Card card in playerCards)
		{
			Destroy(card?.gameObject);
		}
		playerCards.Clear();
	}

	//public void SetCardTransform(Transform transform)
	//{
	//	cardTransform = transform;
	//}
}