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
    public GameManager Instance => _instance;
    public NetworkVariable<Question> Question { get; } = new();
    public NetworkVariable<Category> Category { get; } = new();

    [Rpc(SendTo.ClientsAndHost)]
    public void StartGameRpc()
    {
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
