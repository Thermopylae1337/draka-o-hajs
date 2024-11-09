using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class Category
{
    [JsonProperty("nazwa")]
    public string Nazwa { get; }
    [JsonProperty("pytania", Order = 2)]
    public List<Question> ListaPytań;

    public Category(string nazwa)
    {
        Nazwa = nazwa;
        ListaPytań = new List<Question>();
    }

    [JsonConstructor]
    public Category(string nazwa, List<Question> lista)
    {
        this.Nazwa = nazwa;
        this.ListaPytań = lista;
    }

    public void DodajPytanieDoListy(Question pytanie)
    {
        if (pytanie == null)
        {
            return;
        }
        ListaPytań.Add(pytanie);
    }

    public Question LosujPytanie()
    {
        System.Random random = new System.Random();
        Question question = ListaPytań.Count == 0 ? null : ListaPytań[random.Next(ListaPytań.Count)];

        ListaPytań.Remove(question);
        return question;

    }

    public void Serializuj(string sciezka)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(sciezka, json);
    }

    public static Category Deserializuj(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", sciezka);
        }
        string json = File.ReadAllText(sciezka);
        return JsonConvert.DeserializeObject<Category>(json);
    }
}
