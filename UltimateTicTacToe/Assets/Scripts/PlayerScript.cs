using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    private string playerSymbol;
    // Start is called before the first frame update
    void Start()
    {
        playerSymbol = "";
        if (this.GetComponent<NetworkObject>().IsOwner)
        {
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
    // Update is called once per frame
    void Update()
    {
        
    }

    public void disablePlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void enablePlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
