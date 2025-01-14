using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// Klasa służąca do przechowywania informacjii o możliwych odznak do zdobycia przez drużynę.
/// </summary>
public class BadgeList
{
    /// <summary>
    /// Lista odczytująca przechowane odznaki.
    /// </summary>
    [JsonProperty("badges")]
    private readonly List<Badge> badges;

    /// <summary>
    /// Konstruktor tworzący listę możliwych odznak.
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
            new("Samodzielni geniusze", "Wygranie gry bez podpowiedzi")
        };

    }

    /// <summary>
    /// Właściwość tylko do odczytu, zwracająca listę odznak.
    /// </summary>
    public List<Badge> Badges
    {
        get => badges;
    }
    /// <summary>
    /// Metoda odpowiadająca za dodanie odznaki do listy posiadanych przez drużynę.
    /// </summary>
    /// <param name="badge">Zmienna reprezentująca odznakę.</param>
    public void AddBadge(Badge badge) => badges.Add(badge);

    /// <summary>
    /// Metoda służąca do znalezienia wybranej odznaki.
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki</param>
    /// <returns>Zwraca obiekt reprezentujący znalezioną odznakę lub wartość null, jeśli odznaka nie została odnaleziona </returns>
    public Badge FindBadge(string name) => badges.Count > 0 ? ( badges?.Find(badge => badge.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ) : null;

    /// <summary>
    /// Metoda sprawdzająca czy odznaka została odblokowana.
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki.</param>
    /// <returns>W zależności czy odznaka została odblokowana zwraca true, jeśli nie odnaleziono zwraca false</returns>
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
