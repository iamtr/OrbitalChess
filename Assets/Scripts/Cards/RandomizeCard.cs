public class RandomizeCard : Card
{
	public override void Trigger()
	{
		bc.RandomizeAllPieces();
		Destroy(this.gameObject);
	}
}