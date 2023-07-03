using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerGameController : GameController
{
	[SerializeField] private PhotonView pv;

	public override void Start()
	{
		base.Start();
		pv = GetComponent<PhotonView>();
	}

	public override void SetPlayer()
	{
		pv.RPC(nameof(RPC_SetPlayer), RpcTarget.All);
	}

	public override void HandleCheckAndCheckmate()
	{
		pv.RPC(nameof(RPC_HandleCheckAndCheckmate), RpcTarget.All);
	}

	[PunRPC]
	public void RPC_SetPlayer()
	{
		base.SetPlayer();
	}

	[PunRPC]
	public void RPC_HandleCheckAndCheckmate()
	{
		base.HandleCheckAndCheckmate();
	}
}
