using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeScript : NetworkBehaviour
{

    public class Square
    {
        public Button buttonName;
        public string mark;

        public Square(Button button, string m)
        {
            buttonName = button;
            mark = m;
        }
    }

    public struct Row : INetworkSerializeByMemcpy
    {
        public List<Square> squares;

        public Row(List<Square> s)
        {
            squares = s;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            //serializer.SerializeValue(ref squares);
        }
    }

    public Sprite xImage;
    public Sprite oImage;
    //private static readonly NetworkList<Row> networkList = new();
    //public NetworkList<Row> cell = networkList;
    public MainGameScript mainGameScript;
    public NetworkVariable<PlayerScript> xPlayer = null;
    public NetworkVariable<PlayerScript> oPlayer = null;

    private string playerTurn = "";
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            xPlayer.Value = GameObject.FindWithTag("xPlayer").GetComponent<PlayerScript>();
        }
        else
        {
            oPlayer.Value = GameObject.FindWithTag("oPlayer").GetComponent<PlayerScript>();
        }
        if (playerTurn == "X")
        {
            oPlayer.Value.disablePlayer();
        }
        else
        {
            xPlayer.Value.disablePlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayerTurn(string player)
    {
        playerTurn = player.ToUpper();
    }
    public string getPlayerTurn()
    {
        return playerTurn;
    }
    public void ChoseSquare(Button button)
    {
        Sprite img;
        /*foreach(Rows row in cell.Value)
        {
            foreach(Squares square in row.rowsList)
            {
                if (square.button == button)
                {
                    square.mark = playerTurn;
                    break;
                }
            }
        }
        if (VerticalCheck() || HorizontalCheck() || DiagonalCheck() || ReverseDiagonalCheck())
        {
            Debug.Log("Winner: Player " + playerTurn);
            mainGameScript.setPlayerWin(playerTurn);
            foreach(Rows row in cell.Value)
            {
                foreach(Squares sqaure in row.rowsList)
                {
                    sqaure.button.interactable = false;
                }
            }
        }*/
        if (playerTurn == "X")
        {
            img = xImage;
            playerTurn = "O";
            //oPlayer.Value.enablePlayer();
            //xPlayer.Value.disablePlayer();
        }
        else
        {
            img = oImage;
            playerTurn = "X";
            //xPlayer.Value.enablePlayer();
            //oPlayer.Value.disablePlayer();
        }
        mainGameScript.setPlayerTurn(playerTurn);
        button.GetComponent<Image>().sprite = img;
        button.interactable = false;
    }

    /*public bool VerticalCheck()
    {
        foreach(Rows row in cell.Value)
        {
            string beginningMark = row.rowsList[0].mark;
            if (beginningMark != "")
            {
                if (row.rowsList[1].mark == beginningMark && row.rowsList[2].mark == beginningMark)
                {
                    return true;
                }
            }
        }
        return false;
    }*/

    /*public bool HorizontalCheck()
    {
        for (int i = 0; i < cell.Value.Count; i++)
        {
            string beginningMark = cell.Value[0].rowsList[i].mark;
            if (beginningMark != "")
            {
                if (cell.Value[1].rowsList[i].mark == beginningMark && cell.Value[2].rowsList[i].mark == beginningMark)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool DiagonalCheck()
    {
        string beginningMark = cell.Value[0].rowsList[0].mark;
        if (beginningMark != "")
        {
            if (cell.Value[1].rowsList[1].mark == beginningMark && cell.Value[2].rowsList[2].mark == beginningMark)
            {
                return true;
            }
        }
        return false;
    }

    public bool ReverseDiagonalCheck()
    {
        string beginningMark = cell.Value[0].rowsList[2].mark;
        if (beginningMark != "")
        {
            if (cell.Value[1].rowsList[1].mark == beginningMark && cell.Value[2].rowsList[0].mark == beginningMark)
            {
                return true;
            }
        }
        return false;
    }

    public void Reset()
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
