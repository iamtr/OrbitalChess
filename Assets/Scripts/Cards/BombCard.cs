using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BombCard : Card
{
	public override void Trigger()
	{
		if (GameController.i.IsCheck) return;

		HighlightManager.i.HighlightPawnBombs();
		Destroy(this.gameObject);
	}
}
