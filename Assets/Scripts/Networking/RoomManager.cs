using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
	private PhotonView pv;
	[SerializeField] private PlayerManager playerManager;
	[SerializeField] private SpecialPlayerManager blackPlayer;
	[SerializeField] private SpecialPlayerManager whitePlayer;


	private BoardController bc;
	private GameController gc;

	[SerializeField] private GameObject playerSelectionPanel;
	[SerializeField] private TMP_Text turnText;
	[SerializeField] private TMP_Text checkText;
	[SerializeField] private TMP_Text piecePriceText;
	[SerializeField] private Button blackButton;
	[SerializeField] private Button whiteButton;
	[SerializeField] private bool isGameStarted;

	[SerializeField] private TMP_Text opponentPlayerCoinText;
	[SerializeField] private TMP_Text localPlayerCoinText;
	[SerializeField] private Button buyButton;

	private void Awake()
	{
		bc = FindObjectOfType<MultiplayerBoardController>();
		gc = FindObjectOfType<MultiplayerGameController>();
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
			whiteButton.interactable = false;
		}
		else if (selectedTeam == 1)
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "White", true } });
			blackButton.interactable = false;
		}
	}

	public void SetPlayerSelection()
	{
		blackButton.interactable = !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Black");
		whiteButton.interactable = !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("White");
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (otherPlayer.CustomProperties.TryGetValue("PlayerType", out object value))
		{
			int? team = (int)value;
			if (team == null) return;
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

		if (isGameStarted)
		{
			GameController.SetGameState(GameState.GameOver);
			checkText.gameObject.SetActive(true);
			checkText.text = "Opponent has left room;";
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
			&& (bool)PhotonNetwork.CurrentRoom.CustomProperties["Black"] && (bool)PhotonNetwork.CurrentRoom.CustomProperties["White"] 
			&& !isGameStarted)
		{
			if ((int) PhotonNetwork.CurrentRoom.CustomProperties["Mode"] == 1)
			{
				StartCardMultiplayer();
			}
			else
			{
				StartMultiplayerGame();
			}
				
		}
	}

	public void StartCardMultiplayer()
	{
		Debug.Log("Start card nultiplayer");
		PhotonNetwork.CurrentRoom.IsOpen = false;
		int playerType = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"];

		playerManager.Player = playerType == 0 ? PlayerType.Black : PlayerType.White;

		if (playerType == 1)
		{
			Camera c = FindObjectOfType<Camera>();
			c.transform.eulerAngles = new Vector3(0, 0, 180);
		}

		if (PhotonNetwork.LocalPlayer.IsMasterClient) bc.InstantiatePieces();
		playerSelectionPanel.SetActive(false);
		turnText.gameObject.SetActive(true);

		if (playerManager.Player == PlayerType.Black)
		{
			blackPlayer.MoneyText = localPlayerCoinText;
			whitePlayer.MoneyText = opponentPlayerCoinText;	

		} 
		else if (playerManager.Player == PlayerType.White)
		{
			blackPlayer.MoneyText = opponentPlayerCoinText;
			whitePlayer.MoneyText = localPlayerCoinText;
		}

		playerSelectionPanel.SetActive(false);
		opponentPlayerCoinText.gameObject.SetActive(true);
		localPlayerCoinText.gameObject.SetActive(true);
		buyButton.gameObject.SetActive(true);
		piecePriceText.gameObject.SetActive(true);
		isGameStarted = true;
	}

	public void StartMultiplayerGame()
	{
		Debug.Log("Start game");
		PhotonNetwork.CurrentRoom.IsOpen = false;
		int playerType = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"];
		
		playerManager.Player = playerType == 0 ? PlayerType.Black : PlayerType.White; 

		if (playerType == 1)
		{
			Camera c = FindObjectOfType<Camera>();
			c.transform.eulerAngles = new Vector3(0, 0, 180);
		}

		if (PhotonNetwork.LocalPlayer.IsMasterClient) bc.InstantiatePieces();
		playerSelectionPanel.SetActive(false);
		turnText.gameObject.SetActive(true);
		isGameStarted = true;
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
