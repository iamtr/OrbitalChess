using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class MultiplayerGameController : GameController
{
	protected PhotonView pv;

	private bool isGameStarted = false;
	private bool isBlackSelected = false;
	private bool isWhiteSelected = false;

	[Header("Multiplayer")]
	[SerializeField] private GameObject playerSelectionPanel;
	[SerializeField] private Button blackButton;
	[SerializeField] private Button whiteButton;
	[SerializeField] private Transform blackCardTransform;
	[SerializeField] private Transform whiteCardTransform;
	[SerializeField] private SpecialPlayerManager localPlayer;
	[SerializeField] private SpecialPlayerManager remotePlayer;

	[SerializeField] protected PlayerManager playerManager;

	protected override void Start()
	{
		base.Start();
		pv = GetComponent<PhotonView>();
		//playerManager = FindObjectOfType<PlayerManager>();
	}

	public override void SetPlayer()
	{
		pv.RPC(nameof(RPC_SetPlayer), RpcTarget.All);
	}

	public override void HandleCheckAndCheckmate()
	{
		pv.RPC(nameof(RPC_HandleCheckAndCheckmate), RpcTarget.All);
	}

	public void StartGame()
	{
		if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"].Equals(0))
		{
			playerManager.Player = PlayerType.Black;
		}
		else if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"].Equals(1))
		{
			playerManager.Player = PlayerType.White;
			Camera c = FindObjectOfType<Camera>();
			c.transform.eulerAngles = new Vector3(0, 0, 180);
		}

		bc.InstantiatePieces();
		turnText.gameObject.SetActive(true);
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
