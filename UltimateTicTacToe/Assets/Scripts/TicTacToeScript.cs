using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEditor.VersionControl;
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
    public Button newGameBtn;
    public Button exitGameBtn;
    public TextMeshProUGUI mainText;

    private PlayerScript playerTurn;
    private PlayerScript xPlayer;
    private PlayerScript oPlayer;
    private string message;

    void Awake()
    {
        cellList = new NetworkList<Cell>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            settingNetworkList();
        }
        newGameBtn.gameObject.SetActive(false);
        exitGameBtn.gameObject.SetActive(false);
        message = "Player " + playerTurn + "'s Turn";
        mainText.text = message;
        xPlayer = GameObject.FindWithTag("xPlayer").GetComponent<PlayerScript>();
        oPlayer = GameObject.FindWithTag("oPlayer").GetComponent<PlayerScript>();
        xPlayer.setTicTacToeScript(this);
        oPlayer.setTicTacToeScript(this);
        playerTurn = (PlayerScript)new PlayerScript[] { xPlayer, oPlayer }.GetValue(Random.Range(0, 2));
        if (playerTurn == xPlayer)
        {
            Debug.Log(oPlayer.getClientId());
            oPlayer.disablePlayerClientRpc(createClientParams(oPlayer.getClientId()));
            Debug.Log("Disabling O Player");
        }
        else if (playerTurn == oPlayer)
        {
            Debug.Log(xPlayer.getClientId());
            xPlayer.disablePlayerClientRpc(createClientParams(xPlayer.getClientId()));
            Debug.Log("Disabling X Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public PlayerScript getPlayerTurn()
    {
        return playerTurn;
    }

    public ClientRpcParams createClientParams(ulong clientId)
    {
        return new ClientRpcParams()
        {
            Send = new ClientRpcSendParams()
            {
                TargetClientIds = new[] { clientId }
            }
        };
    }
    public void choseSquare(Button button)
    {
        updateSquareServerRpc(button.transform.name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateSquareServerRpc(string button)
    {
        Sprite img;
        GameObject btn = GameObject.Find(button);
        Cell cellClicked = new Cell(button, "");
        foreach(Cell cell in cellList)
        {
            if (cell.Equals(cellClicked))
            {
                cellClicked.mark = playerTurn.getPlayerSymbol();
                cellList[cellList.IndexOf(cell)] = cellClicked;
                break;
            }
        }
        img = changePlayerTurn();
        btn.GetComponent<Image>().sprite = img;
        btn.GetComponent<Button>().interactable = false;
        if (!checkWinConditions())
        {
            message = "Player " + playerTurn + "'s Turn";
            mainText.text = message;
        }
    }

    public bool checkWinConditions()
    {
        if (verticalCheck() || horizontalCheck() || diagonalCheck() || reverseDiagonalCheck())
        {
            Debug.Log("Winner: Player " + playerTurn);
            message = "Player " + playerTurn + " Wins";
            mainText.text = message;
            newGameBtn.gameObject.SetActive(true);
            exitGameBtn.gameObject.SetActive(true);
            foreach (Transform cell in transform)
            {
                if (cell.name.Contains("Cell"))
                {
                    transform.GetComponent<Button>().interactable = false;
                }
            }
            return true;
        }
        return false;
    }

    public Sprite changePlayerTurn()
    {
        Sprite img;
        if (playerTurn == xPlayer)
        {
            playerTurn = oPlayer;
            oPlayer.enablePlayerClientRpc(createClientParams(oPlayer.getClientId()));
            xPlayer.disablePlayerClientRpc(createClientParams(xPlayer.getClientId()));
            img = xImage;
        }
        else
        {
            playerTurn = xPlayer;
            xPlayer.enablePlayerClientRpc(createClientParams(xPlayer.getClientId()));
            oPlayer.disablePlayerClientRpc(createClientParams(oPlayer.getClientId()));
            img = oImage;
        }
        return img;
    }
    public bool verticalCheck()
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

    public bool horizontalCheck()
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

    public bool diagonalCheck()
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

    public bool reverseDiagonalCheck()
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
        cellList = new NetworkList<Cell>();
        settingNetworkList();
        foreach (Transform cell in transform)
        {
            if (cell.name.Contains("Cell"))
            {
                transform.GetComponent<Button>().interactable = true;
                transform.GetComponent<Image>().sprite = null;
            }
        }
        newGameBtn.gameObject.SetActive(false);
        exitGameBtn.gameObject.SetActive(false);
        playerTurn = (PlayerScript)new PlayerScript[] { xPlayer, oPlayer }.GetValue(Random.Range(0, 2));
        message = "Player " + playerTurn + "'s Turn";
        mainText.text = message;
    }

    public void settingNetworkList()
    {
        foreach (Transform cell in transform)
        {
            if (cell.name.Contains("Cell"))
            {
                cellList.Add(new Cell(cell.name, ""));
            }
        }
    }
}
