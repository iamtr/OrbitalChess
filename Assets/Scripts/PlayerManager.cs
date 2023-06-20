using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private int money;
	[SerializeField] private PlayerType player;
	[SerializeField] private TMP_Text moneyText;
	[SerializeField] private List<Card> playerCards;
	[SerializeField] private Transform cardTransform;

	public int Money { get => money; set => money = value; }
	public PlayerType Player { get => player; set => player = value; }

	private void Start()
	{
		moneyText.text = "Coin: " + money;
	}

	public void AddMoney(int amount)
	{
		money += amount;
		moneyText.text = "Coin: " + money;
	}

	public void AddCard(Card card)
	{
		if (playerCards.Count > 5) return;
		playerCards.Add(card);
		card.transform.parent = cardTransform;
		card.SetCardPlayer(GameController.GetCurrPlayer());
	}
}
