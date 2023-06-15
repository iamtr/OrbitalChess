using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite[] blackSprites;
    [SerializeField] private Sprite[] whiteSprites;
    [SerializeField] private PromotionButton[] promotingBlack;
    [SerializeField] private PromotionButton[] promotingWhite;
    [SerializeField] private PromotionButton promotionButton;

    private Transform promotionButtonTransform;
    private readonly int promotingNumber = 4;

    public static UIManager i { get; set; }

	public void Awake()
	{
		i = this;
	}
	private void Start()
    {
        promotionButtonTransform = GameObject.Find("Promotion Buttons").transform;
        //promotingBlack = new PromotionButton[promotingNumber];
        //promotingWhite = new PromotionButton[promotingNumber];

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
}
