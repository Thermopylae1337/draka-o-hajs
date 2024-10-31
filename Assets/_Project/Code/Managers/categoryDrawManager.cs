using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryDrawManager : MonoBehaviour
{
    const int ROUNDS_LIMIT = 7;

    public int currentRound = 0;
    public Wheel wheel;
    public TextMeshProUGUI categoryDisplayText;
    public TextMeshProUGUI roundDisplayText;

    string[] categories = new string[]       // temp
    {
        "Czarna Skrzynka",
        "Geografia",
        "Historia",
        "Sztuka i Literatura",
        "Nauka i Technologia",
        "Film i Telewizja",
        "Muzyka",
        "Sport",
        "Kulinarne Przepisy",
        "Wynalazki i Odkrycia",
        "Mitologia",
        "Jêzyki i Idiomy",
        "Zwierzêta",
        "Miejsca i Zabytki",
        "Trendy i Popkultura",
        "Ciekawe Fakty",
        "Legendy",
        "Psychologia",
        "Ekologia",
        "Gry i Zagadki",
        "Techniki Przetrwania",
        "Podró¿e",
        "Sztuki Walki",
        "Gospodarka",
        "Edukacja",
        "Technologia",
        "Motoryzacja",
        "Fizyka",
        "Chemia",
        "Biologia",
        "Astronomia"
    };

    void Start()
    {
        wheel.OnWheelStopped += HandleWheelStopped;
    }

    void HandleWheelStopped(int wynik)
    {
        string category = categories[wynik];
        categoryDisplayText.text = "Wylosowano: " + category;
        if (categories[wynik] == "Czarna Skrzynka")
        {
            // CzarnaSkrzynka()
        }
        else if (categories[wynik] == "PodpowiedŸ")
        {
            // dru¿yna.podpowiedzi++ or sth;
        }
        else
        {
            currentRound++;
            roundDisplayText.text = "Runda: " + currentRound;
            // WyœwietlPytanie(category)
        }
    }

    public void SpinWheel()
    {
        /* "Zawsze przed losowaniem musi byc sprawdzane, czy licznik ten jest wiêkszy od zera"
         * ~w moim przypakdu currentRound < ROUNDS_LIMIT
         * imo to powinno byæ sprawdzane przy po odpowiedzi na pytanie,
         * zamiast ³adowaæ scene losowania tylko ¿eby znów ³adowaæ podsumowanie
         */
        if (currentRound < ROUNDS_LIMIT)
        {
            wheel.SpinWheel();
        }
    }
}
