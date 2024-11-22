using UnityEngine;
using TMPro;
using System;

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
        "J�zyki i Idiomy",
        "Zwierz�ta",
        "Miejsca i Zabytki",
        "Trendy i Popkultura",
        "Ciekawe Fakty",
        "Legendy",
        "Psychologia",
        "Ekologia",
        "Gry i Zagadki",
        "Techniki Przetrwania",
        "Podr�e",
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
        else if (categories[result] == "Podpowied�")
        {
            // dru�yna.podpowiedzi++ or sth;
        }
        else
        {
            currentRound++;
            roundDisplayText.text = "Runda: " + currentRound;
            // Wy�wietlPytanie(category)
        }
    }
    
    void AwardBiddingWinners(Team team, String categoryName)
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
        /* "Zawsze przed losowaniem musi byc sprawdzane, czy licznik ten jest wi�kszy od zera"
         * ~w moim przypakdu currentRound < ROUNDS_LIMIT
         * imo to powinno by� sprawdzane przy po odpowiedzi na pytanie,
         * zamiast �adowa� scene losowania tylko �eby zn�w �adowa� podsumowanie
         */
        if (currentRound < Utils.ROUNDS_LIMIT)
        {
            wheel.SpinWheel();
        }
    }
}
