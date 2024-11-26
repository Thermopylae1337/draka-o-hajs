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

    [SerializeField] private NetworkVariable<TeamListModel> _teams = new();
    // [SerializeField] private NetworkVariable<Question> _question = new();
    // [SerializeField] private NetworkVariable<Category> _category = new();

    public NetworkVariable<TeamListModel> Teams => _teams;
    // public NetworkVariable<Question> Question { get => _question; }
    // public NetworkVariable<Category> Category { get => _category; }

    public void StartHost() => _ = NetworkManager.Singleton.StartHost();

    public void StartClient() => _ = NetworkManager.Singleton.StartClient();

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            _teams.Value = new TeamListModel() { Utils.CurrentTeam };
            _ = NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        // else
        // {
        //     _teams.Value.Add(Utils.CurrentTeam);
        //     _teams.Value = _teams.Value;
        //     SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        // }
    }

    [Rpc(SendTo.Everyone)]
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
