using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
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
    public Button mainMenuButton;
    public Button readyButton;
    public GameObject playerListGameObject;
    public GameObject playerListEntryPrefab;
    private Image readyButtonImage;
    private readonly Dictionary<ulong, (bool ready, Transform tile)> playerTiles = new();  // For each user i will store if he is ready and his text on playerListGameObject
    private Color readyColor = Color.green;
    private Color notReadyColor = Color.red;
    private NetworkObject playerObj;
    public GameObject gameManagerPrefab;

    private void LoadBWHostRpc()
    {
        _ = NetworkManager.Singleton.SceneManager.LoadScene("Bidding_War", LoadSceneMode.Single);
    }

    private void Start()
    {
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);
        startButton.onClick.AddListener(StartGameRpc);
        mainMenuButton.onClick.AddListener(Disconnect);

        startButton.interactable = false;

        if (NetworkManager.Singleton.IsHost)
        {
            AddPlayerToList(NetworkManager.Singleton.LocalClientId);
        }
    }

    void LoadAllPlayers(ulong ClientID)
    {
        foreach (ulong item in NetworkManager.ConnectedClientsIds)
        {
            AddPlayerToList(item);
        }
    }

    void OnEnable()
    {
        // NetworkManager.Singleton.OnClientConnectedCallback += AddPlayerToList;
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback +=
                AddPlayerToListRpc;
            NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayerFromListRpc;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback += LoadAllPlayers;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += LoadMainMenu;
    }

    void OnDisable()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= AddPlayerToListRpc;
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemovePlayerFromListRpc;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= LoadAllPlayers;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback -= LoadMainMenu;
    }

    private void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void LoadMainMenu(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameRpc()
    {
        AddColoursToTeams();

        foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            playerObj = NetworkManager.Singleton.ConnectedClients[client.Key].PlayerObject;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            _ = Instantiate(gameManagerPrefab);
            GameManager.Instance.GetComponent<NetworkObject>().Spawn();

            GameManager.Instance.StartingTeamCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
            {
                client.PlayerObject.GetComponent<TeamManager>().NetworkId = (uint)client.ClientId;
            }

            _ = NetworkManager.SceneManager.LoadScene("CategoryDraw", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.Everyone)]
    void AddPlayerToListRpc(ulong clientId)
    {
        AddPlayerToList(clientId);
        List<ulong> playerKeys = playerTiles.Keys.ToList();
        foreach (ulong key in playerKeys)
        {
            SetPlayerReady(false, key);
            NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[key].PlayerObject;
            playerObject.name = playerObject.GetComponent<TeamManager>().teamName.Value.ToString();
        }
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

        playerObject.GetComponent<TeamManager>().teamName.OnValueChanged = delegate (FixedString64Bytes oldName, FixedString64Bytes newName)
        {
            playerListEntry.GetComponent<TextMeshProUGUI>().text = newName.ToString();
        };

        SetPlayerReady(false, clientId);
    }

    void AddColoursToTeams()
    {
        int i = 0;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values.ToList())
        {
            client.PlayerObject.GetComponent<TeamManager>().Colour = (ColourEnum)i;
            i++;
        }
    }

    [Rpc(SendTo.Everyone)]
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

        if (clientId == NetworkManager.Singleton.LocalClientId && readyButtonImage != null)
        {
            Image image = readyButtonImage.GetComponent<Image>();
            image.color = ready ? readyColor : notReadyColor;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            startButton.interactable = playerTiles.All(pair => pair.Value.ready) && playerTiles.Count > 1 && playerTiles.Count <= 3;
        }
    }

    public void OnPlayerReadySwitch()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        BroadcastPlayerReadySetRpc(!playerTiles[localClientId].ready, NetworkManager.Singleton.LocalClientId);
    }
}