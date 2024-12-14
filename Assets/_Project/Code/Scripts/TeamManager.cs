using Assets._Project.Code.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<int> money = new(Utils.START_MONEY, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

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
    public NetworkVariable<FixedString64Bytes> teamName = new(Utils.TEAM_DEFAULT_NAME, writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField]
    public string TeamName => teamName.Value.ToString();

    private NetworkVariable<int> bid = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField]
    private ColourEnum colour;

    //gettery i settery
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public int Money
    {
        get => money.Value;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("Pieniądze zeszły poniżej zera, zeruje.");
                money.Value = 0;
                return;
            }

            money.Value = value;
        }
    }

    public int Bid
    {
        get => bid.Value;
        set => bid.Value = value;
    }
    public ColourEnum Colour
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
        if (money.Value >= amount)
        {
            money.Value -= amount;
            bid.Value += amount;
        }
    }
    public void ResetBid()
    {
        //dodałem funkcję reset bid żeby można było np zrobić odznakę "zakończ licytację z jakąśtam kwotą na końcu"
        bid.Value = 0;
    }
    public void Serialize(string path)
    {
        string jsonString = JsonUtility.ToJson(this);
        File.WriteAllText(path, jsonString);
    }

    public static TeamManager Deserialize(string path)
    {
        string jsonFromFile = File.ReadAllText(path);
        return JsonUtility.FromJson<TeamManager>(jsonFromFile);
    }
}