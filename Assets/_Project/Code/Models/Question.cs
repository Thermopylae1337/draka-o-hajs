using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

public class Question { 

    [JsonProperty("tresc")]
    public string Tresc { get; private set; }
    [JsonProperty("poprawneOdpowiedzi", Order = 2)]
    private List<string> poprawneOdpowiedzi = new List<string>();
    [JsonProperty("podpowiedzi", Order = 3)]
    private List<string> odpowiedzi = new List<string>();


    public Question(string Tresc, List<string> poprawneOdpowiedzi, string odpA, string odpB, string odpC, string odpD)
    {
        this.Tresc = Tresc;
        this.poprawneOdpowiedzi = poprawneOdpowiedzi; // podane jako lista poprawne warianty odpowiedzi


        odpowiedzi.Add(odpA);
        odpowiedzi.Add(odpB);
        odpowiedzi.Add(odpC);
        odpowiedzi.Add(odpD);
    }

    public bool czyPoprawna(string odpowiedz)
    {
        return poprawneOdpowiedzi.Contains(odpowiedz.Trim().ToLower());
    }

    public string Podpowiedz()
    {
        return $"A: {odpowiedzi[0]}, B: {odpowiedzi[1]}, C: {odpowiedzi[2]}, D: {odpowiedzi[3]}";
    }


    public void Serializuj(string sciezka)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.None,
            NullValueHandling = NullValueHandling.Ignore

        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(sciezka, json);
    }

    public static Question Deserializuj(string sciezka)
    {
        if (!File.Exists(sciezka))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", sciezka);
        }
        string json = File.ReadAllText(sciezka);
        return JsonConvert.DeserializeObject<Question>(json);
    }
}
