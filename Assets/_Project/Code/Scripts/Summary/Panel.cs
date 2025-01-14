using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
//using Assets._Project.Code.Models;

/// <summary>
/// Klasa odpowiedzialna za streszcznie po zakończeniu rozgrywki.
/// </summary>
public class Panel : MonoBehaviour
{
    /// <summary>
    /// Lista przechowująca sprite'y, które będą używane do wyświetlania różnych paneli w interfejsie użytkownika.
    /// </summary>
    [SerializeField]
    public List<Sprite> panelSprites = new List<Sprite>();              // wsadzać Y, G, B !

    /// <summary>
    /// Zmienne typu TMP_Text przechowujące informacje o drużynie takie jak nazwa, pieniadze, poprawność oraz liczba wykorzystanych podpowiedzi.
    /// </summary>
    private TMP_Text teamNameObj, moneyObj, accuracyObj, cluesUsedObj;
    /// <summary>
    /// Zmienna przechowująca czas.
    /// </summary>
    private float time = 0.0f;

    /// <summary>
    /// Metoda pobierająca informacje o drużynie.
    /// </summary>
    private void Awake()    // awake zeby pobrało obiekty zanim manager wywoła Initialize, ktore ich uzywa
    {
        teamNameObj = transform.Find("TeamName").GetComponent<TMP_Text>();
        moneyObj = transform.Find("Money").GetComponent<TMP_Text>();
        accuracyObj = transform.Find("Accuracy").GetComponent<TMP_Text>();
        cluesUsedObj = transform.Find("CluesUsed").GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Metoda odpowiedzialna za animowanie ilości zdobytej gotówki przez drużynę.
    /// </summary>
    private void Update()
    {
        time += Time.deltaTime;
        float scale = 0.87f + ( 0.13f * Mathf.Sin(time - ( transform.position.x * 0.002f )) );
        moneyObj.transform.localScale = new Vector2(scale, scale);
    }

    /// <summary>
    /// Inicjalizuje dane panelu na podstawie obiektu drużyny.
    /// </summary>
    /// <param name="team">Obiekt drużyny, którego dane będą wyświetlane.</param>
    public void Initialize(TeamManager team)
    {
        if (team.Colour == ColourEnum.YELLOW)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Summary/tabelka_zolci");
        else if (team.Colour == ColourEnum.GREEN)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Summary/tabelka_zieloni");
        else
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Summary/tabelka_niebiescy");
        
        string teamName = team.TeamName;
        int money = team.Money;
        float goodAnswers = team.QuestionsAnswered;
        float wrongAnswers = team.QuestionsAsked - team.QuestionsAnswered;
        float questions = team.QuestionsAsked;
        int cluesUsed = team.CluesUsed;

        //temp
        //string teamName = "Test init team";
        //int money = 100000;
        //float goodAnswers = 3;
        //float wrongAnswers = 6;
        //float questions = goodAnswers + wrongAnswers;
        //int cluesUsed = 5;

        LeaderboardList leaderboard = new LeaderboardList();
        leaderboard.Deserializuj();
        leaderboard.AddTeam(new LeaderboardTeam(teamName, money));
        leaderboard.Serializuj();

        teamNameObj.text = teamName;
        moneyObj.text = $"<color=green>{money}</color>pln";

        accuracyObj.text = questions == 0 
            ?  $"<size=22>Poprawne odpowiedzi\n</size>" +
            $"<size=60><color=red>0%\n</color></size>" +
            $"<size=18>(0/0)</size>" 
            :  $"<size=22>Poprawne odpowiedzi\n</size>" +
            $"<size=60><color=red>{Mathf.Round(goodAnswers / questions * 100f)}%\n</color></size>" +
            $"<size=18>({goodAnswers}/{questions})</size>";

        cluesUsedObj.text = $"Wykorzystane podpowiedzi\n" +
            $"<size=60><color=orange>{cluesUsed}</size>";
    }
}
