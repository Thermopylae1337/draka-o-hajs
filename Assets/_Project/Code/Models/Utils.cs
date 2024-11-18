using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public static class Utils
{
    public const int START_MONEY = 10000;
    public const string TEAM_DEFAULT_NAME = "New Team";
    public const int ROUNDS_LIMIT = 7;


    // Serialization helpers
    public static List<Y> NetworkSerializeList<T, Y>(BufferSerializer<T> serializer, List<Y> list) where T : IReaderWriter
    {
        string listSerialized = serializer.IsReader ? "" : JsonConvert.SerializeObject(list);

        serializer.SerializeValue(ref listSerialized);

        if (serializer.IsWriter)
        {
            return JsonConvert.DeserializeObject<List<Y>>(listSerialized);
        }

        return null;
    }

    //Bidding management
    public static void awardBiddingWinners(Team team, String categoryName)
    {
        if (categoryName.Equals("Czarna Skrzynka"))
        {
            team.BlackBoxes++;
        }
        else if (categoryName.Equals("Podpowiedz"))
        {
            team.Clues++;
        }
        else //wylosowano kategorie pytaniowa
        {
            //todo tutaj wywolac metode rozpoczynajaca etap pytania
        }
    }


    // Team management
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