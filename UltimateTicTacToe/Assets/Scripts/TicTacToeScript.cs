using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeScript : NetworkBehaviour
{

    public struct Cell : INetworkSerializable, System.IEquatable<Cell>
    {
        public FixedString64Bytes buttonName;
        public FixedString64Bytes mark;

        public  Cell(string button, string m)
        {
            buttonName = button;
            mark = m;
        }

        public bool Equals(Cell other)
        {
            return buttonName.Equals(other.buttonName) && mark.Equals(other.mark);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref mark);
            serializer.SerializeValue(ref buttonName);
        }
    }

    public Sprite xImage;
    public Sprite oImage;
    public NetworkList<Cell> cellList;
    public MainGameScript mainGameScript;
    public NetworkVariable<PlayerScript> xPlayer = null;
    public NetworkVariable<PlayerScript> oPlayer = null;

    private string playerTurn = "";
    // Start is called before the first frame update
    void Start()
    {
        settingNetworkList();
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
        Cell cellClicked = new Cell(button.transform.name, "");
        foreach(Cell cell in cellList)
        {
            if (cell.Equals(cellClicked))
            {
                cellClicked.mark = playerTurn;
                cellList[cellList.IndexOf(cell)] = cellClicked;
                break;
            }
        }
        if (VerticalCheck() || HorizontalCheck() || DiagonalCheck() || ReverseDiagonalCheck())
        {
            Debug.Log("Winner: Player " + playerTurn);
            mainGameScript.setPlayerWin(playerTurn);
            foreach (Transform cell in transform)
            {
                transform.GetComponent<Button>().interactable = false;
            }
        }
        if (playerTurn == "X")
        {
            img = xImage;
            playerTurn = "O";
            oPlayer.Value.enablePlayer();
            xPlayer.Value.disablePlayer();
        }
        else
        {
            img = oImage;
            playerTurn = "X";
            xPlayer.Value.enablePlayer();
            oPlayer.Value.disablePlayer();
        }
        mainGameScript.setPlayerTurn(playerTurn);
        button.GetComponent<Image>().sprite = img;
        button.interactable = false;
    }

    public bool VerticalCheck()
    {
        for (int i = 0; i < cellList.Count; i += 3)
        {
            string mark = cellList[i].mark.ToString();
            if (mark != "")
            {
                if (cellList[i+1].mark == mark && cellList[i+2].mark == mark)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HorizontalCheck()
    {
        for (int i = 0; i < cellList.Count; i++)
        {
            string mark = cellList[i].mark.ToString();
            if (mark != "")
            {
                if (cellList[i + 3].mark == mark && cellList[i + 6].mark == mark)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool DiagonalCheck()
    {
        string mark = cellList[0].mark.ToString();
        if (mark != "")
        {
            if (cellList[4].mark == mark && cellList[8].mark == mark)
            {
                return true;
            }
        }
        return false;
    }

    public bool ReverseDiagonalCheck()
    {
        string mark = cellList[2].mark.ToString();
        if (mark != "")
        {
            if (cellList[4].mark == mark && cellList[6].mark == mark)
            {
                return true;
            }
        }
        return false;
    }

    public void Reset()
    {
        settingNetworkList();
        foreach (Transform cell in transform)
        {
            if (cell.name.Contains("Cell"))
            {
                transform.GetComponent<Button>().interactable = true;
                transform.GetComponent<Image>().sprite = null;
            }
        }
        mainGameScript.Reset();
    }

    public void settingNetworkList()
    {
        cellList = new NetworkList<Cell>();
        foreach (Transform cell in transform)
        {
            if (cell.name.Contains("Cell"))
            {
                cellList.Add(new Cell(cell.name, ""));
            }
        }
    }
}
