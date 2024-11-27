using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Text;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using System;

public class GameController : NetworkBehaviour
{
#pragma warning disable IDE1006 // Naming Styles
    public static GameController Instance;
#pragma warning restore IDE1006 // Naming Styles

    // [SerializeField] private NetworkVariable<Question> _question = new();
    // [SerializeField] private NetworkVariable<Category> _category = new();

    public readonly NetworkVariable<List<Team>> teams = new(new());

    // public NetworkVariable<Question> Question { get => _question; }
    // public NetworkVariable<Category> Category { get => _category; }

    public void StartHost() => _ = NetworkManager.Singleton.StartHost();

    public void StartClient() => _ = NetworkManager.Singleton.StartClient();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnectedRpc;

            teams.Value = new()
            {
                Utils.CurrentTeam
            };

            bool asdf = teams.CheckDirtyState();
            _ = NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void OnClientConnectedRpc(ulong clientId) => AddTeamToServerRpc(Utils.CurrentTeam);

    [Rpc(SendTo.Everyone)]
    private void AddTeamToServerRpc(Team currentTeam)
    {
        if (IsHost && teams.Value.All(team => team.Name != currentTeam.Name))
        {
            teams.Value.Add(currentTeam);
            _ = teams.CheckDirtyState();
        }
    }

    [Rpc(SendTo.Server)]
    public void StartGameRpc()
    {
        _ = NetworkManager.SceneManager.LoadScene("Wheel", LoadSceneMode.Single);
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
