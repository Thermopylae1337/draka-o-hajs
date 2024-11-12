using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Linq;
using System;

[System.Serializable]
public  class LeaderboardEntry:IComparable<LeaderboardEntry>
{
    public string name;
    public int score;

    public int CompareTo(LeaderboardEntry other)
    {
        if(other == null) return 0;
        return other.score.CompareTo(this.score);
    }
}
public class Leaderboardscript : MonoBehaviour
{
   // public Text titleText;
    public List<TextMeshProUGUI> teams;
    public List<TextMeshProUGUI> scores;

    
    private string filePath;

    void Start()
    {
       
        filePath = Path.Combine(Application.dataPath, "Resources/leaderboard.json");
        try
        {
            if (File.Exists(filePath))
            {
                LoadLeaderboard();
            }
            else
            {
                ClearLeaderboard();
            }
        }
        catch (System.Exception)
        {
           // titleText.text = "Brak wyników do wczytania";
        }
       
    }
    void ClearLeaderboard()
    {
        foreach (var team in teams)
        {
            team.text = "-";

        }
        foreach (var score in scores)
        {
            score.text = "-";
        }
    }
    void LoadLeaderboard()
    {
        string jsonContent = File.ReadAllText(filePath);

        LeaderboardList leaderboardEntries = JsonUtility.FromJson<LeaderboardList>(jsonContent);

        leaderboardEntries.entries.Sort();

        for (int i = 0; i < leaderboardEntries.entries.Count; i++)
        {
            teams[i].text = leaderboardEntries.entries[i].name;
            scores[i].text = leaderboardEntries.entries[i].score.ToString();
        }
      
    }

    [System.Serializable]
    private class LeaderboardList
    {
        public List<LeaderboardEntry> entries;
    }
}
