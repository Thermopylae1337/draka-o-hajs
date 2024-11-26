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
    public Button biddingWarButton; 
    public string teamName; 
    public List<string> remainingTeams = new(){ "zieloni", "żółci", "niebiescy" };
    public List<Team> teams = new();

    [Rpc(SendTo.Server)]
    public void AddTeamRpc(ulong id)
    {
        //na razie nie sprawdza czy wystarczy drużyn, po prostu liczy że ich wystarczy
        if (remainingTeams.Count > 0)
        {
            teams.Add(new Team(remainingTeams[0], (int)id));
            remainingTeams.RemoveAt(0);
            UpdateTeamListRpc(new ListOfTeams(teams));
        }
    }

    [Rpc(SendTo.NotServer)]
    public void UpdateTeamListRpc(ListOfTeams LoT)
    {
        teams = LoT.list;
    }
    public void LoadBWHost()
    {
        if (IsHost)
        {
            LoadBWRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    public void LoadBWRpc()
    {
        General_Game_Data.teams = teams;
        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Bidding_War", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void ReceiveTeamInfoRpc(ulong recipient_id, string Team_Name)
    {
        if  (NetworkManager.Singleton.LocalClientId == recipient_id)
        {
            this.teamName = Team_Name;
        }
    }
    //end, check out Start() too
    public GameObject playerListGameObject;
    public GameObject playerListEntryPrefab;
    private Image readyButtonImage;
    private bool selfReady = false;
    private readonly Dictionary<ulong, (bool, Transform, Team)> playerList = new();  // For each user i will store if he is ready and his text on playerListGameObject
 

    private Color readyColor = Color.green;
    private Color notReadyColor = Color.red; 

    private void Start()
    {
        //to delete after testing start 
        biddingWarButton.onClick.AddListener(LoadBWHost);

        AddTeamRpc(NetworkManager.Singleton.LocalClientId);

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
    private void RequestReadyBroadcastRpc() => BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId, Utils.CurrentTeam);

    [Rpc(SendTo.Everyone)]
    private void BroadcastPlayerJoinedRpc(ulong clientId, Team team) => AddPlayerToList(clientId, team);

    private void AddPlayerToList(ulong clientId, Team team)
    {
        GameObject playerListTile = Instantiate(playerListEntryPrefab, playerListGameObject.transform);
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
    private void BroadcastPlayerReadySetRpc(bool ready, ulong clientId, Team team)
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
        if (IsHost)
        {
            DisconnectClientsRpc();
        }

        DisconnectSelf();
    }

    [Rpc(SendTo.NotMe)]
    private void DisconnectClientsRpc() => DisconnectSelf();

    private void DisconnectSelf()
    {
        BroadcastPlayerLeftRpc(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    [Rpc(SendTo.Everyone)]
    private void BroadcastPlayerLeftRpc(ulong clientId)
    {
        Destroy(playerList[clientId].Item2.gameObject);
        _ = playerList.Remove(clientId);
    }
}
