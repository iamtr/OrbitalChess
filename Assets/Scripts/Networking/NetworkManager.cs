using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private MultiplayerBoardController bc;

	private void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
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

	//public override void OnJoinRandomFailed(short returnCode, string message)
	//{
	//	Debug.Log($"Joining random room failed becuse of {message}. Creating new one");
	//	PhotonNetwork.CreateRoom(null, new RoomOptions
	//	{
	//		MaxPlayers = 2
	//	});
	//}

	public override void OnJoinedRoom()
	{
		Debug.Log("Player joined room");
		PhotonNetwork.LoadLevel("Multiplayer Main");
		bc = FindObjectOfType<MultiplayerBoardController>();
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		{
			Debug.Log($"Player is host");
		}
	}

	//internal bool IsRoomFull()
	//{
	//	return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
	//}

}
