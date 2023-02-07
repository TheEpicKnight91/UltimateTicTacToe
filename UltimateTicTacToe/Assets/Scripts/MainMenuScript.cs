using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject loginPanel;
    public TMP_InputField playerName;
    public GameObject errorTxt;
    public GameObject joinPanel;
    public TMP_InputField lobbyCode;

    public TMP_Text playerIdTxt;
    // Start is called before the first frame update
    public void Awake()
    {
        mainMenuPanel.SetActive(false);
        joinPanel.SetActive(false);
        errorTxt.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void LogIn()
    {
        LobbyManager.Instance.Login(playerName.text);
        if (LobbyManager.Instance.getPlayerName() != null)
        {
            errorTxt.SetActive(false);
            mainMenuPanel.SetActive(true);
            loginPanel.SetActive(false);
        }
        else
        {
            errorTxt.SetActive(true);
        }
    }
    public void Hosting()
    {
        LobbyManager.Instance.NewLobby();
        mainMenuPanel.SetActive(false);
    }

    public void JoinPanel()
    {
        joinPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void JoinLobby()
    {
        joinPanel.SetActive(false);
        LobbyManager.Instance.JoinLobbyWithCode(lobbyCode.text);
    }

    public void ToggleReady()
    {
        LobbyManager.Instance.UpdatePlayerReady();
    }
}
