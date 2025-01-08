using Newtonsoft.Json;

/// <summary>
/// Klasa reprezentująca odznakę, zawierająca informacje o nazwie, stanie odblokowania oraz warunku odblokowania odznaki.
/// </summary>
public class Badge
{
    /// <summary>
    /// Zmienna przechowująca nazwę odznaki.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; private set; }

    /// <summary>
    /// Zmienna przechowująca informacje czy odznaka została odblokowana.
    /// </summary>
    [JsonProperty("unlocked")]
    public bool Unlocked { get; set; }

    /// <summary>
    /// Zmienna przechowująca informacje jaki warunek musi zostać spełniony do odblokowania danej odznaki.
    /// </summary>
    [JsonProperty("unlockCondition")]
    public string UnlockCondition { get; private set; }

    /// <summary>
    /// Zmienna przechowująca licznik zdobytych odznak.
    /// </summary>
    [JsonProperty("unlockCounter")]
    public int UnlockCounter { get; private set; }

    /// <summary>
    /// Konstruktor inicjalizujący wszystkie  pola składowe klasy
    /// </summary>
    /// <param name="name">Nazwa odznaki</param>
    /// <param name="unlockCondition">Opis warunku spełnionego, aby odblokować odznakę.</param>
    public Badge(string name, string unlockCondition)
    {
        Name = name;
        UnlockCondition = unlockCondition;
        Unlocked = false;
        UnlockCounter = 0;
    }
}
