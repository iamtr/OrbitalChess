using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingButton : MonoBehaviour
{
	[SerializeField] private TMP_Text roomNameText;
	[SerializeField] private RoomInfo roomInfo;

	public void SetRoomInfo(RoomInfo info)
	{
		roomInfo = info;
		roomNameText.text = info.Name;
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom(roomInfo.Name);
	}
}
