using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
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

    public static Dictionary<Y, Z> NetworkSerializeDictionary<T, Y, Z>(BufferSerializer<T> serializer, Dictionary<Y, Z> dict) where T : IReaderWriter
    {
        string dictSerialized = serializer.IsReader ? "" : JsonConvert.SerializeObject(dict);

        serializer.SerializeValue(ref dictSerialized);

        return serializer.IsWriter ? JsonConvert.DeserializeObject<Dictionary<Y, Z>>(dictSerialized) : null;
    }

    public static readonly string[] CATEGORY_NAMES = new string[]
    {
        "Czarna skrzynka",
        "Anatomia i medycyna",
        "Astronomia",
        "Biologia",
        "Chemia",
        "Czasy współczesne",
        "Film",
        "Filozofia i religie",
        "Fizyka",
        "Geografia",
        "Historia",
        "Język polski",
        "Kulinaria",
        "Literatura",
        "Matematyka",
        "Podpowiedź",
        "Motoryzacja",
        "Muzyka klasyczna",
        "Muzyka rozrywkowa",
        "Piłka nożna",
        "Polityka i gospodarka",
        "Popkultura",
        "Przysłowia i cytaty",
        "Rozmaitości",
        "Seriale",
        "Sport",
        "Sztuka",
        "Technologie",
        "Wędkarstwo",
    };
}