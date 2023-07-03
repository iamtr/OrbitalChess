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
	[SerializeField] private PhotonView pv;

	private bool isGameStarted = false;
	private bool isBlackSelected = false;
	private bool isWhiteSelected = false;

	[SerializeField] private PlayerType localPlayer;
	[SerializeField] private GameObject playerSelectionPanel;
	[SerializeField] private Button blackButton;
	[SerializeField] private Button whiteButton;

	public PlayerType LocalPlayer { get => localPlayer; set => value = localPlayer; }

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

	public void OnPlayerColorSelected(int team)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
		// Broadcast the player's color choice to all other players
		pv.RPC(nameof(SyncPlayerColorChoice), RpcTarget.All, player.ActorNumber, team);
		playerSelectionPanel.SetActive(false);	
	}

	public void StartGame()
	{
		bc.InstantiatePieces();
		if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"].Equals(0))
		{
			bc.localPlayer = PlayerType.Black;
		}
		else if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"].Equals(1))
		{
			currPlayer = PlayerType.White;
		}
	}

	[PunRPC]
	private void SyncPlayerColorChoice(int playerActorNumber, int selectedTeam)
	{
		// Update the color choice for the corresponding player
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerActorNumber);
		player.CustomProperties["PlayerType"] = selectedTeam;

		if (selectedTeam == 0)
		{
			Debug.Log("Black selected");
			isBlackSelected = true;
			blackButton.interactable = false;
		} 
		else if (selectedTeam == 1)
		{
			Debug.Log("Black selected");
			isWhiteSelected = true;
			whiteButton.interactable = false;
		}

		// Check if both players have made their selections
		if (isBlackSelected && isWhiteSelected && !isGameStarted)
		{
			StartGame();
			isGameStarted = true;
		}
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
