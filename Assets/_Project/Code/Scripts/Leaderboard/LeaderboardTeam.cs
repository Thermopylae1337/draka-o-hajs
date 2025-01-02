using System;

/// <summary>
/// Klasa reprezentująca drużynę na tablicy wyników.
/// </summary>
public class LeaderboardTeam : IComparable<LeaderboardTeam>
{
    /// <summary>
    /// Nazwa drużyny.
    /// </summary>
    string name;

    /// <summary>
    /// Ilość pieniędzy posiadanych przez drużynę.
    /// </summary>
    int money;

    /// <summary>
    /// Konstruktor klasy LeaderboardTeam.
    /// </summary>
    /// <param name="name">Nazwa drużyny.</param>
    /// <param name="money">Ilość pieniędzy posiadanych przez drużynę.</param>
    public LeaderboardTeam(string name, int money)
    {
        this.name = name;
        this.money = money;
    }

    /// <summary>
    /// Właściwość tylko do odczytu, zwraca nazwę drużyny.
    /// </summary>
    public string Name
    {
        get { return this.name; }
    }

    /// <summary>
    /// Właściwość do odczytu i zapisu, reprezentuje ilość pieniędzy drużyny.
    /// </summary>
    public int Money
    {
        get => money;
        set => money = value;
    }

    /// <summary>
    /// Metoda porównująca bieżącą drużynę z inną na podstawie ilości pieniędzy.
    /// </summary>
    /// <param name="other">Inna drużyna do porównania.</param>
    /// <returns>
    /// Wartość 1, jeśli bieżąca drużyna ma mniej pieniędzy niż inna;
    /// -1, jeśli ma więcej; 0, jeśli mają taką samą ilość pieniędzy.
    /// </returns>
    public int CompareTo(LeaderboardTeam other)
    {
        return Money < other.Money ? 1 : Money > other.Money ? -1 : 0;
    }
}