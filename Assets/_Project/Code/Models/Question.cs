using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;

public class Question : INetworkSerializable
{

    [JsonProperty("tresc")]
    public string Content { get => content; private set => content = value; }
    [JsonProperty("poprawneOdpowiedzi", Order = 2)]
    private readonly List<string> correctAnswers;
    [JsonProperty("podpowiedzi", Order = 3)]
    private readonly List<string> answerChoices;
    private static readonly Random _random = new();
    private string content;

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

    public Question(string content, List<string> correctAnswers, List<string> falseAnswers)
    {
        Content = content;
        this.correctAnswers = correctAnswers; // podane jako lista poprawne warianty odpowiedzi
        answerChoices = falseAnswers.Count == 4 ? throw new ArgumentException("Niepoprawna ilość podpowiedzi") : falseAnswers;
    }

    public bool IsCorrect(string answer)
    {
        return correctAnswers.Contains(answer.Trim().ToLower());
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
        _ = Utils.NetworkSerializeList(serializer, correctAnswers);
        _ = Utils.NetworkSerializeList(serializer, answerChoices);
    }
}
