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
    { get { return this.money; } set { this.money = value; } }

    public int CompareTo(LeaderboardTeam other)
    {
        if (this.Money < other.Money)
            return 1;
        else if (this.Money > other.Money)
            return -1;
        else return 0;
    }
}
