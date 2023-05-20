using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BoardController boardController;
    [SerializeField] private PlayerType currPlayer;

    public PlayerType CurrPlayer => currPlayer;

	private void Start()
	{
        boardController = GameObject.Find("Board").GetComponent<BoardController>();
    }
	private void Update()
	{
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            if (collider != null)
            {
                boardController.HandleColliderClicked(collider);
            }
        }
    }

    public void RoundEnd()
	{
        if (currPlayer == PlayerType.Black)
		{
            currPlayer = PlayerType.White;
		} 
        else if (currPlayer == PlayerType.White)
		{
            currPlayer = PlayerType.Black;
		}
	}

    
}

public enum PlayerType { Black, White }
