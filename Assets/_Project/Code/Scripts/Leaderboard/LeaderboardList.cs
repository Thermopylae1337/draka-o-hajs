using Newtonsoft.Json;
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
    public void Serializuj()
    {
        StreamWriter sw = new StreamWriter(path);
        sw.Write(JsonConvert.SerializeObject(teamList));
        sw.Close();
    }
    public void Deserializuj()
    {
        string json = File.ReadAllText(path);
        if (json.Equals("") || json.Equals(null))
        {
            teamList = new List<LeaderboardTeam>();
        }
        else
        {
            teamList = JsonConvert.DeserializeObject<List<LeaderboardTeam>>(json);
        }
    }
}
