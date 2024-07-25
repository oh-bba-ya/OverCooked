using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class OverCookGameLobby : MonoBehaviour
{
    public static OverCookGameLobby Instance { get; private set; }


    private Lobby joinedLobby;
    private float heartbeatTimer;


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        // ����Ƽ ���� ���� �ʱ�ȭ..
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        // ����Ƽ ���� ����..
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();

            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync();

            // �͸� ����
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }

    }

    private void Update()
    {
        HandleHeartbeat();
    }

    /// <summary>
    /// ����Ƽ �κ�ý����� ��� �κ� ���� �� �ֱ������� ��ȣ�� ������ ������ �κ� ���� ó����..
    /// �ִ� 30�� �̳��� ��ȣ�� �ֱ������� ��������
    /// </summary>
    private void HandleHeartbeat()
    {
        if(IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                // SendHeartbeatPingAsync()�� ���� ������ �κ� ����..
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName,
                    KitchenGameMultiplayer.MAX_PLAYER_AMOUNT,
                    new CreateLobbyOptions { IsPrivate = isPrivate }
            );

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void QuickJoin()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }



}
