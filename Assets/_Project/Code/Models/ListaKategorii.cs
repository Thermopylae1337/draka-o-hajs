using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
public class CategoryList
{
    [JsonProperty("listaKategorii")]
    List<Category> listaKategorii = new List<Category>(); 

    public CategoryList(List<Category> lista) 
    {
        listaKategorii = lista;
    }
        
    public List<Question> WyszukajKategorie(string nazwa)
    {
        if (string.IsNullOrWhiteSpace(nazwa))
        {
            throw new ArgumentNullException("Nazwa kategorii nie może być pusta.");
        }

        foreach (Category item in listaKategorii)
        {
            if (item.Nazwa.Equals(nazwa, StringComparison.OrdinalIgnoreCase))
            {
                return item.questionList;
            }
        }
        throw new Exception("Nie znaleziono kategorii");
    }

    public void DodajKategorię(Category k)
    {
        if (k == null)
        {
            throw new ArgumentNullException(nameof(k), "Kategoria nie może być null.");
        }
        listaKategorii.Add(k);
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

    public static CategoryList Deserializuj(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", sciezka);
        }
        string json = File.ReadAllText(sciezka);
        return JsonConvert.DeserializeObject<CategoryList>(json);
    }
}
