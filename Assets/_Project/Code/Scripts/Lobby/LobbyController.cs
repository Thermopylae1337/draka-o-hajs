using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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

        // NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayerFromListRpc;

        //to delete after testing start
        biddingWarButton.onClick.AddListener(LoadBWHostRpc);

        //to delete after testing end
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);
        startButton.onClick.AddListener(StartGameRpc);
    }

    private void Start()
    {
        startButton.interactable = false;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AddPlayerToListRpc(clientId);
        }

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<TeamManager>().teamName.Value = TeamCreatorController.chosenTeamName;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameRpc()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _ = NetworkManager.SceneManager.LoadScene("CategoryDraw", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void AddPlayerToListRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>().teamName.OnValueChanged = (FixedString64Bytes oldName, FixedString64Bytes newName) => AddPlayerToList(clientId);
        AddPlayerToList(clientId);
    }

    void AddPlayerToList(ulong clientId)
    {
        GameObject playerListEntry;
        if (!playerTiles.ContainsKey(clientId))
        {
            playerListEntry = Instantiate(playerListEntryPrefab, playerListGameObject.transform);
            playerTiles[clientId] = (false, playerListEntry.transform);
        }
        else
        {
            playerListEntry = playerTiles[clientId].tile.gameObject;
        }

        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        playerListEntry.GetComponent<TextMeshProUGUI>().text = playerObject.GetComponent<TeamManager>().teamName.Value.ToString();
        playerObject.name = playerObject.GetComponent<TeamManager>().teamName.Value.ToString();
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
        playerTiles[clientId].tile.GetComponent<TextMeshProUGUI>().color = ready ? readyColor : notReadyColor;
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