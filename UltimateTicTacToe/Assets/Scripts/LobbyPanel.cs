using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    public static LobbyPanel Instance { get; private set; }
    public TMP_Text player1;
    public TMP_Text player2;
    public TMP_Text lobbyCodeTxt;
    public Button startBtn;
    public List<Toggle> ready = new List<Toggle>();
    public List<Button> readyBtn = new List<Button>();
    // Start is called before the first frame update

    //creates and instance of lobby panel
    private void Awake()
    {
        Instance = this;
    }
    //sets up the lobby manager events and the first time lobby hosting screen
    private void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby;
        LobbyManager.Instance.OnToggleReady += UpdateLobby;
        LobbyManager.Instance.OnStartGame += StartGame;
        startBtn.interactable = false;
        startBtn.gameObject.SetActive(false);
        Hide();
    }
    //updates the lobby gui with player states and what players are in the lobby
    private void UpdateLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = LobbyManager.Instance.getCurLobby();

        if (lobby.Players.Count >= 1)
        {
            player1.text = "Player 1 : " + lobby.Players[0].Data["PlayerName"].Value;
            ready[0].isOn = ReadyCheck(lobby.Players[0].Data["PlayerReady"].Value);
        }
        else
        {
            player1.text = "Player 1 : None";
            ready[0].isOn = false;
        }
        if (lobby.Players.Count >= 2)
        {
            player2.text = "Player 2 : " + lobby.Players[1].Data["PlayerName"].Value;
            ready[1].isOn = ReadyCheck(lobby.Players[1].Data["PlayerReady"].Value);
        }
        else
        {
            player2.text = "Player 2 : None";
            ready[0].isOn = false;
        }
        if (LobbyManager.Instance.IsLobbyHost())
        {
            startBtn.gameObject.SetActive(true);
            readyBtn[1].gameObject.SetActive(false);
        }
        else
        {
            readyBtn[0].gameObject.SetActive(false);
        }
        lobbyCodeTxt.text = "Lobby Code: " + lobby.LobbyCode;
        bool startOn = true;
        foreach (Toggle toggle in ready)
        {
            if (!toggle.isOn)
            {
                startOn = false;
                break;
            }
        }
        startBtn.interactable = startOn;
        Show();
    }
    //sets the player objects on the server with correct names, and client ids
    private void StartGame(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = LobbyManager.Instance.getCurLobby();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        players[0].GetComponent<PlayerScript>().setPlayerName(lobby.Players[0].Data[LobbyManager.KEY_PLAYER_NAME].Value);
        players[0].GetComponent<PlayerScript>().setPlayerClientID(NetworkManager.Singleton.ConnectedClientsIds[0]);
        players[1].GetComponent<PlayerScript>().setPlayerName(lobby.Players[1].Data[LobbyManager.KEY_PLAYER_NAME].Value);
        players[1].GetComponent<PlayerScript>().setPlayerClientID(NetworkManager.Singleton.ConnectedClientsIds[1]);
    }
    //showsstart button once all players are ready
    private void Show()
    {
        gameObject.SetActive(true);
    }
    //hides the start button when at least one player isnt ready
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    //check to see if player is ready
    private bool ReadyCheck(string ready)
    {
        if (ready == "True")
        {
            return true;
        }
        return false;
    }
}
