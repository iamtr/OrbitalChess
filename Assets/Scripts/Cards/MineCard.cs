public class MineCard : Card
{
	public override void Trigger()
	{
		if (gc.IsCheck) return;
		hm.HighlightPlantMinePositions();
	}
}