using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Text.Json.Serialization;
using UnityEngine;

public class Team
{
    //zadeklarowanie nazwa zmiennych
    private string name;
    private int money = 10000;
    private int cluesUsed;
    private int inactiveRounds = 0; //licznik rund bierności w licytacji
    private int totalMoney;
    private List<string> powerUps;
    private List<string> badges;

    //konstruktor klasy
    public Team(string name, int cluesUsed, int TotalMoney, List<string> powerUps, List<string> badges)
    {
        this.name = name;
        this.money = 10000;
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
            if (value >= 0)
            {
                money = value;
            }
            else
            {
                throw new Exception("Pieniądze nie mogą być na minusie.");
            }
        }
    }
    public int CluesUsed
    {
        get { return cluesUsed; }
        set
        {
            cluesUsed = value;
        }
    }
    public int InactiveRounds
    {
        get { return inactiveRounds; }
        set
        {
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
    public void Serialize(Team team)
    {
        string jsonString = JsonConvert.SerializeObject(team);
        File.WriteAllText("team.json", jsonString);
    }

    //deserializacja
    public void Deserialize(Team team)
    {
        string jsonFromFile = File.ReadAllText("team.json");
        Team deserializedTeam = JsonConvert.DeserializeObject<Team>(jsonFromFile);
        Console.WriteLine("Name: {0}, money: {1}, cluesUsed: {2}, inactiveRounds: {3}, totalMoney: {4}, powerUps: {5}, badges: {6}", deserializedTeam.name, deserializedTeam.cluesUsed, deserializedTeam.inactiveRounds, deserializedTeam.totalMoney, deserializedTeam.powerUps, deserializedTeam.badges);
    }

}