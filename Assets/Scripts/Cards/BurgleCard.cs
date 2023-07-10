using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgleCard : Card
{
	public override void Trigger()
	{
		if (gc.IsCheck) return;

		bc.RandomlySteal();
	}
}
