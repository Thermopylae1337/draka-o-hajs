using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor.Search;

public class Question { 

    [JsonProperty("tresc")]
    public string content { get; private set; }
    [JsonProperty("poprawneOdpowiedzi", Order = 2)]
    private List<string> correctAnswers;
    [JsonProperty("podpowiedzi", Order = 3)]
    private List<string> falseAnswers;  // tutaj znajduje się 1 prawidłowa podpowiedź do ew. wyboru przez gracza 
    private static System.Random random = new System.Random();
    public string[] Hints { get
        {
            List<string> shuffledAnswers = new List<string>(falseAnswers);

            // Fisher-Yates Shuffle na kopii listy
            for (int i = shuffledAnswers.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                string temp = shuffledAnswers[i];
                shuffledAnswers[i] = shuffledAnswers[j];
                shuffledAnswers[j] = temp;
            }

            return shuffledAnswers.ToArray(); 
        } }


    public Question(string Tresc, List<string> correctAnswers, List<string> falseAnswers)
    {
        this.content = Tresc;
        this.correctAnswers = correctAnswers; // podane jako lista poprawne warianty odpowiedzi
        this.falseAnswers = falseAnswers.Count == 4 ? throw new ArgumentException("Niepoprawna ilość podpowiedi"): falseAnswers;
    }

    public bool IsCorrect(string answer)
    {
        return correctAnswers.Contains(answer.Trim().ToLower());
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
