using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTurnCard : Card
{
    public override void Trigger()
    {
		if (gc.IsCheck) return;

		gc.SetDoubleTurn(true);
	}
}
