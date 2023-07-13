using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnWallCard : Card
{
	public override void Trigger()
	{
		if (gc.IsCheck) return;

		hm.HighlightSacrificialPieces();
	}
}
