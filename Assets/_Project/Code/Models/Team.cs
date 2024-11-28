using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Team : MonoBehaviour, INetworkSerializable, IEquatable<Team>
{
    [SerializeField]
    private int money = Utils.START_MONEY;
    [SerializeField]
    private int clues = 0;
    [SerializeField]
    private int cluesUsed = 0;
    [SerializeField]
    private int blackBoxes = 0;
    [SerializeField]
    private int inactiveRounds = 0; //licznik rund bierności w licytacji
    [SerializeField]
    private List<string> powerUps = new(); //deprecated?
    [SerializeField]
    private List<string> badges = new();
    [SerializeField]
    private string teamName = Utils.TEAM_DEFAULT_NAME;
    private int bid = 0;
    [SerializeField]
    private string colour;

    //gettery i settery
    public string TeamName { get => teamName; set => teamName = value; }

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

    public int Bid
    {
        get => bid;
        set => bid = value;
    }
    public string Colour
    {
        get => colour;
    }

    public int Clues
    {
        get => clues;
        set
        {
            if (value < 0)
            {
                throw new Exception("Dostępnych wskazówek nie może być mniej niż 0.");
            }

            clues = value;
        }
    }

    public int CluesUsed
    {
        get => cluesUsed;
        set
        {
            if (value < 0)
            {
                throw new Exception("Zużyte wskazówki nie mogą być na minusie.");
            }

            cluesUsed = value;
        }
    }
    public int BlackBoxes
    {
        get => blackBoxes;
        set
        {
            if (value < 0)
            {
                throw new Exception("Czarne Skrzynki nie mogą być na minusie.");
            }

            blackBoxes = value;
        }
    }
    public int InactiveRounds
    {
        get => inactiveRounds;
        set
        {
            if (value < 0)
            {
                throw new Exception("Rundy bierności w licytacji nie mogą być na minusie.");
            }

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
    public void RaiseBid(int amount)
    {
        if (money > amount)
        {
            money -= amount;
            bid += amount;
        }
    }
    public void ResetBid()
    {
        //dodałem funkcję reset bid żeby można było np zrobić odznakę "zakończ licytację z jakąśtam kwotą na końcu"
        bid = 0;
    }
    public void Serialize(string path)
    {
        string jsonString = JsonUtility.ToJson(this);
        File.WriteAllText(path, jsonString);
    }

    public static Team Deserialize(string path)
    {
        string jsonFromFile = File.ReadAllText(path);
        return JsonUtility.FromJson<Team>(jsonFromFile);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref teamName);
        serializer.SerializeValue(ref money);
        serializer.SerializeValue(ref cluesUsed);
        serializer.SerializeValue(ref inactiveRounds);

        _ = Utils.NetworkSerializeList(serializer, powerUps);
        _ = Utils.NetworkSerializeList(serializer, badges);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public bool Equals(Team team) => money == team.money && clues == team.clues && cluesUsed == team.cluesUsed && blackBoxes == team.blackBoxes && inactiveRounds == team.inactiveRounds && EqualityComparer<List<string>>.Default.Equals(powerUps, team.powerUps) && EqualityComparer<List<string>>.Default.Equals(badges, team.badges) && teamName == team.teamName && bid == team.bid && colour == team.colour;
}