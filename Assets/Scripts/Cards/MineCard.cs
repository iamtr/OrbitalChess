public class MineCard : Card
{
	public override void Trigger()
	{
		hm.HighlightPlantMinePositions();
		Destroy(this.gameObject);
	}
}