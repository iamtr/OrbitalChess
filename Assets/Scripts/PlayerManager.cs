using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private int money;
	[SerializeField] private TMP_Text moneyText;
	[SerializeField] private PlayerType player;

	public int Money { get => money; set => money = value; }
	public PlayerType Player { get => player; set => player = value; }
}
