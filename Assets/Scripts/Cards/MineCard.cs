using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCard : Card
{
	public override void Trigger()
	{
		BoardController.i.HighlightPlantMinePositions ();
		Destroy(this.gameObject);
	}
}
