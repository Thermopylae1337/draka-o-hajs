using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class LobbyController : NetworkBehaviour
{
    // Używam Jednego obiektu do zarządzania logiką lobby, bo lobby musi być
    // reaktywne na wiele związanych ze sobą i zdarzen i w wielu miejscach.
    public Button startButton;
    public Button readyButton;
    public Button licyButton;
    public GameObject playerList;
    public GameObject playerListEntryPrefab;
    public NetworkManager NetMan;
    public string Team_Name; 
    public int default_balance_value = 10000;
    // !Licytacja tworzę drużyny żeby móc przypisać kążdego gracza do danej drużyny, na razie będzie to po prostu na zasadzie pierwszy gracz-pierwsza drużyna
    public List<string> Remaining_Teams = new List<string> { "zieloni","żółci" ,"niebiescy"  };
    public List<Team> Teams = new List<Team>();



    private Image readyButtonImage;
    private bool currentReady = true; // Will be changed on Start to false
    private Dictionary<ulong, bool> playersReadyStatus = new Dictionary<ulong, bool>();

    Color readyColor = Color.green;
    Color notReadyColor = Color.red;


    
    void Start()
    {
        NetMan = NetworkManager;
        //Debug.Log("netman " + netman.GetInstanceID());
        General_Game_Data.NetMan = NetMan;
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(SwitchReady);
        licyButton.onClick.AddListener(Load_Lic_Host );
        
        SwitchReady();

        if (!IsHost)
        {
            Add_TeamRpc(NetMan.LocalClientId);

        }
        else
        {
            Add_TeamRpc(NetMan.LocalClientId);
            /*
            Teams.Add(Remaining_Teams[0]);
            Remaining_Teams.RemoveAt(0);
            Teams[0].ID = NetMan.LocalClientId;*/
        }

    }



    [Rpc(SendTo.Server)]
    public void Add_TeamRpc(ulong id)
    {
        //na razie nie sprawdza czy wystarczy drużyn, po prostu liczy że ich wystarczy
        if (Remaining_Teams.Count > 0)
        { 
            Teams.Add ( new Team( Remaining_Teams[0],id));
            Remaining_Teams.RemoveAt(0); 
            Update_Team_ListRpc(General_Game_Data.Team_List_Serializer(Teams));
            
           // Receive_Team_Info_Rpc(id, Teams[Teams.Count - 1].Name);
        }
    }
    [Rpc(SendTo.NotServer)]
    public void Update_Team_ListRpc(string LoT) 
    {
        
        Teams = General_Game_Data.Team_List_Deserializer(LoT); 
    }
    public void Load_Lic_Host()
    {
        if (IsHost) {
            List<Team> lot = new List<Team>();
             lot = this.Teams;
        Load_LicRpc();
    }}


    [Rpc(SendTo.Everyone)]
    public void Load_LicRpc( ) 
    {
        General_Game_Data.Teams = Teams;
        General_Game_Data._is_host = IsHost;
        General_Game_Data.ID = NetMan.LocalClientId ; 
        if (!IsHost)
        {
            Debug.LogWarning(Teams[0].Serialize());
            Debug.LogWarning(Teams[1].Serialize());
        }
        else
        {
            NetMan.SceneManager.LoadScene("Bidding_War", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void Receive_Team_Info_Rpc(ulong recipient_id, string Team_Name) {
        if (NetMan.LocalClientId==recipient_id) {
            this.Team_Name = Team_Name; 
        }
    } 

























    public void SwitchReady()
    {
        currentReady = !currentReady;
        BroadcastPlayerReadyRpc(currentReady, NetworkManager.LocalClientId);
        readyButtonImage.color = currentReady ? readyColor : notReadyColor;
    }

    public void OnStartGame()
    {

    }
    [Rpc(SendTo.Everyone)]
   

    void BroadcastPlayerReadyRpc(bool ready, ulong playerId)
    {
        playersReadyStatus[playerId] = ready;
        RefreshPlayerList();
    }
    public void RefreshPlayerList()
    {
        foreach (var playerId in NetworkManager.ConnectedClientsIds)
        {
            Transform entry = playerList.transform.Find(playerId.ToString());
            if (!entry)
            {
                playersReadyStatus[playerId] = false;
                entry = Instantiate(playerListEntryPrefab, playerList.transform).transform;
                entry.name = playerId.ToString();
            }
            TMP_Text textComponent = entry.GetComponentInChildren<TMP_Text>();
            textComponent.text = playerId.ToString();
            textComponent.color = playersReadyStatus[playerId] ? readyColor : notReadyColor;
        }

        startButton.interactable = IsHost && playersReadyStatus.Values.All(x => x);
    }

}
