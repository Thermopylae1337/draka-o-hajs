using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ListaKategorii
{
    [JsonProperty("Lista Kategorii")]
    List<Kategoria> listaKategorii = new List<Kategoria>();

    public ListaKategorii(List<Kategoria> lista)
    {
        listaKategorii = lista;
    }

    public List<Pytanie> WyszukajKategorie(string nazwa)
    {
        if (string.IsNullOrWhiteSpace(nazwa))
        {
            throw new ArgumentNullException("Nazwa kategorii nie może być pusta.");
        }

        foreach (Kategoria item in listaKategorii)
        {
            if (item.Nazwa.Equals(nazwa, StringComparison.OrdinalIgnoreCase))
            {
                return item.ListaPytań;
            }
        }
        throw new Exception("Nie znaleziono kategorii");
    }

    public void DodajKategorię(Kategoria k)
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
            NullValueHandling = NullValueHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(sciezka, json);
    }

    public static ListaKategorii Deserializuj(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", sciezka);
        }
        string json = File.ReadAllText(sciezka);
        return JsonConvert.DeserializeObject<ListaKategorii>(json);
    }

}
