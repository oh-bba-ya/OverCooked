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

        // 유니티 서비스 인증 초기화..
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        // 유니티 서비스 인증..
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();

            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync();

            // 익명 설정
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }

    }

    private void Update()
    {
        HandleHeartbeat();
    }

    /// <summary>
    /// 유니티 로비시스템의 경우 로비 생성 후 주기적으로 신호를 보내지 않으면 로비가 삭제 처리됌..
    /// 최대 30초 이내로 신호를 주기적으로 보내야함
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

                // SendHeartbeatPingAsync()를 통해 생성한 로비 유지..
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
