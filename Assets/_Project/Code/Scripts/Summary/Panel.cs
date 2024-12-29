using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets._Project.Code.Models;

public class Panel : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> panelSprites = new List<Sprite>();              // wsadzać Y, G, B !
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
        if (team.Colour == ColourEnum.YELLOW)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Summary/tabelka_zolci");
        else if (team.Colour == ColourEnum.GREEN)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Summary/tabelka_zieloni");
        else
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Summary/tebelka_niebiescy");
        
        string teamName = team.TeamName;
        int money = team.Money;
        //float goodAnswers = team.goodAnswers;
        //float wrongAnswers = team.wrongAnswers;
        //float questions = team.goodAnswers + team.wrongAnswers;
        //int cluesUsed = team.CluesUsed;

        //temp
        //string teamName = "Test init team";
        //int money = 100000;
        float goodAnswers = 3;
        float wrongAnswers = 6;
        float questions = goodAnswers + wrongAnswers;
        int cluesUsed = 5;

        teamNameObj.text = teamName;
        moneyObj.text = $"<color=green>{money}</color>pln";
        accuracyObj.text = $"<size=22>Poprawne odpowiedzi\n</size>" +
            $"<size=60><color=red>{Mathf.Round(goodAnswers / questions * 100f)}%\n</color></size>" +
            $"<size=18>({goodAnswers}/{questions})</size>";
        cluesUsedObj.text = $"Wykorzystane podpowiedzi\n" +
            $"<size=60><color=orange>{cluesUsed}</size>";
    }
}
