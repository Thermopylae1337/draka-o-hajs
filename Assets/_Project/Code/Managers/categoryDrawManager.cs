using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class CategoryDrawManager : MonoBehaviour
{
    int currentRound = 0;
    Wheel wheel;
    TMP_Text categoryDisplayText;
    TMP_Text roundDisplayText;

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
        wheel = GameObject.Find("Wheel").GetComponent<Wheel>();
        categoryDisplayText = GameObject.Find("CategoryDisplay").GetComponent<TMP_Text>();
        roundDisplayText = GameObject.Find("RoundCounter").GetComponent<TMP_Text>();

        wheel.OnWheelStopped += HandleWheelStopped;
    }

    void HandleWheelStopped(int result)
    {
        string category = categories[result];
        categoryDisplayText.text = "Wylosowano: " + category;
        if (categories[result] == "Czarna Skrzynka")
        {
            // CzarnaSkrzynka()
        }
        else if (categories[result] == "PodpowiedŸ")
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
