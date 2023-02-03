#if UNITY_EDITOR
    using ParrelSync;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_READY = "PlayerReady";

    private string playerName;
    private bool ready;
    private Lobby curLobby;
    private float heartbeatTimer;
    private float lobbyPollTimer;
    private int maxPlayers = 2;
    private UnityTransport transport;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnToggleReady;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public Lobby getCurLobby() 
    {
        return curLobby; 
    }

    public string getPlayerName()
    { 
        return playerName; 
    }

    private void Awake()
    {
        Instance = this;
        playerName = null;
        ready = false;
        transport = FindObjectOfType<UnityTransport>();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public bool IsLobbyHost()
    {
        return curLobby != null && curLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    //Logins Player in by creating new player object
    public async void Login(string name)
    {
        if (name != null && name != "")
        {
            this.playerName = name;
            InitializationOptions initializationOptions = new InitializationOptions();
#if UNITY_EDITOR
            initializationOptions.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif            
            initializationOptions.SetProfile(name);
            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in with Player Id: " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    //refreshes lobby to stay alive
    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(curLobby.Id);
            }
        }
    }
    //Gets Player data
    private Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { KEY_PLAYER_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "False") }
        });
    }

    private void  SetTransformForClient(JoinAllocation a)
    {
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }
    //Updates the lobby every second
    private async void HandleLobbyPolling()
    {
        if (curLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                curLobby = await LobbyService.Instance.GetLobbyAsync(curLobby.Id);

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = curLobby });
            }
        }
    }

    public async void NewLobby()
    {
        try
        {
            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joincode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            Player player = GetPlayer();
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { "j", new DataObject(DataObject.VisibilityOptions.Public, joincode)} },
                IsPrivate = true,
                Player = player,
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(playerName + "'s Lobby", maxPlayers, createLobbyOptions);
            transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
            curLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyWithCode(string code)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(),
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code.ToUpper(), joinLobbyByCodeOptions);
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data["j"].Value);
            SetTransformForClient(a);
            curLobby = lobby;

            OnJoinedLobby?.Invoke(this,new LobbyEventArgs { lobby = lobby });
            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerReady()
    {
        if (curLobby != null)
        {
            try
            {
                ready = !ready;
                UpdatePlayerOptions player = new UpdatePlayerOptions();
                player.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        KEY_PLAYER_READY, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: ready.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(curLobby.Id, playerId, player);
                curLobby = lobby;

                OnToggleReady.Invoke(this, new LobbyEventArgs { lobby = curLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public void StartGame()
    {
        Debug.Log(NetworkManager.Singleton.ConnectedClients);
        NetworkManager.Singleton.SceneManager.LoadScene("TicTacToeScene", LoadSceneMode.Single);
    }
}
