using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
    // Używam Jednego obiektu do zarządzania logiką lobby, bo lobby musi być
    // reaktywne na wiele związanych ze sobą i zdarzen i w wielu miejscach.
    public Button startButton;
    public Button readyButton;

    //adding this for the purposes of testing the bidding war, thus the unorthodox formatting (for easier deletion)
    public Button BiddingWarButton;
    public NetworkManager NetMan;
    public string Team_Name;
    public int default_balance_value = 10000;
    public List<string> Remaining_Teams = new List<string> { "zieloni", "żółci", "niebiescy" };
    public List<Team> Teams = new List<Team>();


    [Rpc(SendTo.Server)]
    public void Add_TeamRpc(ulong id)
    {
        //na razie nie sprawdza czy wystarczy drużyn, po prostu liczy że ich wystarczy
        if (Remaining_Teams.Count > 0)
        {
            Teams.Add(new Team(Remaining_Teams[0], (int)id));
            Remaining_Teams.RemoveAt(0);
            Update_Team_ListRpc(new ListOfTeams(Teams));
        }
    }

    [Rpc(SendTo.NotServer)]
    public void Update_Team_ListRpc(ListOfTeams LoT)
    {
        Teams = LoT.list;
    }
    public void Load_BW_Host()
    {
        if (IsHost)
        {
            List<Team> lot = new List<Team>();
            lot = this.Teams;
            Load_BWRpc();
        }
    }


    [Rpc(SendTo.Everyone)]
    public void Load_BWRpc()
    {
        General_Game_Data.Teams = Teams;
        General_Game_Data._is_host = IsHost;
        General_Game_Data.ID = NetMan.LocalClientId;
        if (IsHost)
        {
            NetMan.SceneManager.LoadScene("Bidding_War", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void Receive_Team_Info_Rpc(ulong recipient_id, string Team_Name)
    {
        if (NetMan.LocalClientId == recipient_id)
        {
            this.Team_Name = Team_Name;
        }
    }
    //end, check out Start() too
    public GameObject playerListGameObject;
    public GameObject playerListEntryPrefab;
    private Image readyButtonImage;
    private bool selfReady = false;
    private readonly Dictionary<ulong, (bool, Transform, Team)> playerList = new();  // For each user i will store if he is ready and his text on playerListGameObject
    Color readyColor = Color.green;
    Color notReadyColor = Color.red;

    void Start()
    {
        //to delete after testing start
        NetMan = this.NetworkManager;
        General_Game_Data.NetMan = NetMan;
        BiddingWarButton.onClick.AddListener(Load_BW_Host);

        Add_TeamRpc(NetMan.LocalClientId);


        //to delete after testing  end
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);
        startButton.interactable = false;
        OnSelfJoin();
    }

    public void OnSelfJoin()
    {
        selfReady = false;
        // foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        // {
        //     AddPlayerToList(clientId);
        // }

        RequestReadyBroadcastRpc();
        BroadcastPlayerJoinedRpc(NetworkManager.Singleton.LocalClientId, Utils.CurrentTeam);
        BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId, Utils.CurrentTeam);
    }

    [Rpc(SendTo.NotMe)]
    void RequestReadyBroadcastRpc()
    {
        BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId, Utils.CurrentTeam);
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastPlayerJoinedRpc(ulong clientId, Team team)
    {
        AddPlayerToList(clientId, team);
    }

    private void AddPlayerToList(ulong clientId, Team team)
    {
        var playerListTile = Instantiate(playerListEntryPrefab, playerListGameObject.transform);
        playerListTile.name = $"PlayerListTile_{clientId}";
        playerListTile.GetComponent<TMP_Text>().text = team.Name;
        playerList[clientId] = (false, playerListTile.transform, team);
    }

    public void OnPlayerReadySwitch()
    {
        selfReady = !selfReady;
        readyButtonImage.color = selfReady ? readyColor : notReadyColor;
        BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId, Utils.CurrentTeam);
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastPlayerReadySetRpc(bool ready, ulong clientId, Team team)
    {
        if (!playerList.ContainsKey(clientId))
        {
            AddPlayerToList(clientId, team);
        };
        Transform playerListTile = playerList[clientId].Item2;
        playerList[clientId] = (ready, playerListTile, team);

        playerList[clientId].Item2.GetComponent<TMP_Text>().color = ready ? readyColor : notReadyColor;

        if (IsHost)
        {
            startButton.interactable = playerList.Values.All(x => x.Item1) && playerList.Count > 1 && playerList.Count < 4;
        }
    }

    public void OnPlayerLeave()
    {
        if (IsHost) DisconnectClientsRpc();
        DisconnectSelf();
    }


    [Rpc(SendTo.NotMe)]
    private void DisconnectClientsRpc()
    {
        DisconnectSelf();
    }

    private void DisconnectSelf()
    {
        BroadcastPlayerLeftRpc(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastPlayerLeftRpc(ulong clientId)
    {
        Destroy(playerList[clientId].Item2.gameObject);
        playerList.Remove(clientId);
    }
}
