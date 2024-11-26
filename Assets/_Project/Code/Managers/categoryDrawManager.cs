using TMPro;
using UnityEngine;

public class CategoryDrawManager : MonoBehaviour
{
    private int currentRound = 0;
    private Wheel wheel;
    private TMP_Text categoryDisplayText;
    private TMP_Text roundDisplayText;
    private readonly string[] categories = new string[]       // temp
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
        "Języki i Idiomy",
        "Zwierzęta",
        "Miejsca i Zabytki",
        "Trendy i Popkultura",
        "Ciekawe Fakty",
        "Legendy",
        "Psychologia",
        "Ekologia",
        "Gry i Zagadki",
        "Techniki Przetrwania",
        "Podróże",
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

    private void Start()
    {
        wheel = GameObject.Find("Wheel").GetComponent<Wheel>();
        categoryDisplayText = GameObject.Find("CategoryDisplay").GetComponent<TMP_Text>();
        roundDisplayText = GameObject.Find("RoundCounter").GetComponent<TMP_Text>();

        wheel.OnWheelStopped += HandleWheelStopped;
    }

    private void HandleWheelStopped(int result)
    {
        string category = categories[result];
        categoryDisplayText.text = "Wylosowano: " + category;
        if (categories[result] == "Czarna Skrzynka")
        {
            // CzarnaSkrzynka()
        }
        else if (categories[result] == "Podpowiedź")
        {
            // drużyna.podpowiedzi++ or sth;
        }
        else
        {
            currentRound++;
            roundDisplayText.text = "Runda: " + currentRound;
            // WyświetlPytanie(category)
        }
    }

    private void AwardBiddingWinners(Team team, string categoryName)
    {
        if (categoryName.Equals("Czarna Skrzynka"))
        {
            team.BlackBoxes++;
        }
        else if (categoryName.Equals("Podpowiedz"))
        {
            team.Clues++;
        }
        else //wylosowano kategorie pytaniowa
        {
            //todo tutaj wywolac metode rozpoczynajaca etap pytania
        }
    }

    public void SpinWheel()
    {
        /* "Zawsze przed losowaniem musi byc sprawdzane, czy licznik ten jest większy od zera"
         * ~w moim przypadku currentRound < ROUNDS_LIMIT
         * imo to powinno by� sprawdzane przy po odpowiedzi na pytanie,
         * zamiast �adowa� scene losowania tylko �eby zn�w �adowa� podsumowanie
         */
        if (currentRound < Utils.ROUNDS_LIMIT)
        {
            wheel.SpinWheel();
        }
    }
}
