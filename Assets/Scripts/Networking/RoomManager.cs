using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
	private PhotonView pv;
	private PlayerManager playerManager;

	[SerializeField] private BoardController bc;
	[SerializeField] private GameObject playerSelectionPanel;
	[SerializeField] private TMP_Text turnText;
	[SerializeField] private Button blackButton;
	[SerializeField] private Button whiteButton;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		playerManager = FindObjectOfType<PlayerManager>();
	}

	private void Start()
	{
		SetPlayerSelection();
	}

	public void OnPlayerColorSelected(int team)
	{
		SyncPlayerColorChoice(team);
	}

	public void SyncPlayerColorChoice(int selectedTeam)
	{
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerType", selectedTeam } });
		if (selectedTeam == 0)
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Black", true } });
		}
		else if (selectedTeam == 1)
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "White", true } });
		}
	}

	public void SetPlayerSelection()
	{
		blackButton.interactable = !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Black");
		whiteButton.interactable = !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("White");
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		int team = (int)otherPlayer.CustomProperties["PlayerType"];
		if (team == 0)
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Black", false } });
			blackButton.interactable = true;
		}
		else if (team == 1)
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "White", false } });
			whiteButton.interactable = true;
		}
	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Black", out object blackValue) && blackValue != null)
		{
			blackButton.interactable = !(bool)blackValue;
		}
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("White", out object whiteValue) && whiteValue != null)
		{
			whiteButton.interactable = !(bool)whiteValue;
		}

		if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("White") && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Black")
			&& (bool)PhotonNetwork.CurrentRoom.CustomProperties["Black"] && (bool)PhotonNetwork.CurrentRoom.CustomProperties["White"])
		{
			pv.RPC(nameof(RPC_StartGame), RpcTarget.All);
		}
	}

	[PunRPC]
	public void RPC_StartGame()
	{
		int playerType = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"];
		playerManager.Player = playerType == 0 ? PlayerType.Black : PlayerType.White;

		if (playerType == 1)
		{
			Camera c = FindObjectOfType<Camera>();
			c.transform.eulerAngles = new Vector3(0, 0, 180);
		}

		bc.InstantiatePieces();
		playerSelectionPanel.SetActive(false);
		turnText.gameObject.SetActive(true);
	}

	[PunRPC]
	public void RPC_TogglePlayerOptions(int team, bool activate)
	{
		if (team == 0)
		{
			blackButton.interactable = activate;
		}
		else if (team == 1)
		{
			whiteButton.interactable = activate;
		}
	}
}
