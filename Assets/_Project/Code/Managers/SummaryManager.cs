using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;
    public List<TeamManager> teams;
    TeamManager richestTeam;
    ulong winnerId;

    private void Start()
    {
        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();
        richestTeam = teams.OrderByDescending(team => team.Money).FirstOrDefault();
        winnerId = richestTeam.OwnerClientId;

        if (NetworkManager.Singleton.LocalClientId == winnerId && teams[(int)NetworkManager.Singleton.LocalClientId].CluesUsed == 0)
        {
            UnlockBadge("Samodzielni Geniusze");
        }

        if (teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered == 0 && teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAsked > 0)
        {
            UnlockBadge("Mistrzowie pomyłek");
        }

        if (teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered == teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAsked && teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered > 0)
        {
            UnlockBadge("As opowiedzi");
        }

        if (teams[(int)NetworkManager.Singleton.LocalClientId].Money >= 19000)
        {
            UnlockBadge("Królowie skarbca");
        }

        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {

            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(teamClient.PlayerObject.GetComponent<TeamManager>());
        }
    }

    public void ChangeScene()
    {
        NetworkManager.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click

    private void UnlockBadge(string name)
    {
        teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge(name);
    }
}
