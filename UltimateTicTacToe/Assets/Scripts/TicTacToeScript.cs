using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class TicTacToeScript : NetworkBehaviour
{
    [System.Serializable]
    public class Squares
    {
        public Button buttonObject;
        public string buttonSymbol;
    }
    [System.Serializable]
    public class Rows
    {
        public List<Squares> row = new List<Squares>();
    }

    public Sprite xImage;
    public Sprite oImage;
    public MainGameScript mainGameScript;
    private PlayerScript xPlayer = null;
    private PlayerScript oPlayer = null;

    private PlayerScript currentPlayerTurn;
    public List<Rows> listOfSqaures = new List<Rows>();
    // Start is called before the first frame update

    //creates a client rpc param to specify which client to send a rpc to
    public ClientRpcParams createClientRpcParms(ulong clientID)
    {
        return new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
    }

    void Start()
    {
        if (IsServer)
        {
            //sets the player to certian smbols for tic tac toe
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                PlayerScript ps = player.GetComponent<PlayerScript>();
                if (ps.getPlayerClientID().Equals(0))
                {
                    ps.setPlayerSymbol("X");
                    xPlayer = ps;
                }
                else
                {
                    ps.setPlayerSymbol("O");
                    oPlayer = ps;
                }
            }
            //sets up the player text to display palyers names and sybmols
            string p1Txt = "Player 1 : " + xPlayer.getPlayerName() + "\nSymbol : " + xPlayer.getPlayerSymbol();
            string p2Txt = "Player 2 : " + oPlayer.getPlayerName() + "\nSymbol : " + oPlayer.getPlayerSymbol();
            setUpText(p1Txt, p2Txt);
            currentPlayerTurn = (PlayerScript)new PlayerScript[] { xPlayer, oPlayer }.GetValue(Random.Range(0, 2));
            setCurrentPlayerTurn(currentPlayerTurn);
            //sends an rpc to specific client to do the same as the above
            firstTimeSetupClientRpc(p1Txt, p2Txt, createClientRpcParms(1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Sends data to client for first time setup
    [ClientRpc]
    public void firstTimeSetupClientRpc(string p1Text, string p2Text, ClientRpcParams clientRpcParams = default)
    {
        setUpText(p1Text, p2Text);
    }

    public void setUpText(string p1Text, string p2Text)
    {
        GameObject.Find("Player1XText").GetComponent<TextMeshProUGUI>().text = p1Text;
        GameObject.Find("Player2OText").GetComponent<TextMeshProUGUI>().text = p2Text;
    }

    [ClientRpc]
    public void setCurrentPlayerTurnClientRpc(string ptText, ClientRpcParams clientRpcParams = default)
    {
        setMainText(ptText);
    }

    public void setMainText(string cpt)
    {
        GameObject.Find("MainText").GetComponent<TextMeshProUGUI>().text = cpt;
    }

    public void setCurrentPlayerTurn(PlayerScript pt)
    {
        currentPlayerTurn = pt;
        string currentPTurn = currentPlayerTurn.getPlayerName() + " Turn";
        setMainText(currentPTurn);
        setCurrentPlayerTurnClientRpc(currentPTurn, createClientRpcParms(1));
        if (currentPlayerTurn.getPlayerClientID().Equals(0))
        {
            disableButtonsClientRpc(createClientRpcParms(1));
            enableButtons();
        }
        else
        {
            disableButtons();
            enableButtonsClientRpc(createClientRpcParms(1));
        }
    }

    [ClientRpc]
    public void enableButtonsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        enableButtons();
    }
    public void enableButtons()
    {
        foreach (Rows row in listOfSqaures)
        {
            foreach (Squares square in row.row)
            {
                if (square.buttonSymbol.Equals(""))
                {
                    square.buttonObject.interactable = true;
                }
            }
        }
    }

    [ClientRpc]
    public void disableButtonsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        disableButtons();
    }
    public void disableButtons()
    {
        foreach(Rows row in listOfSqaures)
        {
            foreach(Squares square in row.row)
            {
                square.buttonObject.interactable = false;
            }
        }
    }
    public void ChoseSquare(Button button)
    {
        int rowIndex = 0;
        int squareIndex = 0;
        foreach(Rows row in listOfSqaures)
        {
            foreach(Squares square in row.row)
            {
                if (square.buttonObject.Equals(button))
                {
                    rowIndex = listOfSqaures.IndexOf(row);
                    squareIndex = row.row.IndexOf(square);
                    break;
                }
            }
        }
        if (IsServer)
        {
            setSqaureToSymbol(rowIndex, squareIndex);
        }
        else
        {
            setSquareServerRpc(rowIndex, squareIndex);
        }
    }

    //client to server function to set square
    [ServerRpc(RequireOwnership = false)]
    public void setSquareServerRpc(int row, int square)
    {
        setSqaureToSymbol(row, square);
    }

    //server to client function to set square
    [ClientRpc]
    public void setSquareToSymbolClientRpc(int row, int square, string symbol, ClientRpcParams clientRpcParams = default)
    {
        Sprite img;
        if (symbol.Equals("X"))
        {
            img = xImage;
        }
        else
        {
            img = oImage;
        }
        setSquare(row, square, symbol, img);
    }
    //server function
    public void setSqaureToSymbol(int row, int square)
    {
        Sprite img;
        PlayerScript newPlayer;
        if (currentPlayerTurn.Equals(xPlayer))
        {
            img = xImage;
            newPlayer = oPlayer;
        }
        else
        {
            img = oImage;
            newPlayer = xPlayer;
        }
        setSquare(row, square, currentPlayerTurn.getPlayerSymbol(), img);
        setSquareToSymbolClientRpc(row, square, currentPlayerTurn.getPlayerSymbol(), createClientRpcParms(1));
        if (VerticalCheck() || HorizontalCheck() || DiagonalCheck() || ReverseDiagonalCheck())
        {
            string win = "Winner: " + currentPlayerTurn.getPlayerName();
            Debug.Log(win);
            setMainText(win);
            setCurrentPlayerTurnClientRpc(win, createClientRpcParms(1));
            disableButtons();
            disableButtonsClientRpc();
        }
        else
        {
            setCurrentPlayerTurn(newPlayer);
        }
    }

    public void setSquare(int row, int square, string symbol, Sprite img)
    {
        Squares selectedSquare = listOfSqaures[row].row[square];
        selectedSquare.buttonObject.GetComponent<Image>().sprite = img;
        selectedSquare.buttonObject.interactable = false;
        selectedSquare.buttonSymbol = symbol;
    }
    public bool VerticalCheck()
    {
        foreach(Rows row in listOfSqaures)
        {
            string beginningMark = row.row[0].buttonSymbol;
            if (beginningMark != "")
            {
                if (row.row[1].buttonSymbol.Equals(beginningMark) && row.row[2].buttonSymbol.Equals(beginningMark))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HorizontalCheck()
    {
        for (int i = 0; i < listOfSqaures.Count; i++)
        {
            string beginningMark = listOfSqaures[0].row[i].buttonSymbol;
            if (beginningMark != "")
            {
                if (listOfSqaures[1].row[i].buttonSymbol.Equals(beginningMark) && listOfSqaures[2].row[i].buttonSymbol.Equals(beginningMark))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool DiagonalCheck()
    {
        string beginningMark = listOfSqaures[0].row[0].buttonSymbol;
        if (beginningMark != "")
        {
            if (listOfSqaures[1].row[1].buttonSymbol.Equals(beginningMark) && listOfSqaures[2].row[2].buttonSymbol.Equals(beginningMark))
            {
                return true;
            }
        }
        return false;
    }

    public bool ReverseDiagonalCheck()
    {
        string beginningMark = listOfSqaures[0].row[2].buttonSymbol;
        if (beginningMark != "")
        {
            if (listOfSqaures[1].row[1].buttonSymbol.Equals(beginningMark) && listOfSqaures[2].row[0].buttonSymbol.Equals(beginningMark))
            {
                return true;
            }
        }
        return false;
    }

    /*public void Reset()
    {
        foreach (Rows row in cell.Value)
        {
            foreach (Squares sqaure in row.rowsList)
            {
                sqaure.mark = "";
                sqaure.button.GetComponent<Image>().sprite = null;
                sqaure.button.interactable = true;
            }
        }
        mainGameScript.Reset();
    }*/
}
