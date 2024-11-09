using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class Category
{
    [JsonProperty("nazwa")]
    public string Name { get; }
    [JsonProperty("pytania", Order = 2)]
    public List<Question> QuestionList;

    public Category(string name)
    {
        Name = name;
        QuestionList = new List<Question>();
    }

    [JsonConstructor]
    public Category(string nazwa, List<Question> list)
    {
        this.Name = nazwa;
        this.QuestionList = list;
    }

    public void AddQuestionToList(Question question)
    {
        if (question == null)
        {
            return;
        }
        QuestionList.Add(question);
    }

    public Question DrawQuestion()
    {
        System.Random random = new System.Random();
        Question question = QuestionList.Count == 0 ? null : QuestionList[random.Next(QuestionList.Count)];

        QuestionList.Remove(question);
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
