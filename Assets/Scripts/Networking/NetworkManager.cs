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

	public void CreateRoom(string roomName)
	{
		PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
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
		PhotonNetwork.LoadLevel("Multiplayer Main");
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		{
			Debug.Log($"Player is host");
		}
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		Debug.Log("Player left room");
	}
}
