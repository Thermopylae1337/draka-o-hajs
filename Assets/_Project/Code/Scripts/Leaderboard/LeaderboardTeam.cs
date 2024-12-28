using System;

public class LeaderboardTeam : IComparable<LeaderboardTeam>
{
    string name;
    int money;
    public LeaderboardTeam(string name, int money)
    {
        this.name = name;
        this.money = money;
    }
    public string Name
    { get { return this.name; } }
    public int Money
    { get => money; set => money = value; }

    public int CompareTo(LeaderboardTeam other)
    {
        return Money < other.Money ? 1 : Money > other.Money ? -1 : 0;
    }
}
