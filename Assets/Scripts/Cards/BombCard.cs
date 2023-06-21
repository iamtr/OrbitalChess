using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCard : Card
{
	public override void Trigger()
	{
		HighlightManager.i.HighlightPawnBombs();
		Destroy(this.gameObject);
	}
}
