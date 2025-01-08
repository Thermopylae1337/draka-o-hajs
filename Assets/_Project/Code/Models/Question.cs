using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;

/// <summary>
/// Klasa do przechowywania informacji odnoszących się do wylosowanych pytań. 
/// </summary>
public class Question : INetworkSerializable, IEquatable<Question>
{
    /// <summary>
    /// Pobiera i ustawia treść konkretnego pytania.
    /// </summary>
    [JsonProperty("tresc")]
    public string Content { get => content; private set => content = value; }

    /// <summary>
    /// Lista przechowująca możliwe podpowiedzi do pytań.
    /// </summary>
    [JsonProperty("podpowiedzi", Order = 3)]
    private readonly List<string> answerChoices = new();

    /// <summary>
    /// Lista przechowująca poprawne odpowiedzi na pytanie.
    /// </summary>
    [JsonProperty("poprawneOdpowiedzi", Order = 2)]
    private List<string> correctAnswers = new();
    /// <summary>
    /// Zmienna przechowująca informacje o treści konkretnego pytania.
    /// </summary>
    private string content;
    /// <summary>
    /// Statyczne pole przechowujące instancję klasy <see cref="Random"/> do generowania losowych liczb.
    /// </summary>
    private static readonly Random _random = new();

    /// <summary>
    /// Pobiera losowo przetasowane podpowiedzi (opcje odpowiedzi) z listy. Zwraca tablicę stringów zawierające potasowane odpowiedzi.
    /// </summary>
    public string[] Hints
    {
        get
        {
            List<string> choicesCopy = new(answerChoices);

            // Fisher-Yates Shuffle na kopii listy
            for (int i = choicesCopy.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);

                (choicesCopy[j], choicesCopy[i]) = (choicesCopy[i], choicesCopy[j]);
            }

            return choicesCopy.ToArray();
        }
    }

    /// <summary>
    /// Pobiera listę poprawnych odpowiedzi na pytanie.
    /// </summary>
    public List<string> CorrectAnswers { get=>correctAnswers; }
    /// <summary>
    /// Konstruktor kopiujący inicjalizujący pole content oraz listy correctAnswers i answerChoices.   
    /// </summary>
    /// <param name="content">Zmienna reprezentująca treść pytania.</param>
    /// <param name="correctAnswers">Zmienna reprezentująca poprawną odpowiedź.</param>
    /// <param name="answerChoices">Zmienna reprezentująca użyte podpowiedzi.</param>
    public Question(string content, List<string> correctAnswers, List<string> answerChoices)
    {
        Content = content;
        this.correctAnswers = correctAnswers; // podane jako lista poprawne warianty odpowiedzi
        this.answerChoices = answerChoices.Count != 4 ? throw new ArgumentException("Niepoprawna ilość podpowiedzi") : answerChoices;
    }

    public Question()
    {
    }

    /// <summary>
    /// Sprawdza, czy podana odpowiedź jest poprawna w porównaniu do listy poprawnych odpowiedzi.
    /// </summary>
    /// <param name="answer">Zmienna reprezentująca odpowiedzi do sprawdzenia.</param>
    /// <returns>True, jeśli odpowiedź znajduje się na liście poprawncyh odpowiedzi; w przeciwnym wypadku zwraca False</returns>
    public bool IsCorrect(string answer)
    {
        return CorrectAnswers.Any(correctAnswer =>
            string.Equals(
                correctAnswer.Trim(),
                answer.Trim(),
                StringComparison.OrdinalIgnoreCase
            )
        );
    }

    /// <summary>
    /// Serializuję bieżący obiekt do formatu JSON i zapisuje go do podanego pliku.
    /// </summary>
    /// <param name="path">Ścieżka do pliku, w którym dane JSON mają zostać zapisane.</param>
    public void Serialize(string path)
    {
        JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Deserializuje obiekt typu Question z pliku JSON znajdującego się pod podaną ścieżką.
    /// </summary>
    /// <param name="path">Ścieżka do pliku JSON, który ma zostać zdeserializowany.</param>
    /// <returns>Obiekt typu <see cref="Question"/> odtworzony z danych JSON. Wyrzuca wyjątek, jeśli plik JSON nie istnieje.</returns>
    public static Question Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", path);
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Question>(json);
    }

    /// <summary>
    /// Serializuje dane obiektu do sieciowego formatu przy użyciu dostarczonego serializera.
    /// </summary>
    /// <typeparam name="T">Typ, który implementuje interfejs <see cref="IReaderWriter"/>. Określa sposób serializacji i deserializacji danych.</typeparam>
    /// <param name="serializer">Obiekt odpowiedzialny za serializację danych w formacie sieciowym.</param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref content);
        _ = Utils.NetworkSerializeList(serializer, CorrectAnswers);
        _ = Utils.NetworkSerializeList(serializer, answerChoices);
    }
    /// <summary>
    /// Porównuje bieżący obiekt z innym obiektem na podstawie treści (content).
    /// </summary>
    /// <param name="question">Przechowuje konkretne pytanie.</param>
    /// <returns>True, jeśli treści pytań są identyczne; w przeciwnym razie false.</returns>
    public bool Equals(Question question) => content == question.content;
}
