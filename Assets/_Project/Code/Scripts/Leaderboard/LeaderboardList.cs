using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeaderboardList
{
    List<LeaderboardTeam> teamList;
    string path = Path.Combine(Application.streamingAssetsPath, "leaderboard.json");
    public List<LeaderboardTeam> TeamList
        {
        get => this.teamList;
        }
    public void AddTeam(LeaderboardTeam team)
    {
        foreach (LeaderboardTeam item in teamList)
        {
            if (item.Name.Equals(team.Name))
            {
                item.Money += team.Money;
                return;
            }
        }
        teamList.Add(team);
    }
    public LeaderboardTeam FindTeam(string teamName)
    {
        foreach(LeaderboardTeam team in teamList)
        {
            if(team.Name.Equals(teamName))
            {
                return team;
            }
        }

        throw new Exception("Team does not exist");
    }
    public void Serializuj()
    {
        StreamWriter sw = new(path);
        sw.Write(JsonConvert.SerializeObject(teamList));
        sw.Close();
    }
    public void Deserializuj()
    {
        string json = File.ReadAllText(path);
        teamList = json.Equals("") || json.Equals(null)
            ? new List<LeaderboardTeam>()
            : JsonConvert.DeserializeObject<List<LeaderboardTeam>>(json);
    }
}
