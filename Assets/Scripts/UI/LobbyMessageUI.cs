using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoingGame += KitchenGameMultiplayer_OnFailedToJoingGame;
        OverCookGameLobby.Instance.OnCreateLobbyStarted += OverCookGameLobby_OnCreateLobbyStarted;
        OverCookGameLobby.Instance.OnCreateLobbyFailed += OverCookGameLobby_OnCreateLobbyFailed;
        OverCookGameLobby.Instance.OnJoinStarted += OverCookGameLobby_OnJoinStarted;
        OverCookGameLobby.Instance.OnJoinFailed += OverCookGameLobby_OnJoinFailed;
        OverCookGameLobby.Instance.OnQuickJoinFailed += OverCookGameLobby_OnQuickJoinFailed;


        Hide();
    }

    private void OverCookGameLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join Lobby..");
    }

    private void OverCookGameLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Could not find a Lobby to Quick Join");
    }

    private void OverCookGameLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining Lobby..");
    }

    private void OverCookGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to create Lobby");
    }

    private void OverCookGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby..");
    }

    private void KitchenGameMultiplayer_OnFailedToJoingGame(object sender, EventArgs e)
    {
        if(NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoingGame -= KitchenGameMultiplayer_OnFailedToJoingGame;
    }
}
