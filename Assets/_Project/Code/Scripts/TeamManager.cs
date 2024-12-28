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
    private NetworkVariable<int> clues = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField]
    private NetworkVariable<int> cluesUsed = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField]
    private int blackBoxes = 0;

    private int wonBid = 0;

    private int questionsAnswered = 0;

    private int questionsAsked = 0;

    private int vaBanque = 0;

    [SerializeField]
    private NetworkVariable<int> inactiveRounds = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone); //licznik rund bierności w licytacji

    private BadgeList badgeList = new();

    [SerializeField]
    public NetworkVariable<FixedString64Bytes> teamName = new(Utils.TEAM_DEFAULT_NAME, writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField]
    public string TeamName => teamName.Value.ToString();

    private NetworkVariable<int> bid = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField]
    private ColourEnum colour;

    [SerializeField]
    private NetworkVariable<bool> inGame = new(true, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    [SerializeField]
    private NetworkVariable<uint> teamId = new(0, writePerm: NetworkVariableWritePermission.Owner, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField]
    private NetworkVariable<uint> networkId = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public uint NetworkId
    {
        get => networkId.Value;
        set => networkId.Value = value;
    }

    public uint TeamId
    {
        get => teamId.Value;
        set => teamId.Value = value;
    }
    public bool InGame
    {
        get => inGame.Value;
        set => inGame.Value = value;
    }

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
        set => colour = value;
    }

    public int Clues
    {
        get => clues.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Dostępnych wskazówek nie może być mniej niż 0.");
            }

            clues.Value = value;
        }
    }

    public int CluesUsed
    {
        get => cluesUsed.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Zużyte wskazówki nie mogą być na minusie.");
            }

            cluesUsed.Value = value;
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
    public int WonBid
    {
        get => wonBid;
        set => wonBid = value >= 0 ? value : 0;
    }

    public int QuestionsAnswered
    {
        get => questionsAnswered;
        set => questionsAnswered = value >= 0 ? value : 0;
    }

    public int QuestionsAsked
    {
        get => questionsAsked;
        set => questionsAsked = value >= 0 ? value : 0;
    }

    public int VaBanque
    {
        get => vaBanque;
        set => vaBanque = value >= 0 ? value : 0;
    }
    public int InactiveRounds
    {
        get => inactiveRounds.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Rundy bierności w licytacji nie mogą być na minusie.");
            }

            inactiveRounds.Value = value;
        }
    }

    public int TotalMoney { get; set; }
    public BadgeList BadgeList
    {
        get => badgeList;
        set => badgeList = value;
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