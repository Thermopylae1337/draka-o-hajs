using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSpawner : MonoBehaviour
{
    public GameObject leaderboardPrefab;
    public Transform contentParent;
    private void Start()
    {
        GenerateLeaderboard();
    }

    private void GenerateLeaderboard()
    {
        LeaderboardList leaderboard = new LeaderboardList();
        leaderboard.Deserializuj();
        List<LeaderboardTeam> teams = leaderboard.TeamList;
        teams.Sort();
        if (teams != null && teams.Count > 0)
        {
            int miejsce = 1;
            foreach (LeaderboardTeam item in teams)
            {
                GameObject leaderboardObject = Instantiate(leaderboardPrefab, contentParent);
                TextMeshProUGUI name = leaderboardObject.transform.Find("name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI money = leaderboardObject.transform.Find("money").GetComponent<TextMeshProUGUI>();
                name.text = miejsce+". "+item.Name;
                money.text = "" + item.Money;
                if (item.Money > 0)
                {
                    money.color = new Color(0.19f, 0.8f, 0.19f);
                }
                else
                {
                    money.color = new Color(1f, 0f, 0f);
                }
                Image background = leaderboardObject.GetComponent<Image>();
                if (miejsce == 1)
                {
                    background.color = new Color(1f, 0.84f, 0f);
                }
                else if (miejsce == 2)
                {
                    background.color = new Color(0.75f, 0.75f, 0.75f);
                }
                else if (miejsce == 3)
                {
                    background.color = new Color(0.72f, 0.45f, 0.2f);
                }
                else
                {
                    background.color = new Color(1f, 1f, 1f, 0.1f);
                }
                miejsce++;
            }
        }
        else
        {
            GameObject leaderboardObject = Instantiate(leaderboardPrefab, contentParent);
            TextMeshProUGUI name = leaderboardObject.transform.Find("name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI money = leaderboardObject.transform.Find("money").GetComponent<TextMeshProUGUI>();
            name.text = "Brak druzyn do wyswietlenia";
            money.text = "";
        }
    }
}
