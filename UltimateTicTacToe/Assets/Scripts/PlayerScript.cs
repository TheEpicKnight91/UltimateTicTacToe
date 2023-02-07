using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{

    private string playerSymbol;
    private string playerName;
    private ulong clientId;

    [HideInInspector]
    public TicTacToeScript ticTacToeScript;
    // Start is called before the first frame update
    void Start()
    {
        playerName = null;
        clientId = 0;
        playerSymbol = "";
        if (this.GetComponent<NetworkObject>().IsOwner)
        {
            clientId = GetComponent<NetworkObject>().OwnerClientId;
            playerSymbol = "X";
            this.gameObject.tag = "xPlayer";
        }
        else
        {
            playerSymbol = "O";
            this.gameObject.tag = "oPlayer";
        }
    }

    public string getPlayerSymbol()
    {
        return playerSymbol;
    }

    public string getPlayerName()
    {
        return playerName; 
    }

    public void setPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

    public ulong getClientId()
    {
        return clientId;
    }

    public void setClientId(ulong clientId)
    {
        this.clientId = clientId;
    }
    public void setTicTacToeScript(TicTacToeScript ticTacToeScript)
    {
        this.ticTacToeScript = ticTacToeScript;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    [ClientRpc]
    public void disablePlayerClientRpc(ClientRpcParams clientRpc)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    [ClientRpc]
    public void enablePlayerClientRpc(ClientRpcParams client)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
