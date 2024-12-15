using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class TempTeam : IComparable<TempTeam>
{
    public string Name;
    public int Score;
    public int CompareTo(TempTeam other)
    {
        if(other == null) 
            return 0;
        return other.Score.CompareTo(this.Score);
    }
}
public class LeaderboardScript : MonoBehaviour
{
    public GameObject RowLeaderboard;
    public Transform RowContainer;
    private string filePath;
    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "Resources/leaderboard.json");
        try
        {
            if (File.Exists(filePath))
            {
                LoadLeaderboardFromJson();
            }
            else
            {
                ClearLeaderboard();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"B³¹d podczas wczytywania leaderboard: {e.Message}");
        }
    }
    void ClearLeaderboard()
    {
          foreach(Transform child in RowContainer)
          {
            Destroy(child.gameObject);
          }
    }
    void LoadLeaderboardFromJson()
    {
        string jsonContent = File.ReadAllText(filePath);
        LeaderboardList leaderboardEntries = JsonUtility.FromJson<LeaderboardList>(jsonContent);
        leaderboardEntries.entries.Sort();
        foreach (var entry in leaderboardEntries.entries)
        {
            GameObject newRow = Instantiate(RowLeaderboard, RowContainer);
            TextMeshProUGUI[] textFields = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            textFields[0].text = entry.Name;
            textFields[1].text = entry.Score.ToString();
        }
        
    }
    [System.Serializable]
    private class LeaderboardList
    {
        public List<TempTeam> entries;
    }
}
