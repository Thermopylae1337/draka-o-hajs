using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public static class Utils
{
    public const int QUESTIONS_AMOUNT = 8;
    public const int START_MONEY = 10000;
    public const string TEAM_DEFAULT_NAME = "New Team";
    public const int ROUNDS_LIMIT = 7;

    // Serialization helpers
    public static List<Y> NetworkSerializeList<T, Y>(BufferSerializer<T> serializer, List<Y> list) where T : IReaderWriter
    {
        string listSerialized = serializer.IsReader ? "" : JsonConvert.SerializeObject(list);

        serializer.SerializeValue(ref listSerialized);

        return serializer.IsWriter ? JsonConvert.DeserializeObject<List<Y>>(listSerialized) : null;
    }

    public static Team CurrentTeam { get; private set; }

    public static void LoadTeamFromDisk()
    {
        // Load team from disk
        if (CurrentTeam == null)
        {
            try
            {
                CurrentTeam = Team.Deserialize("team.json");
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error loading team from disk: " + e.Message);
                Debug.LogWarning("Creating new team");
                CurrentTeam = new Team();
            }
        }
        else
        {
            throw new Exception("Team already loaded");
        }
    }

    public static void SaveTeamToDisk()
    {
        // Save team to disk
        if (CurrentTeam != null)
        {
            CurrentTeam.Serialize("team.json");
        }
        else
        {
            throw new Exception("Team not loaded");
        }
    }
}