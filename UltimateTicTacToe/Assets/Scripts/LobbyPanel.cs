using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
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

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby;
        LobbyManager.Instance.OnToggleReady += UpdateLobby;
        startBtn.interactable = false;
        startBtn.gameObject.SetActive(false);
        Hide();
    }

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
            startBtn.gameObject.SetActive(false);
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

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private bool ReadyCheck(string ready)
    {
        if (ready == "True")
        {
            return true;
        }
        return false;
    }
}
