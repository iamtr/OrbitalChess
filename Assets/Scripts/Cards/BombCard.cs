public class BombCard : Card
{
	public override void Trigger()
	{
		if (gc.IsCheck) return;

		hm.HighlightPawnBombs();
		Destroy(this.gameObject);
	}
}