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

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public NetworkVariable<Question> Question { get; } = new();
    public NetworkVariable<Category> Category { get; } = new();
    public NetworkVariable<ulong> Winner { get; } = new();
    public NetworkVariable<int> CurrentBid { get; } = new();
    public NetworkVariable<int> startingTeamCount { get; } = new();

    [Rpc(SendTo.ClientsAndHost)]
    public void StartGameRpc()
    {
        CurrentBid.Value = 0;

        if (NetworkManager.Singleton.IsHost)
        {
            _ = NetworkManager.SceneManager.LoadScene("Wheel", LoadSceneMode.Single);
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
