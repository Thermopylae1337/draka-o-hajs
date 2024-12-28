using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Mathematics;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;

public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;
    private static readonly System.Random _random = new();

    private void Start()
    {
        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {
            TeamManager team = teamClient.PlayerObject.GetComponent<TeamManager>();

            while (team.BlackBoxes > 0)
            {
                team.BlackBoxes--;

                double los = _random.NextDouble();

                if (los < 0.8) // 80% szans na pieniądze
                {
                    int money = DrawMoney();
                    team.Money += money;
                }
                else // 20% szans na odznakę
                {
                    string badge = DrawBadge();
                    //team.Badges.
                }
                // animacja (otwiera sie skrzynia i na skrzyni pojawia sie tekst co wylosowano) + nazwa druzyny  

            }

            
            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(team);
            //panel.Initialize(teamClient.PlayerObject.GetComponent<TeamManager>());
        }
    }

    private static int DrawMoney()
    {
        int[] progi = Enumerable.Range(0, 21).Select(i => i == 0 ? 1 : i * 500).ToArray() ; // progi kwot [1, 500, 1000, .. , 10000]
        double[] szanse = new double[]
        { 0.08, 0.07, 0.07, 0.07, 0.07, 0.07, 0.07, 0.07, 0.06, 0.06, 0.06, 0.06,0.03, 0.03, 0.03, 0.03, 0.03, 0.01, 0.01, 0.01}; // Szansa na wylosowanie każdego progu

        double los = _random.NextDouble();
        double kumulatywnaSzansa = 0.0;

        for (int i = 0; i < progi.Length; i++)
        {
            kumulatywnaSzansa += szanse[i];
            if (los < kumulatywnaSzansa)
            {
                return progi[i];
            }
        }

        return 1; // Domyślna wartość (nigdy nie powinna wystąpić)
    }

    private static string DrawBadge()
    {
        string[] odznaki = new string[] { "Samochód", "Ogórek" };
        double[] szanse = new double[] { 0.7, 0.3 }; // 70% na samochód, 30% na ogórka

        double los = _random.NextDouble();
        double kumulatywnaSzansa = 0.0;

        for (int i = 0; i < odznaki.Length; i++)
        {
            kumulatywnaSzansa += szanse[i];
            if (los < kumulatywnaSzansa)
            {
                return odznaki[i];
            }
        }

        return "Ogórek"; // Domyślna wartość (nigdy nie powinna wystąpić)
    }

    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click
}
