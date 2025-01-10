using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Klasa zarządzająca listą drużyn na tablicy wyników.
/// </summary>
public class LeaderboardList
{
    /// <summary>
    /// Lista drużyn na tablicy wyników.
    /// </summary>
    List<LeaderboardTeam> teamList;

    /// <summary>
    /// Ścieżka do pliku JSON przechowującego dane tablicy wyników.
    /// </summary>
    //string path = Path.Combine(Application.streamingAssetsPath, "teams.json");
    string path = "teams.json";
    /// <summary>
    /// Właściwość tylko do odczytu, zwracająca listę drużyn.
    /// </summary>
    public List<LeaderboardTeam> TeamList
    {
        get => this.teamList;
    }

    /// <summary>
    /// Dodaje drużynę do listy lub aktualizuje jej dane, jeśli już istnieje.
    /// </summary>
    /// <param name="team">Drużyna do dodania lub zaktualizowania.</param>
    public void AddTeam(LeaderboardTeam team)
    {
        foreach (LeaderboardTeam item in teamList)
        {
            if (item.Name.Equals(team.Name))
            {
                item.Money += team.Money;
                foreach(Badge badge in team.Badges)
                {
                    if (badge.Unlocked==true)
                    {
                        item.FindBadge(badge.Name).Unlocked = true;
                    }
                }
                return;
            }
        }
        teamList.Add(team);
    }

    /// <summary>
    /// Wyszukuje drużynę na podstawie nazwy.
    /// </summary>
    /// <param name="teamName">Nazwa drużyny do wyszukania.</param>
    /// <returns>Zwraca drużynę, jeśli istnieje w liście.</returns>
    /// <exception cref="Exception">Wyrzucany, gdy drużyna o podanej nazwie nie istnieje.</exception>
    public LeaderboardTeam FindTeam(string teamName)
    {
        foreach (LeaderboardTeam team in teamList)
        {
            if (team.Name.Equals(teamName))
            {
                return team;
            }
        }

        throw new Exception("Team does not exist");
    }

    /// <summary>
    /// Serializuje listę drużyn i zapisuje ją do pliku JSON.
    /// </summary>
    public void Serializuj()
    {
        //StreamWriter sw = new(path);
        //sw.Write(JsonConvert.SerializeObject(teamList));
        //sw.Close();
        File.WriteAllText(path,JsonConvert.SerializeObject(teamList));
    }

    /// <summary>
    /// Deserializuje dane z pliku JSON i ładuje listę drużyn.
    /// </summary>
    public void Deserializuj()
    {
        string json = File.ReadAllText(path);
        teamList = json.Equals("") || json.Equals(null)
            ? new List<LeaderboardTeam>()
            : JsonConvert.DeserializeObject<List<LeaderboardTeam>>(json);
    }
}
