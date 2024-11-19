using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using UnityEngine;


public class Team
{
    //zadeklarowanie nazwa zmiennych
    private string name;
    private int money = Constants.START_MONEY;
    private int cluesUsed;
    private int inactiveRounds = 0; //licznik rund bierności w licytacji
    private int totalMoney;
    private List<string> powerUps;
    private List<string> badges;

    //konstruktor klasy
    public Team(string name, int cluesUsed, int TotalMoney, List<string> powerUps, List<string> badges)
    {
        this.name = name;
        this.money = Constants.START_MONEY;
        this.cluesUsed = cluesUsed;
        this.inactiveRounds = 0;
        this.totalMoney = TotalMoney;
        this.powerUps = powerUps;
        this.badges = badges;
    }

    //gettery i settery
    public string Name => name;
    public int Money
    {
        get { return money; }
        set
        {
            if (value < 0)
            {
                money = 0;
                Debug.LogWarning("Pieniądze zeszły poniżej zera, zeruje.");
                return;
            }
            money = value;
        }
    }
    public int CluesUsed
    {
        get { return cluesUsed; }
        set
        {
            if (value < 0)
                throw new Exception("Zużyte wskazówki nie mogą być na minusie.");

            cluesUsed = value;
        }
    }
    public int InactiveRounds
    {
        get { return inactiveRounds; }
        set
        {
            if (value < 0)
                throw new Exception("Rundy bierności w licytacji nie mogą być na minusie.");

            inactiveRounds = value;
        }
    }
    public int TotalMoney
    {
        get { return totalMoney; }
        set
        {
            totalMoney = value;
        }
    }
    public ReadOnlyCollection<string> PowerUps
    {
        get { return powerUps.AsReadOnly(); }
        set
        {
            powerUps = value.ToList();
        }
    }
    public ReadOnlyCollection<string> Badges
    {
        get { return badges.AsReadOnly(); }
        set
        {
            badges = value.ToList();
        }
    }

    //serializacja
    public void Serialize(string path)
    {
        string jsonString = JsonUtility.ToJson(this);
        File.WriteAllText(path, jsonString);
    }

    //deserializacja
    static public Team Deserialize(string path)
    {
        string jsonFromFile = File.ReadAllText(path);
        return JsonUtility.FromJson<Team>(jsonFromFile);
    }

}