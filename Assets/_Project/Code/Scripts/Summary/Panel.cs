using UnityEngine;
using TMPro;

public class Panel : MonoBehaviour
{
    [SerializeField] int teamId = 0;
    TMP_Text teamNameObj, moneyObj, accuracyObj, cluesUsedObj;
    float time = 0.0f;

    void Start()
    {
        teamNameObj = transform.Find("TeamName").GetComponent<TMP_Text>();
        moneyObj = transform.Find("Money").GetComponent<TMP_Text>();
        accuracyObj = transform.Find("AccuracyTxt").GetComponent<TMP_Text>();
        cluesUsedObj = transform.Find("CluesUsed").GetComponent<TMP_Text>();

        Initialize(teamId);
    }

    void Update()
    {
        time += Time.deltaTime;
        float scale = 0.87f + 0.13f * Mathf.Sin(time - transform.position.x * 0.002f);
        moneyObj.transform.localScale = new Vector2(scale, scale);
    }
    
    
    public void Initialize(int teamId)
    {
        /*
        string teamName = teams[teamId].name;
        int money = teams[teamId].Money;
        float goodAnswers = teams[teamId].goodAnswers;
        float wrongAnswers = teams[teamId].wrongAnswers;
        float questions = goodAnswers + wrongAnswers;
        int cluesUsed = teams[teamId].CluesUsed;
        */

        //usunac
        string teamName = "Test team";
        int money = 100000;
        float goodAnswers = 3;
        float wrongAnswers = 6;
        float questions = goodAnswers + wrongAnswers;
        int cluesUsed = 5;

        teamNameObj.text = teamName;
        moneyObj.text = $"<color=green>{money.ToString()}</color>z�";
        accuracyObj.text = $"<size=22>Poprawne odpowiedzi\n</size>" +
            $"<size=60><color=red>{Mathf.Round((goodAnswers / questions) * 100f).ToString()}%\n</color></size>" +
            $"<size=18>({goodAnswers.ToString()}/{questions.ToString()})</size>";
        cluesUsedObj.text = $"U�yte podpowiedzi\n" +
            $"</size><size=60><color=orange>{cluesUsed.ToString()}";
    }
    
}
