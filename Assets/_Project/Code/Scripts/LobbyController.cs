using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
    // Używam Jednego obiektu do zarządzania logiką lobby, bo lobby musi być
    // reaktywne na wiele związanych ze sobą i zdarzen i w wielu miejscach.
    public Button startButton;
    public Button readyButton;
    public GameObject playerList;
    public GameObject playerListEntryPrefab;

    private Image readyButtonImage;
    private bool currentReady = true; // Will be changed on Start to false
    private Dictionary<ulong, bool> playersReadyStatus = new Dictionary<ulong, bool>();

    Color readyColor = Color.green;
    Color notReadyColor = Color.red;


    void Start()
    {
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(SwitchReady);
        SwitchReady();
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
}
