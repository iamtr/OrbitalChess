public class MineCard : Card
{
	public override void Trigger()
	{
		HighlightManager.i.HighlightPlantMinePositions();
		Destroy(this.gameObject);
	}
}
