using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager i;

	private void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
		DontDestroyOnLoad(this);

		if (i != null && i != this) Destroy(this);
		else i = this;
	}

	public void Connect()
	{
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log($"Connected to server.");
		PhotonNetwork.JoinLobby();
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("Created room");
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("Unable to create room");
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("Player joined room");

		switch (PhotonNetwork.CurrentRoom.CustomProperties["Mode"])
		{
			case 0:
				PhotonNetwork.LoadLevel("Multiplayer Main");
				break;
			case 1:
				PhotonNetwork.LoadLevel("Multiplayer Custom Game Mode");
				break;
			default:
				Debug.Log("Invalid mode");
				break;
		}

		if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
		{
			Debug.Log("2 players are inside");
			//PhotonNetwork.CurrentRoom.IsOpen = false;
		}
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		Debug.Log("Player left room");
		PhotonNetwork.JoinLobby();
	}

	public override void OnLeftLobby()
	{
		Debug.Log("Leave Lobby");
	}
}
