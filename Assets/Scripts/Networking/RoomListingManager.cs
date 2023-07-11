using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomListingManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject roomListingPrefab;
	[SerializeField] private Transform roomListParent;
	[SerializeField] private TMP_InputField roomNameInputField;
	[SerializeField] private Button createRoomButton;

	private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

	private void Start()
	{
		DisplayLobbyRooms();
	}

	private void Update()
	{
		createRoomButton.interactable = PhotonNetwork.IsConnected && roomNameInputField.text.Length >= 1;
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		UpdateCachedRoomList(roomList);
		DisplayLobbyRooms();
	}

	public void DisplayLobbyRooms()
	{
		foreach (RoomInfo room in cachedRoomList.Values)
		{
			Debug.Log("Room: " + room.Name);
			GameObject roomListing = Instantiate(roomListingPrefab, roomListParent);
			RoomListingButton listingButton = roomListing.GetComponent<RoomListingButton>();
			listingButton.SetRoomInfo(room);
		}
	}

	private void UpdateCachedRoomList(List<RoomInfo> roomList)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			RoomInfo info = roomList[i];
			if (info.RemovedFromList || info.PlayerCount == 0)
			{
				Debug.Log("Removed: " + info.Name);
				cachedRoomList.Remove(info.Name);
			}
			else
			{
				Debug.Log("Added: " + info.Name);
				cachedRoomList[info.Name] = info;
			}
		}
	}

	public override void OnJoinedLobby()
	{
		cachedRoomList.Clear();
		Debug.Log("Joined lobby");
	}

	public override void OnLeftLobby()
	{
		cachedRoomList.Clear();
		Debug.Log("Left lobby");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		cachedRoomList.Clear();
	}

	public void CreateRoom()
	{
		if (roomNameInputField.text.Length >= 1)
		{
			PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions { MaxPlayers = 2 });
		}
	}
}
