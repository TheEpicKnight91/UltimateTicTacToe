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

    // Start is called before the first frame update
    void Start()
    {
        newGameBtn.gameObject.SetActive(false);
        exitGameBtn.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*public void Reset()
    {
        newGameBtn.gameObject.SetActive(false);
        exitGameBtn.gameObject.SetActive(false);
        win = false;
        playerTurn = new string[] { player1, player2 }.GetValue(Random.Range(0, 2)).ToString();
        message = "Player " + playerTurn + "'s Turn";
        mainText.text = message;
    }*/
}
