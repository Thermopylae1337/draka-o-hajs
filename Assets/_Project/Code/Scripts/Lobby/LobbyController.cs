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

/// <summary>
/// Klasa zarządzająca lobby.
/// </summary>
public class LobbyController : NetworkBehaviour
{
    // Używam Jednego obiektu do zarządzania logiką lobby, bo lobby musi być
    // reaktywne na wiele związanych ze sobą i zdarzen i w wielu miejscach.
    /// <summary>
    /// Obiekt reprezentujący przycisk rozpoczynający rozgrywkę.
    /// </summary>
    public Button startButton;
    /// <summary>
    /// Obiekt reprezentujący przycisk menu głównego
    /// </summary>
    public Button mainMenuButton;
    /// <summary>
    /// Obiekt reprezentujący przycisk gotowości.
    /// </summary>
    public Button readyButton;
    //adding this for the purposes of testing the bidding war, thus the unorthodox formatting (for easier deletion)
    public Button biddingWarButton; //TODO: delete after testing
    
    /// <summary>
    /// Obiekt gry reprezentujący listę graczy w grze.
    /// </summary>
    public GameObject playerListGameObject;
    /// <summary>
    /// Prefab obiektu gry (GameObject), który reprezentuje pojedynczy wpis na liście graczy.
    /// </summary>
    public GameObject playerListEntryPrefab;
    /// <summary>
    /// Obrazek reprezentujący przycisk "Gotowy". 
    /// </summary>
    private Image readyButtonImage;
    /// <summary>
    /// Słownik przechowujący dane o graczach, w tych stan gotowości.
    /// </summary>
    private readonly Dictionary<ulong, (bool ready, Transform tile)> playerTiles = new();  // For each user i will store if he is ready and his text on playerListGameObject
    /// <summary>
    /// Kolor przypisywany do gracza, który jest gotowy.
    /// </summary>
    private Color readyColor = Color.green;
    /// <summary>
    /// Kolor przypisywany do gracza, który nie jest gotowy.
    /// </summary>
    private Color notReadyColor = Color.red;
    /// <summary>
    /// Pole przechowujące referencje do obiektu sieciowego, który reprezentuje gracza.
    /// </summary>
    private NetworkObject playerObj;
    /// <summary>
    /// Pole przechowujące referencje do klasy GameManager, odpowiedzialnej za zarządzanie logiką gry.
    /// </summary>
    private GameManager gameManager;

    /// <summary>
    /// Metoda RPC służąca do załadowania sceny "Bidding_War".
    /// </summary>
    private void LoadBWHostRpc()
    {
        _ = NetworkManager.Singleton.SceneManager.LoadScene("Bidding_War", LoadSceneMode.Single);
    }

    /// <summary>
    /// Metoda wywoływana przy uruchomieniu skryptu. Inicjalizuje przyciski interfejsu użytkownika i ustawia odpowiednie nasłuchiwania na kliknięcia.
    /// Blokuje przycisk rozpoczęcia gry i dodaje lokalnego gracza do listy, jeśli jest hostem.
    /// </summary>
    /// </summary>
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

    /// <summary>
    /// Metoda która połączy wszystkich graczy. 
    /// </summary>
    /// <param name="ClientID">Zmienna przechowująca identyfikator gracza.</param>
    void LoadAllPlayers(ulong ClientID)
    {
        foreach (ulong item in NetworkManager.ConnectedClientsIds)
        {
            AddPlayerToList(item);
        }
    }

    /// <summary>
    /// Rejestruje odpowiednie metody obsługi zdarzeń połączenia i rozłączenia klientów w zależności od tego, czy klient jest hostem.
    /// </summary>
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

    /// <summary>
    /// Wyrejestrowuje metody obsługi zdarzeń połączenia i rozłączenia klientów.
    /// </summary>
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

    /// <summary>
    /// Metoda rozłączająca z gry.
    /// </summary>
    private void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    /// <summary>
    /// Metoda, która umożliwia załadowanie menu głównego graczom.
    /// </summary>
    /// <param name="clientId">Zmienna przechowująca identyfikator gracza.</param>
    private void LoadMainMenu(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Metoda RPC rozpoczynająca rozgrywkę wywoływana przez hosta. Przypisuje kolory drużynom, inicjując menadżera gry oraz ładuje odpowiednią scenę.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameRpc()
    {
        AddColoursToTeams();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            playerObj = NetworkManager.Singleton.ConnectedClients[client.Key].PlayerObject;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            gameManager.StartingTeamCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
            {
                client.PlayerObject.GetComponent<TeamManager>().NetworkId = (uint)client.ClientId;
            }

            _ = NetworkManager.SceneManager.LoadScene("CategoryDraw", LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Metoda RPC wywołująca dodanie gracza do listy graczy na wszystkich klientach.
    /// </summary>
    /// <param name="clientId">Zmienna przechowująca identyfikator gracza.</param>
    [Rpc(SendTo.Everyone)]
    void AddPlayerToListRpc(ulong clientId)
    {
        AddPlayerToList(clientId);
    }

    /// <summary>
    /// Metoda dodająca gracza do listy graczy w interfejsie użytkownika. Sprawdza, czy gracz już istnieje na liście, a jeśli nie tworzy nowy wpis. Ustawia nazwę drużyny gracza oraz możliwość oznaczenia go jako gotowego.
    /// </summary>
    /// <param name="clientId">Zmienna przechowująca identyfikator gracza.</param>
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

    /// <summary>
    /// Metoda przypisująca kolory drużynom. Iteruje przez wszystkich połączonych graczy i przypisuje im odpowiedni kolor drużyny na podstawie indeksu.
    /// </summary>
    void AddColoursToTeams()
    {
        int i = 0;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values.ToList())
        {
            client.PlayerObject.GetComponent<TeamManager>().Colour = (ColourEnum)i;
            i++;
        }
    }

    /// <summary>
    /// Metoda RPC usuwająca graczy z listy.
    /// </summary>
    /// <param name="clientId">Zmienna przechowująca identyfikator gracza.</param>
    [Rpc(SendTo.Everyone)]
    void RemovePlayerFromListRpc(ulong clientId)
    {
        Destroy(playerTiles[clientId].tile.gameObject);
        _ = playerTiles.Remove(clientId);
    }

    /// <summary>
    /// Metoda RPC, która rozsyła informację o gotowości gracza do wszystkich klientów i hosta.
    /// </summary>
    /// <param name="ready">Zmienna logiczna informująca, czy gracz jest gotowy.</param>
    /// <param name="clientId">Identyfikator gracza, którego status gotowości jest akutalizowany.</param>
    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastPlayerReadySetRpc(bool ready, ulong clientId)
    {
        SetPlayerReady(ready, clientId);
    }

    /// <summary>
    /// Metoda ustawiająca stan gotowości gracza i aktualizująca wygląd w UI. Ustawia kolor w interfejsie użytkownika na podstawie stanu gotowości gracza.
    /// </summary>
    /// <param name="ready"></param>
    /// <param name="clientId"></param>
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

    /// <summary>
    /// Metoda wywoływana, gdy gracz zmienia swój stan gotowości.
    /// </summary>
    public void OnPlayerReadySwitch()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        BroadcastPlayerReadySetRpc(!playerTiles[localClientId].ready, NetworkManager.Singleton.LocalClientId);
    }
}