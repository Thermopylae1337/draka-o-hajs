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
    public GameObject playerListGameObject;
    public GameObject playerListEntryPrefab;

    public Button questionButton;
    private Image readyButtonImage;
    private bool selfReady = false;
    private readonly Dictionary<ulong, (bool, Transform, Team)> playerList = new();  // For each user i will store if he is ready and his text on playerListGameObject

    private Color readyColor = Color.green;
    private Color notReadyColor = Color.red;

    private void Start()
    {
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);

        startButton.interactable = false;
        questionButton.onClick.AddListener(OnStartGame);
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
    public void OnStartGame()
    {
        if (IsHost)
        {
            LoadGameSceneRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void LoadGameSceneRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("QuestionStage", LoadSceneMode.Single);
    }
}
