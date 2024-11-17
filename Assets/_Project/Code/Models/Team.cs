using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public class Team : INetworkSerializable
{
    private int money = Utils.START_MONEY;
    private int clues = 0;
    private int cluesUsed = 0;
    private int blackBoxes = 0;
    private int inactiveRounds = 0; //licznik rund bierności w licytacji
    private List<string> powerUps = new(); //deprecated?
    private List<string> badges = new();
    private string name;

    public Team() : this(Utils.TEAM_DEFAULT_NAME)
    {
    }

    public Team(string name = "New Team")
    {
        Name = name;
    }

    public Team(string name, int money, int clues, int cluesUsed, int blackBoxes, int inactiveRounds, List<string> powerUps, List<string> badges) : this(name)
    {
        Money = money;
        Clues = clues;
        CluesUsed = cluesUsed;
        BlackBoxes = blackBoxes;
        InactiveRounds = inactiveRounds;
        this.badges = badges;
        this.powerUps = powerUps;
    }

    //gettery i settery
    public string Name { get => name; set => name = value; }

    public int Money
    {
        get => money;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("Pieniądze zeszły poniżej zera, zeruje.");
                money = 0;
                return;
            }
            money = value;
        }
    }
    public int Clues
    {
        get => clues;
        set
        {
            if (value < 0)
                throw new Exception("Dostępnych wskazówek nie może być mniej niż 0.");

            clues = value;
        }
    }

    public int CluesUsed
    {
        get => cluesUsed;
        set
        {
            if (value < 0)
                throw new Exception("Zużyte wskazówki nie mogą być na minusie.");

            cluesUsed = value;
        }
    }
    public int BlackBoxes
    {
        get => blackBoxes;
        set
        {
            if (value < 0)
                throw new Exception("Czarne Skrzynki nie mogą być na minusie.");

            blackBoxes = value;
        }
    }
    public int InactiveRounds
    {
        get => inactiveRounds;
        set
        {
            if (value < 0)
                throw new Exception("Rundy bierności w licytacji nie mogą być na minusie.");

            inactiveRounds = value;
        }
    }
    public int TotalMoney { get; set; }
    public ReadOnlyCollection<string> PowerUps
    {
        get => powerUps.AsReadOnly();
        set => powerUps = value.ToList();
    }
    public ReadOnlyCollection<string> Badges
    {
        get => badges.AsReadOnly();
        set => badges = value.ToList();
    }

    public void Serialize(string path)
    {
        string jsonString = JsonUtility.ToJson(this);
        File.WriteAllText(path, jsonString);
    }

    static public Team Deserialize(string path)
    {
        string jsonFromFile = File.ReadAllText(path);
        return JsonUtility.FromJson<Team>(jsonFromFile);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref money);
        serializer.SerializeValue(ref cluesUsed);
        serializer.SerializeValue(ref inactiveRounds);

        Utils.NetworkSerializeList(serializer, powerUps);
        Utils.NetworkSerializeList(serializer, badges);
    }
}