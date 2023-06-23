using UnityEngine;

public class Replay : MonoBehaviour
{
	[SerializeField] private Piece[] defaultPieceSetup;
	public GameObject checkText;

	public void ResetGame()
	{
		GameController.SetGameState(GameState.Play);
		GameController.SetPlayer(PlayerType.White);
		checkText.SetActive(false);
		GameController.i.ResetPlayer();
		ResetPieces();
		Timer.ResetTimers();
	}

	public void ResetPieces()
	{
		for (var i = 0; i < 64; i++)
		{
			BoardController.i.DestroyPiece(i);
			if (defaultPieceSetup[i] != null) BoardController.i.InstantiatePiece(defaultPieceSetup[i], i);
		}
	}
}