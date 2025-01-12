using NUnit.Framework;
using System;
using System.Collections.Generic;

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
    /// Odznaki druzyny.
    /// </summary>
    List<Badge> badges;

    /// <summary>
    /// Konstruktor klasy LeaderboardTeam.
    /// </summary>
    /// <param name="name">Nazwa drużyny.</param>
    /// <param name="money">Ilość pieniędzy posiadanych przez drużynę.</param>
    /// <param name="badges">Odznaki drużyny.</param>
    public LeaderboardTeam(string name, int money, List<Badge> badges)
    {
        this.name = name;
        this.money = money;
        this.badges = badges;
    }
    /// <summary>
    /// Wyszukuje odznake z odznak danej drużyny na podstawie nazwy.
    /// </summary>
    /// <param name="name">Nazwa odznaki do wyszukania.</param>
    /// <returns>Zwraca odznake, jeśli istnieje w liście.</returns>
    /// <exception cref="Exception">Wyrzucany, gdy odznaka o podanej nazwie nie istnieje.</exception>
    public Badge FindBadge(string name)
    {
        foreach (Badge badge in badges)
        {
            if(badge.Name.Equals(name))
            {
                return badge;
            }
        }
        throw new Exception("Badge does not exist");
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
    /// Właściwość do odczytu i zapisu, reprezentuje odznaki drużyny.
    /// </summary>
    public List<Badge> Badges
    {
        get => badges;
        set => badges = value;
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