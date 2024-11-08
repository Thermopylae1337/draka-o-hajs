using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


public class Kategoria
{
    public string Nazwa { get; }
    [JsonProperty("Lista Pytań", Order = 2)]
    public List<Pytanie> ListaPytań;

    public Kategoria(string nazwa)
    {
        Nazwa = nazwa;
        ListaPytań = new List<Pytanie>();
    }

    [JsonConstructor]
    public Kategoria(string nazwa, List<Pytanie> lista)
    {
        this.Nazwa = nazwa;
        this.ListaPytań = lista;
    }

    public void DodajPytanieDoListy(Pytanie pytanie)
    {
        if (pytanie == null)
        {
            return;
        }
        ListaPytań.Add(pytanie);
    }

    public Pytanie LosujPytanie()
    {
        Random random = new Random();
        Pytanie question = ListaPytań.Count == 0 ? null : ListaPytań[random.Next(ListaPytań.Count)];

        ListaPytań.Remove(question);
        return question;

    }

    public void Serializuj(string sciezka)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(sciezka, json);
    }

    public static Kategoria Deserializuj(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", sciezka);
        }
        string json = File.ReadAllText(sciezka);
        return JsonConvert.DeserializeObject<Kategoria>(json);
    }

}
