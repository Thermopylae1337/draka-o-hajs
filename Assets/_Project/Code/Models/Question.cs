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
    private List<string> correctAnswers = new List<string>();
    [JsonProperty("podpowiedzi", Order = 3)]
    private List<string> answers = new List<string>();


    public Question(string Tresc, List<string> correctAnswers, string odpA, string odpB, string odpC, string odpD)
    {
        this.Tresc = Tresc;
        this.correctAnswers = correctAnswers; // podane jako lista poprawne warianty odpowiedzi


        answers.Add(odpA);
        answers.Add(odpB);
        answers.Add(odpC);
        answers.Add(odpD);
    }

    public bool IsCorrenct(string answer)
    {
        return correctAnswers.Contains(answer.Trim().ToLower());
    }

    public string Hint()
    {
        return $"A: {answers[0]}, B: {answers[1]}, C: {answers[2]}, D: {answers[3]}";
    }


    public void Serialize(string path)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.None,
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
}
