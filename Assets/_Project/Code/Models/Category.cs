using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;

public class Category : INetworkSerializable, IEquatable<Category>
{
    [JsonProperty("nazwa")]
    public string Name => name;

    [JsonProperty("pytania", Order = 2)]
    public List<Question> questionList = new();

    private static readonly System.Random _random = new();
    private string name = string.Empty;

    public Category(string name) => this.name = name;

    [JsonConstructor]
    public Category(string nazwa, List<Question> list)
    {
        name = nazwa;
        questionList = list;
    }

    public Category()
    {
    }

    public void AddQuestionToList(Question question)
    {
        if (question == null)
        {
            return;
        }

        questionList.Add(question);
    }

    public Question DrawQuestion()
    {
        Question question = questionList.Count == 0 ? null : questionList[_random.Next(questionList.Count)];

        _ = questionList.Remove(question);
        return question;
    }

    public void Serialize(string path)
    {
        JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(path, json);
    }

    public static Category Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", path);
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Category>(json);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);

        _ = Utils.NetworkSerializeList(serializer, questionList);
    }

    public bool Equals(Category category) => name == category.name;
}
