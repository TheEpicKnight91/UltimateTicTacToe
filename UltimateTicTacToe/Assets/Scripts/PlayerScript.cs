using Unity.Netcode;

public class PlayerScript : NetworkBehaviour
{

    private string playerSymbol;
    private ulong playerClientID;
    private string playerName;
    // Start is called before the first frame update
    void Start()
    {
        playerSymbol = "";
    }

    public string getPlayerSymbol()
    {
        return playerSymbol;
    }

    public void setPlayerSymbol(string playerSymbol)
    {
        this.playerSymbol = playerSymbol;
    }

    public void setPlayerClientID(ulong playerClientID)
    {
        this.playerClientID = playerClientID;
    }

    public ulong getPlayerClientID()
    {
        return this.playerClientID;
    }

    public void setPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

    public string getPlayerName()
    {
        return this.playerName;
    }
}
