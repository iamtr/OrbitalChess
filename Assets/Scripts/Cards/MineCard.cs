using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCard : Card
{
	public override void Trigger()
	{
		HighlightManager.i.HighlightPlantMinePositions();
		Destroy(this.gameObject);
	}
}
