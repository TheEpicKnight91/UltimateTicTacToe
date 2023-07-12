using UnityEngine;
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
    //ui for login screen
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
    //changes ui for when hosting a new lobby
    public void Hosting()
    {
        LobbyManager.Instance.NewLobby();
        mainMenuPanel.SetActive(false);
    }
    //shows the list of lobby screen
    public void JoinPanel()
    {
        if (playerName.text != null && playerName.text != "")
        {
            errorTxt.SetActive(false);
            joinPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
        else
        {
            errorTxt.SetActive(true);
        }
    }
    //allows player to join a certain lobby by code
    public void JoinLobby()
    {
        joinPanel.SetActive(false);
        LobbyManager.Instance.JoinLobbyWithCode(lobbyCode.text);
    }
    //toggles the player ready state
    public void ToggleReady()
    {
        LobbyManager.Instance.UpdatePlayerReady();
    }
}
