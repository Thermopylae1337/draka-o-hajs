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

/// <summary>
/// Główna klasa menedżera gry, odpowiedzialna za zarządzanie rozgrywką i synchronizację stanu gry między klientami.
/// </summary>
public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// Instancja singletona klasy GameManager.
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// Właściwość zapewniająca dostęp do instancji klasy GameManager.
    /// </summary>
    public static GameManager Instance => _instance;

    /// <summary>
    /// Zmienna sieciowa przechowująca aktualne pytanie.
    /// </summary>
    public NetworkVariable<Question> Question { get; } = new();

    /// <summary>
    /// Zmienna sieciowa przechowująca aktualną kategorię.
    /// </summary>
    public NetworkVariable<Category> Category { get; } = new();

    /// <summary>
    /// Zmienna sieciowa przechowująca identyfikator zwycięzcy (ID gracza).
    /// </summary>
    public NetworkVariable<ulong> Winner { get; } = new();

    /// <summary>
    /// Zmienna sieciowa przechowująca aktualną stawkę w grze.
    /// </summary>
    public NetworkVariable<int> CurrentBid { get; } = new();
    /// <summary>
    /// Zmienna sieciowa przechowująca ilość drużyn biorących udział w grze.
    /// </summary>
    public NetworkVariable<int> StartingTeamCount { get; } = new();

    /// <summary>
    /// RPC uruchamiający grę. Wywoływany na kliencie i serwerze.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    public void StartGameRpc()
    {
        CurrentBid.Value = 0;

        if (NetworkManager.Singleton.IsHost)
        {
            _ = NetworkManager.SceneManager.LoadScene("Wheel", LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Metoda wywoływana podczas inicjalizacji obiektu.
    /// Ustawia singletona i zapobiega tworzeniu wielu instancji GameManager.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
