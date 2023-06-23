public class BombCard : Card
{
	public override void Trigger()
	{
		if (GameController.i.IsCheck) return;

		HighlightManager.i.HighlightPawnBombs();
		Destroy(this.gameObject);
	}
}