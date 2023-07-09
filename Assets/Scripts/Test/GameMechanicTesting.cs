using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameMechanicTesting : MonoBehaviour
{
	private BoardController bc;
	private GameController gc;

	private void Start()
	{
		bc = FindObjectOfType<BoardController>();
		gc = FindObjectOfType<GameController>();	
	}

	public void Try_Checkmate_GameStateEqualsGameOver()
	{
		bc.ResetPieces();
		bc.MovePiece(0, 3, bc.GetPieceFromPos(bc.ConvPos(2, 7)));
		bc.MovePiece(2, 1, bc.GetPieceFromPos(bc.ConvPos(4, 7)));

		gc.HandleCheckAndCheckmate();

		bool test1 = GameController.GetGameState() == GameState.GameOver;
		Assert.IsTrue(test1);
		Debug.Log("Checkmate; Game state equals game over: " + test1);

		bool test2 = gc.GetCheckText().text == "Checkmate!" && gc.GetTurnText().gameObject.activeInHierarchy;
		Assert.IsTrue(test2);
		Debug.Log("Checkmate; Checkmate text shown: " + test2);



		bc.DisableAllUIElements();
	}

	public void TestAll()
	{

	}
}
