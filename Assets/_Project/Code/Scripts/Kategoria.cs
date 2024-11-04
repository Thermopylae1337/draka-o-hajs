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
        // Inicjalizacja tablic pytañ i odpowiedzi w konstruktorze
        pytania = new string[]
        {
            "Stolica Polski?",
            "Najwy¿szy szczyt œwiata?",
            "Najwiêkszy ocean?",
            "G³ówny sk³adnik sushi?",
            "Najbli¿sza gwiazda?",
            "Jêzyk urzêdowy w Brazylii?",
            "Stolica Francji?",
            "Wynalazca telefonu?",
            "Planeta znana jako czerwona planeta?",
            "Najwiêkszy kontynent?"
        };

        odpowiedzi = new string[]
        {
            "Warszawa",
            "Mount Everest",
            "Pacyfik",
            "Ry¿",
            "S³oñce",
            "Portugalski",
            "Pary¿",
            "Edison",
            "Mars",
            "Azja"
        };

        podpowiedzi = new string[][]
        {
            // Pytanie 1
            new string[] { "Warszawa", "Kraków", "Wroc³aw", "Gdañsk" },
            // Pytanie 2
            new string[] { "Mount Everest", "K2", "Kilimand¿aro", "Mont Blanc" },
            // Pytanie 3
            new string[] { "Pacyfik", "Atlantyk", "Indyjski", "Arktyczny" },
            // Pytanie 4
            new string[] { "Ry¿", "Makaron", "Chleb", "Kasza" },
            // Pytanie 5
            new string[] { "S³oñce", "Ksiê¿yc", "Proxima Centauri", "Sirius" },
            // Pytanie 6
            new string[] { "Portugalski", "Hiszpañski", "Francuski", "Angielski" },
            // Pytanie 7
            new string[] { "Pary¿", "Londyn", "Berlin", "Madryt" },
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
