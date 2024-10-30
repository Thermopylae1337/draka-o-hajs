using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
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

    private Image readyButtonImage;
    private bool selfReady = false;
    private readonly Dictionary<ulong, (bool, Transform)> playerList = new();  // For each user i will store if he is ready and his text on playerListGameObject

    Color readyColor = Color.green;
    Color notReadyColor = Color.red;

    void Start()
    {
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);

        startButton.interactable = false;
        OnSelfJoin();
    }

    public void OnSelfJoin()
    {
        selfReady = false;


        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AddPlayerToList(clientId);
        }

        BroadcastPlayerJoinedRpc(NetworkManager.Singleton.LocalClientId);
        BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId);

        RequestReadyBroadcastRpc();
    }


    [Rpc(SendTo.NotMe)]
    void RequestReadyBroadcastRpc()
    {
        BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.NotMe)]
    void BroadcastPlayerJoinedRpc(ulong clientId)
    {
        AddPlayerToList(clientId);
    }

    private void AddPlayerToList(ulong clientId)
    {
        var playerListTile = Instantiate(playerListEntryPrefab, playerListGameObject.transform);
        playerListTile.name = $"PlayerListTile_{clientId}";
        playerListTile.GetComponent<TMP_Text>().text = $"Player_{clientId}";
        playerList[clientId] = (false, playerListTile.transform);
    }

    public void OnPlayerReadySwitch()
    {
        selfReady = !selfReady;
        readyButtonImage.color = selfReady ? readyColor : notReadyColor;
        BroadcastPlayerReadySetRpc(selfReady, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastPlayerReadySetRpc(bool ready, ulong clientId)
    {
        playerList[clientId] = (ready, playerList[clientId].Item2);

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
