using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class Category
{
    [JsonProperty("nazwa")]
    public string name { get; }
    [JsonProperty("pytania", Order = 2)]
    public List<Question> questionList;
    private static System.Random random = new System.Random();

    public Category(string name)
    {
        this.name = name;
        questionList = new List<Question>();
    }

    [JsonConstructor]
    public Category(string nazwa, List<Question> list)
    {
        this.name = nazwa;
        this.questionList = list;
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
        Question question = questionList.Count == 0 ? null : questionList[random.Next(questionList.Count)];

        questionList.Remove(question);
        return question;
    }

    public void Serialize(string path)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
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
}
