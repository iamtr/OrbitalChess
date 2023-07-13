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
	[SerializeField] private TMP_Dropdown modeSelectDropdown;

	// Used to cache room list for lobby list updates
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
		Debug.Log("RoomListUpdate");
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
		ClearCachedList();
		for (int i = 0; i < roomList.Count; i++)
		{
			RoomInfo info = roomList[i];
			if (info.RemovedFromList || info.PlayerCount == 0 || !info.IsOpen)
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

	public void ClearCachedList()
	{
		cachedRoomList.Clear();
		foreach (Transform t in roomListParent)
		{
			Destroy(t.gameObject);
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
			int mode = modeSelectDropdown.value;
			string roomName = modeSelectDropdown.options[mode].text + " " + roomNameInputField.text;

			PhotonNetwork.CreateRoom(roomName, 
				new RoomOptions 
				{ 
					MaxPlayers = 2, 
					CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "Mode", mode } }
				});
		}
	}
}
