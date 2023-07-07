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

	private void Update()
	{
		createRoomButton.interactable = PhotonNetwork.IsConnected && roomNameInputField.text.Length >= 3;
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		ClearRoomList();

		foreach (RoomInfo room in roomList)
		{
			if (room.IsOpen && room.PlayerCount < room.MaxPlayers)
			{
				GameObject roomListing = Instantiate(roomListingPrefab, roomListParent);
				RoomListingButton listingButton = roomListing.GetComponent<RoomListingButton>();
				listingButton.SetRoomInfo(room);
			}
		}
	}

	public void ClearRoomList()
	{
		foreach (Transform child in roomListParent)
		{
			Destroy(child.gameObject);
		}
	}

	public void CreateRoom()
	{
		if (roomNameInputField.text.Length >= 1)
		{
			PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions { MaxPlayers = 2 });
		}
	}
}
