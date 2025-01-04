using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class BadgeList
{
    [JsonProperty("badges")]
    private readonly List<Badge> badges;

    public BadgeList()
    {
        badges = new List<Badge>
        {
            new("Mistrzowie Aukcji", "Za wygranie przynajmniej 5 licytacji"),
            new("As opowiedzi", "Dla drużyny która ani razu się nie pomyliła przy opowiedzi"),
            new("Królowie skarbca", "Za przekroczenie ustalonego limitu pieniędzy na koniec gry czyli 19 000 zł."),
            new("Ryzykanci", "Za odważne przebicie zakładu przeciwników grając 3 razy va banque w trakcie całej rozgrywki."),
            new("Czas to pieniądz", "Za poprawna opowiedz 27 sekund przed czasem"),
            new("Czarni łowcy", "za wylicytyowanie min 2 czarnych skrzynek"),
            new("Bankruci", "VaBanque i odpadniecie z gry przzy pierwszym pytaniu"),
            new("Mistrzowie pomyłek", "Za niepoprawne odpowiedzenie na wszystkie swoje pytania"),
            new("Samodzielni geniusze", "Wygranie gry bez podpowiedzi")
        };

    }

    public void AddBadge(Badge badge) => badges.Add(badge);

    public Badge FindBadge(string name) => badges.Count > 0 ? ( badges?.Find(badge => badge.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ) : null;

    public bool IsBadgeUnlocked(string name)
    {
        Badge badge = FindBadge(name);
        return badge != null;
    }
    public void UnlockBadge(string name)
    {
        if(FindBadge(name) != null)
        {
            FindBadge(name).Unlocked = true;
        }
    }
}
