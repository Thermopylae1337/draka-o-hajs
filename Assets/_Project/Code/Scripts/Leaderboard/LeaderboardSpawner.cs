using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa odpowiedzialna za generowanie i wyświetlanie tablicy wyników w interfejsie użytkownika.
/// </summary>
public class LeaderboardSpawner : MonoBehaviour
{
    /// <summary>
    /// Prefab reprezentujący element na tablicy wyników.
    /// </summary>
    public GameObject leaderboardPrefab;

    /// <summary>
    /// Rodzic w hierarchii, w którym będą umieszczane elementy tablicy wyników.
    /// </summary>
    public Transform contentParent;

    /// <summary>
    /// Metoda wywoływana podczas uruchomienia obiektu. Generuje tablicę wyników.
    /// </summary>
    private void Start()
    {
        GenerateLeaderboard();
    }

    /// <summary>
    /// Generuje tablicę wyników na podstawie danych z listy drużyn.
    /// </summary>
    private void GenerateLeaderboard()
    {
        LeaderboardList leaderboard = new();
        leaderboard.Deserializuj();
        List<LeaderboardTeam> teams = leaderboard.TeamList;
        teams.Sort();

        if (teams != null && teams.Count > 0)
        {
            int miejsce = 1;
            foreach (LeaderboardTeam item in teams)
            {
                GameObject leaderboardObject = Instantiate(leaderboardPrefab, contentParent);
                TextMeshProUGUI name = leaderboardObject.transform.Find("name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI money = leaderboardObject.transform.Find("money").GetComponent<TextMeshProUGUI>();
                name.text = miejsce + ". " + item.Name;
                money.text = "" + item.Money;
                money.color = item.Money > 0 ? new Color(0.19f, 0.8f, 0.19f) : new Color(1f, 0f, 0f);
                Image background = leaderboardObject.GetComponent<Image>();
                background.color = miejsce == 1
                    ? new Color(1f, 0.84f, 0f)
                    : miejsce == 2 ? new Color(0.75f, 0.75f, 0.75f) : miejsce == 3 ? new Color(0.72f, 0.45f, 0.2f) : new Color(1f, 1f, 1f, 0.1f);
                miejsce++;
            }
        }
        else
        {
            GameObject leaderboardObject = Instantiate(leaderboardPrefab, contentParent);
            TextMeshProUGUI name = leaderboardObject.transform.Find("name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI money = leaderboardObject.transform.Find("money").GetComponent<TextMeshProUGUI>();
            name.text = "Brak drużyn do wyświetlenia";
            money.text = "";
        }
    }
}
