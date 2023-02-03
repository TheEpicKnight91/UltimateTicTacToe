using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainGameScript : MonoBehaviour
{
    public Button newGameBtn;
    public Button exitGameBtn;
    public TextMeshProUGUI mainText;
    public TicTacToeScript ticTacToeScript;

    private bool win;
    private string message;
    private string playerTurn = "";
    private string player1 = "X";
    private string player2 = "O";
    // Start is called before the first frame update
    void Start()
    {
        newGameBtn.gameObject.SetActive(false);
        exitGameBtn.gameObject.SetActive(false);
        playerTurn = new string[] { player1, player2 }.GetValue(Random.Range(0, 2)).ToString();
        //ticTacToeScript.setPlayerTurn(playerTurn);
        message = "Player " + playerTurn + "'s Turn";
        mainText.text = message;
        win = false;
        if (NetworkManager.Singleton.IsHost)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setPlayerTurn(string player)
    {
        if(!win)
        {
            playerTurn = ticTacToeScript.getPlayerTurn();
            message = "Player " + playerTurn + "'s Turn";
            mainText.text = message;
        }
    }

    public void setPlayerWin(string player)
    {
        playerTurn = ticTacToeScript.getPlayerTurn();
        message = "Player " + playerTurn + " Wins";
        mainText.text = message;
        newGameBtn.gameObject.SetActive(true);
        exitGameBtn.gameObject.SetActive(true);
        win = true;
    }

    public void Reset()
    {
        newGameBtn.gameObject.SetActive(false);
        exitGameBtn.gameObject.SetActive(false);
        win = false;
        playerTurn = new string[] { player1, player2 }.GetValue(Random.Range(0, 2)).ToString();
        ticTacToeScript.setPlayerTurn(playerTurn);
        message = "Player " + playerTurn + "'s Turn";
        mainText.text = message;
    }
}
