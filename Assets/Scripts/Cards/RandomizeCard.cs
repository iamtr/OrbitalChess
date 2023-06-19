using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeCard : Card
{
	public override void Trigger()
	{
		BoardController.i.RandomizeAllPieces();
		Destroy(this.gameObject);
	}
}
