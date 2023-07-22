using UnityEngine;

public class InputManager : MonoBehaviour
{
	protected BoardController bc;
	protected HighlightManager hm;
	protected GameController gc;

	protected virtual void Start()
	{
		bc = FindObjectOfType<BoardController>();
		hm = FindObjectOfType<HighlightManager>();
		gc = FindObjectOfType<GameController>();
	}


	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D collider = Physics2D.OverlapPoint(mousePosition);

			HandleColliderClicked(collider);
		}
	}


	/// <summary>
	/// Handles the mouse events (click)
	/// </summary>
	/// <param name="col"></param>
	public virtual void HandleColliderClicked(Collider2D col)
	{
		if (col == null)
		{
			bc.DisableAllUIElements();
		}
		else if (col.gameObject.CompareTag("Highlight Square"))
		{
			hm.HandleHighlightSquareClicked(col);
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
			if (gc.IsCheck) return;

			if (gc.GetCurrPlayerManager().Money < col.GetComponent<Piece>().Value)
			{
				Debug.Log("Not Enough Money!");
				return;
			}

			Piece piece = col.gameObject.GetComponent<Piece>();
			bc.SetPieceToInstantiate(piece);
			hm.HighlightSpawnPiece(piece);
		}
		else if (col.gameObject.CompareTag("Card")
			&& col.GetComponent<Card>().player == GameController.GetCurrPlayer()
			&& GameController.GetGameState() == GameState.Play)
		{
			bc.SyncCurrCard(col.GetComponent<Card>());
			col.GetComponent<Card>().Trigger();
		}
	}
}