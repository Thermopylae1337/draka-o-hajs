using TMPro;
using UnityEngine;

public class Panel : MonoBehaviour
{
    private TMP_Text teamNameObj, moneyObj, accuracyObj, cluesUsedObj;
    private float time = 0.0f;

    private void Awake()    // awake zeby pobrało obiekty zanim manager wywoła Initialize, ktore ich uzywa
    {
        teamNameObj = transform.Find("TeamName").GetComponent<TMP_Text>();
        moneyObj = transform.Find("Money").GetComponent<TMP_Text>();
        accuracyObj = transform.Find("Accuracy").GetComponent<TMP_Text>();
        cluesUsedObj = transform.Find("CluesUsed").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        float scale = 0.87f + ( 0.13f * Mathf.Sin(time - ( transform.position.x * 0.002f )) );
        moneyObj.transform.localScale = new Vector2(scale, scale);
    }

    public void Initialize(TeamManager team)
    {
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
