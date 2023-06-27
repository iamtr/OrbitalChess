using UnityEngine;

public class InputManager : MonoBehaviour
{
	protected BoardController bc;
	protected HighlightManager hm; 

	private void Start()
	{
		bc = FindObjectOfType<BoardController>();
		hm = FindObjectOfType<HighlightManager>();
	}

	/// <summary>
	/// Handles the mouse events (click)
	/// </summary>
	/// <param name="col"></param>
	public void HandleColliderClicked(Collider2D col)
	{
		if (col == null)
		{
			bc.DisableAllUIElements();
		}
		else if (col.gameObject.CompareTag("Highlight Square"))
		{
			bc.HandleHighlightSquareClicked(col);
		}
		else if (col.gameObject.CompareTag("Piece")
			&& col.GetComponent<Piece>().Player == GameController.GetCurrPlayer()
			&& GameController.GetGameState() == GameState.Play)
		{
			bc.HandlePieceClicked(col);
		}
		else if (col.gameObject.CompareTag("Promotion Button") && GameController.GetGameState() == GameState.Promoting)
		{
			bc.HandlePromotionButtonClicked(col);
		}
		else if (col.gameObject.CompareTag("Buy Option"))
		{
			// Cannot buy pieces if is in check
			if (GameController.i.IsCheck) return;

			Piece piece = col.gameObject.GetComponent<Piece>();
			bc.SetPieceToInstantiate(piece);
			hm.HighlightSpawnPiece(piece);
		}
	}
}