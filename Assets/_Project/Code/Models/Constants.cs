using System;
using UnityEngine;

public static class Constants
{
    public readonly static int START_MONEY = 10000;
    private static Team currentTeam;
    public static Team CurrentTeam { get => currentTeam; }

    public static void LoadTeamFromDisk()
    {
        // Load team from disk
        if (currentTeam == null)
            try
            {

                currentTeam = Team.Deserialize("team.json");
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error loading team from disk: " + e.Message);
                Debug.LogWarning("Creating new team");
                currentTeam = new Team();
            }
        else
            throw new Exception("Team already loaded");
    }

    public static void SaveTeamToDisk()
    {
        // Save team to disk
        if (currentTeam != null)
            currentTeam.Serialize("team.json");
        else
            throw new Exception("Team not loaded");
    }
}