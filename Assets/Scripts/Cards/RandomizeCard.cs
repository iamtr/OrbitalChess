public class RandomizeCard : Card
{
	public override void Trigger()
	{
		BoardController.i.RandomizeAllPieces();
		Destroy(this.gameObject);
	}
}