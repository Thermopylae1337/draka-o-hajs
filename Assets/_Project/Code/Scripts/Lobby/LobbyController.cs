using Newtonsoft.Json;
using System;
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
    public Button biddingWarButton; //TODO: delete after testing
    public GameObject playerListGameObject;
    public GameObject playerListEntryPrefab;
    private Image readyButtonImage;
    private bool selfReady = false;
    private readonly Dictionary<string, (bool ready, Transform tile)> playerTiles = new();  // For each user i will store if he is ready and his text on playerListGameObject
    private Color readyColor = Color.green;
    private Color notReadyColor = Color.red;

    [Rpc(SendTo.Everyone)]
    private void LoadBWHostRpc()
    {
        _ = NetworkManager.SceneManager.LoadScene("BiddingWar", LoadSceneMode.Single);
    }

    private void Start()
    {
        //to delete after testing start
        biddingWarButton.onClick.AddListener(LoadBWHostRpc);

        //to delete after testing  end
        readyButtonImage = readyButton.GetComponent<Image>();
        readyButton.onClick.AddListener(OnPlayerReadySwitch);
        startButton.onClick.AddListener(StartGame);
        startButton.interactable = false;

        GameController.Instance.Teams.OnValueChanged += (_, current) =>
            {
                Debug.Log("Teams changed");

                if (!GameController.Instance.Teams.Value.Contains(Utils.CurrentTeam))
                {
                    GameController.Instance.Teams.Value.Add(Utils.CurrentTeam);
                    GameController.Instance.Teams.Value = GameController.Instance.Teams.Value; // Trigger update
                    return;
                }

                RefreshPlayerList();
            };
        Debug.Log("Teams changed");

        TeamListModel value = GameController.Instance.Teams.Value;
        GameController.Instance.Teams.Value = value; // Trigger update
        RefreshPlayerList();
    }

    private void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("Wheel", LoadSceneMode.Single);
    }

    private void RefreshPlayerList()
    {
        Debug.Log("Refreshing player list");

        foreach (Team team in GameController.Instance.Teams.Value)
        {
            (bool ready, Transform tile) currentTeamTile;
            if (!playerTiles.ContainsKey(team.Name))
            {
                Transform textTransform = Instantiate(playerListEntryPrefab, playerListGameObject.transform).transform;
                currentTeamTile = (false, textTransform);
                playerTiles.Add(team.Name, currentTeamTile);
                textTransform.GetComponent<TextMeshProUGUI>().text = team.Name;
            }
            else
            {
                currentTeamTile = playerTiles[team.Name];
            }

            currentTeamTile.tile.GetComponent<TextMeshProUGUI>().color = currentTeamTile.ready ? readyColor : notReadyColor;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastPlayerReadySetRpc(bool ready, Team team)
    {
        playerTiles[team.Name] = (ready, playerTiles[team.Name].tile);
        RefreshPlayerList();

        if (NetworkManager.Singleton.IsHost)
        {
            startButton.interactable = playerTiles.All(pair => pair.Value.ready) && playerTiles.Count > 1;
        }
    }

    public void OnPlayerReadySwitch()
    {
        selfReady = !selfReady;
        readyButtonImage.color = selfReady ? readyColor : notReadyColor;
        BroadcastPlayerReadySetRpc(selfReady, Utils.CurrentTeam);
    }
}