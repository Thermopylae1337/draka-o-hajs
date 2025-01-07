using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// Klasa służąca do przechowywania informacjii o odznakach uzyskanych przez drużynę.
/// </summary>
public class BadgeList
{
    /// <summary>
    /// Zmienna przechowywująca listę odznak odblokowanych przez drużynę.
    /// </summary>
    [JsonProperty("badges")]
    private readonly List<Badge> badges;

    /// <summary>
    /// Konstruktor klasy.
    /// </summary>
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
            new("Samodzielni geniusze", "Wygranie gry bez podpowiedzi"),
            new("Łowcy ogórka","Za wylosowanie kiszonego ogórka"),
            new("Symboliczna złotówka" ,"Za wylosowanie jednej złotówki"),
            new("Samochód", "Odznaka za wygranie samochodu"),
            new("Nagroda + 5000zł", "Za wylosowanie 5 000 zł"),
            new("Nagroda + 10000zł", "Za wylosowanie 10 000 zł")
        };

    }
    /// <summary>
    /// Metoda odpowiadająca za dodanie odznaki do listy posiadancyh przez drużynę.
    /// </summary>
    /// <param name="badge">Zmienna reprezentująca odznakę.</param>
    public void AddBadge(Badge badge) => badges.Add(badge);
    /// <summary>
    /// Metoda służąca do znalezienia odznaki.
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki.</param>
    /// <returns>Zwraca obiekt odznaki.</returns>
    public Badge FindBadge(string name) => badges.Count > 0 ? ( badges?.Find(badge => badge.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ) : null;
    /// <summary>
    /// Metoda służąca do stwierdzenia czy odznaka została odblokowana przez drużynę.
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki.</param>
    /// <returns>Zwraca true jeśli odznaka jest odblokowana, false w przeciwnym wypadku.</returns>
    public bool IsBadgeUnlocked(string name)
    {
        Badge badge = FindBadge(name);
        return badge != null;
    }
    /// <summary>
    /// Metoda służąca do odblokowania odznaki przez drużynę.
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki.</param>
    public void UnlockBadge(string name)
    {
        if(FindBadge(name) != null)
        {
            FindBadge(name).Unlocked = true;
        }
    }
}
