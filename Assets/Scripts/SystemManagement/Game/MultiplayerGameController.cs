using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerGameController : GameController
{
	[PunRPC]
	public override void HandleCheckAndCheckmate()
	{
		base.HandleCheckAndCheckmate();
	}

	[PunRPC]
	public override void SetPlayer()
	{
		base.SetPlayer();
	}
}
