using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;

public class Question : INetworkSerializable, IEquatable<Question>
{

    [JsonProperty("tresc")]
    public string Content { get => content; private set => content = value; }

    [JsonProperty("podpowiedzi", Order = 3)]
    private readonly List<string> answerChoices = new();

    [JsonProperty("poprawneOdpowiedzi", Order = 2)]
    private List<string> correctAnswers = new();
    private string content;
    private static readonly Random _random = new();

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

    public List<string> CorrectAnswers { get=>correctAnswers; }
    public Question(string content, List<string> correctAnswers, List<string> answerChoices)
    {
        Content = content;
        this.correctAnswers = correctAnswers; // podane jako lista poprawne warianty odpowiedzi
        this.answerChoices = answerChoices.Count != 4 ? throw new ArgumentException("Niepoprawna ilość podpowiedzi") : answerChoices;
    }

    public Question()
    {
    }

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

    public static Question Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", path);
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Question>(json);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref content);
        _ = Utils.NetworkSerializeList(serializer, CorrectAnswers);
        _ = Utils.NetworkSerializeList(serializer, answerChoices);
    }
    public bool Equals(Question question) => content == question.content;
}
