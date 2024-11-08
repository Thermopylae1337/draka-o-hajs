using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Pytanie
{
    public string Tresc { get; private set; }
    [JsonProperty("Poprawne Odpowiedzi", Order = 2)]
    private List<string> poprawneOdpowiedzi = new List<string>();
    [JsonProperty("Odpowiedzi", Order = 3)]
    private Dictionary<char, string> odpowiedzi;


    public Pytanie(string Tresc, List<string> poprawneOdpowiedzi, string odpA, string odpB, string odpC, string odpD)
    {
        this.Tresc = Tresc;
        this.poprawneOdpowiedzi = poprawneOdpowiedzi; // podane jako lista poprawne warianty odpowiedzi

        odpowiedzi = new Dictionary<char, string>
        {
            {'A', odpA},
            {'B', odpB},
            {'C', odpC},
            {'D', odpD}
        };
    }

    public bool czyPoprawna(string odpowiedz)
    {
        return poprawneOdpowiedzi.Contains(odpowiedz.Trim().ToLower());
    }

    public string Podpowiedz()
    {
        return $"A: {odpowiedzi['A']}, B: {odpowiedzi['B']}, C: {odpowiedzi['C']}, D: {odpowiedzi['D']}";
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

    public static Pytanie Deserializuj(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", sciezka);
        }
        string json = File.ReadAllText(sciezka);
        return JsonConvert.DeserializeObject<Pytanie>(json);
    }
}

