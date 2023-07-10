using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealCard : Card
{
	public override void Trigger()
	{
		if (gc.IsCheck) return;

		bc.RandomlySteal();
	}
}
