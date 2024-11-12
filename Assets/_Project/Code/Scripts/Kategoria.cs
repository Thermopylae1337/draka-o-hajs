using UnityEngine;
using System;
public class Kategoria
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string[] pytania;

    // Tablica odpowiedzi
    public string[] odpowiedzi;

    public string[][] podpowiedzi;

    private int indexLosowegoPytania;
    public Kategoria()
    {
        // Inicjalizacja tablic pyta� i odpowiedzi w konstruktorze
        pytania = new string[]
        {
            "Stolica Polski?",
            "Najwy�szy szczyt �wiata?",
            "Najwi�kszy ocean?",
            "G��wny sk�adnik sushi?",
            "Najbli�sza gwiazda?",
            "J�zyk urz�dowy w Brazylii?",
            "Stolica Francji?",
            "Wynalazca telefonu?",
            "Planeta znana jako czerwona planeta?",
            "Najwi�kszy kontynent?"
        };

        odpowiedzi = new string[]
        {
            "Warszawa",
            "Mount Everest",
            "Pacyfik",
            "Ry�",
            "S�o�ce",
            "Portugalski",
            "Pary�",
            "Edison",
            "Mars",
            "Azja"
        };

        podpowiedzi = new string[][]
        {
            // Pytanie 1
            new string[] { "Warszawa", "Krak�w", "Wroc�aw", "Gda�sk" },
            // Pytanie 2
            new string[] { "Mount Everest", "K2", "Kilimand�aro", "Mont Blanc" },
            // Pytanie 3
            new string[] { "Pacyfik", "Atlantyk", "Indyjski", "Arktyczny" },
            // Pytanie 4
            new string[] { "Ry�", "Makaron", "Chleb", "Kasza" },
            // Pytanie 5
            new string[] { "S�o�ce", "Ksi�yc", "Proxima Centauri", "Sirius" },
            // Pytanie 6
            new string[] { "Portugalski", "Hiszpa�ski", "Francuski", "Angielski" },
            // Pytanie 7
            new string[] { "Pary�", "Londyn", "Berlin", "Madryt" },
            // Pytanie 8
            new string[] { "Edison", "Bell", "Tesla", "Marconi" },
            // Pytanie 9
            new string[] { "Mars", "Jowisz", "Merkury", "Saturn" },
            // Pytanie 10
            new string[] { "Azja", "Europa", "Afryka", "Ameryka" }
        };
}

    public string LosujPytanie()
    {
        System.Random rnd = new System.Random();
        indexLosowegoPytania = rnd.Next(0, pytania.Length);
        return pytania[indexLosowegoPytania];
    }
    public string PobierzPoprawnaOdpowiedz()
    {
        return odpowiedzi[indexLosowegoPytania];
    }

    public string[] PobierzPodpowiedzi()
    {
        return podpowiedzi[indexLosowegoPytania];
    }

}
