using UnityEngine;
using TMPro;

public class Panel : MonoBehaviour
{
    TMP_Text teamNameObj, moneyObj, accuracyObj, cluesUsedObj;
    float time = 0.0f;

    void Awake()    // awake zeby pobralo obiekty zanim manager wywola Initialize, ktore ich uzywa
    {
        teamNameObj = transform.Find("TeamName").GetComponent<TMP_Text>();
        moneyObj = transform.Find("Money").GetComponent<TMP_Text>();
        accuracyObj = transform.Find("Accuracy").GetComponent<TMP_Text>();
        cluesUsedObj = transform.Find("CluesUsed").GetComponent<TMP_Text>();
    }

    void Update()
    {
        time += Time.deltaTime;
        float scale = 0.87f + 0.13f * Mathf.Sin(time - transform.position.x * 0.002f);
        moneyObj.transform.localScale = new Vector2(scale, scale);
    }
    
    
    public void Initialize(Team team)
    {
        /*
        string teamName = team.name;
        int money = team.Money;
        float goodAnswers = team.goodAnswers;
        float wrongAnswers = team.wrongAnswers;
        float questions = team.goodAnswers + team.wrongAnswers;
        int cluesUsed = team.CluesUsed;
        */

        //temp
        string teamName = "Test init team";
        int money = 100000;
        float goodAnswers = 3;
        float wrongAnswers = 6;
        float questions = goodAnswers + wrongAnswers;
        int cluesUsed = 5;

        teamNameObj.text = teamName;
        moneyObj.text = $"<color=green>{money.ToString()}</color>pln";
        accuracyObj.text = $"<size=22>Poprawne odpowiedzi\n</size>" +
            $"<size=60><color=red>{Mathf.Round((goodAnswers / questions) * 100f).ToString()}%\n</color></size>" +
            $"<size=18>({goodAnswers.ToString()}/{questions.ToString()})</size>";
        cluesUsedObj.text = $"Wykorzystane podpowiedzi\n" +
            $"<size=60><color=orange>{cluesUsed.ToString()}</size>";
    }
}
