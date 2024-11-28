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
    //adding this for the purposes of testing the bidding war, thus the unorthodox formatting (for easier deletion)
    public Button biddingWarButton; //TODO: delete after testing
    public GameObject playerListGameObject;
    public GameObject playerListEntryPrefab;
    private Image readyButtonImage;
    private readonly Dictionary<ulong, (bool ready, Transform tile)> playerTiles = new();  // For each user i will store if he is ready and his text on playerListGameObject
    private Color readyColor = Color.green;
    private Color notReadyColor = Color.red;

    [Rpc(SendTo.Everyone)]
    private void LoadBWHostRpc()
    {
        _ = NetworkManager.SceneManager.LoadScene("BiddingWar", LoadSceneMode.Single);
    }

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AddPlayerToListRpc;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayerFromListRpc;

        if (NetworkManager.Singleton.IsHost)
        {
            // NetworkObject.Spawn();
        }
    }

    private void Start()
    {
        //to delete after testing start
        biddingWarButton.onClick.AddListener(LoadBWHostRpc);

        //to delete after testing end
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);
        startButton.onClick.AddListener(StartGameRpc);
        startButton.interactable = false;

        if (NetworkManager.Singleton.IsHost)
        {
            AddPlayerToList(NetworkManager.Singleton.LocalClientId);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.Singleton.OnClientConnectedCallback -= AddPlayerToListRpc;
        NetworkManager.Singleton.OnClientDisconnectCallback -= RemovePlayerFromListRpc;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameRpc()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _ = NetworkManager.SceneManager.LoadScene("Wheel", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AddPlayerToListRpc(ulong clientId)
    {
        AddPlayerToList(clientId);
    }

    void AddPlayerToList(ulong clientId)
    {
        GameObject playerListEntry = Instantiate(playerListEntryPrefab, playerListGameObject.transform);
        playerTiles[clientId] = (false, playerListEntry.transform);
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        playerListEntry.GetComponentInChildren<TextMeshProUGUI>().text = playerObject.GetComponent<Team>().TeamName;
        SetPlayerReady(false, clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void RemovePlayerFromListRpc(ulong clientId)
    {
        Destroy(playerTiles[clientId].tile.gameObject);
        _ = playerTiles.Remove(clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastPlayerReadySetRpc(bool ready, ulong clientId)
    {
        SetPlayerReady(ready, clientId);
    }

    void SetPlayerReady(bool ready, ulong clientId)
    {
        playerTiles[clientId].tile.GetComponentInChildren<TextMeshProUGUI>().color = ready ? readyColor : notReadyColor;
        playerTiles[clientId] = (ready, playerTiles[clientId].tile);

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            readyButtonImage.color = ready ? readyColor : notReadyColor;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            startButton.interactable = playerTiles.All(pair => pair.Value.ready) && playerTiles.Count > 1;
        }
    }

    public void OnPlayerReadySwitch()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        BroadcastPlayerReadySetRpc(!playerTiles[localClientId].ready, NetworkManager.Singleton.LocalClientId);
    }
}