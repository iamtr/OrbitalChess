public class BombCard : Card
{
	public override void Trigger()
	{
		if (GameController.i.IsCheck) return;

		hm.HighlightPawnBombs();
		Destroy(this.gameObject);
	}
}