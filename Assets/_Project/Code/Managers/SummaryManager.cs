using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;
using System;

public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;
    [SerializeField] private TextMeshProUGUI teamDrawingText;
    [SerializeField] private TextMeshProUGUI[] boxesText;
    [SerializeField] private VideoPlayer[] boxOpeningVideoPlayer;
    [SerializeField] private RawImage videoCanvas;
    [SerializeField] private GameObject[] prizesObjects;
    public List<TeamManager> teams;
    TeamManager richestTeam;
    ulong winnerId;

    private static readonly System.Random _random = new();
    private static readonly string[] _badges = { "Samochód", "Ogórek" };
    private static readonly double[] _badgeChances = { 0.2, 0.8 };
    private static readonly int[] _moneyTiers = Enumerable.Range(0, 21).Select(i => i == 0 ? 1 : i * 500).ToArray();
    private static readonly double[] _moneyChances =
    {
        0.01, 0.09, 0.09, 0.09, 0.08, 0.07, 0.07, 0.07, 0.07, 0.06, 0.05, 0.05, 0.05,
        0.03, 0.03, 0.02, 0.02, 0.02, 0.01, 0.01, 0.01
    };

    private void Start()
    {
        if (NetworkManager != null)
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
        }

        if (NetworkManager.Singleton?.IsHost == true)
        {
            _ = StartCoroutine(HandleTeams());
        }
    }

    private IEnumerator HandleTeams()
    {
        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {
            ulong clientId = teamClient.ClientId;

            
            TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();

            //test
            team.BlackBoxes = _random.Next(4);
            Debug.Log(team.name);
            Debug.Log(team.BlackBoxes);

            if (team.BlackBoxes > 0)
            {
                CalculatePrizeServerRpc(clientId);
                yield return new WaitUntil(() => videoCanvas.gameObject.activeSelf == false);
            }

            CreatePanelClientRpc(clientId);

            yield return new WaitForSeconds(2f);
        }
    }

    [ClientRpc]
    private void CreatePanelClientRpc(ulong clientId)
    {
        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();

        GameObject panelObject = Instantiate(panelPrefab, grid);
        Panel panel = panelObject.GetComponent<Panel>();
        panel.Initialize(team);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CalculatePrizeServerRpc(ulong clientId)
    {
        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();

        
        PrizeData[] prizes = Enumerable.Range(0, team.BlackBoxes)
                                .Select(_ => DrawPrize(team))
                                .ToArray();

        HandleBlackBoxBadges(clientId, new PrizeDataList { prizes = prizes });
        DisplayPrizeClientRpc(new PrizeDataList { prizes = prizes });
        
    }

    [ClientRpc]
    private void DisplayPrizeClientRpc(PrizeDataList prizeDataList)
    {
        PrizeData[] prizes = prizeDataList.prizes;
        teamDrawingText.text = $"Drużyna {prizes[0].teamName} losuje czarną skrzynkę:";

        if (prizes.Length == 1)
        {
            boxesText[0].text = GetPrizeText(prizes[0]);
            //Debug.Log(boxesText[0].text);
            _ = StartCoroutine(PlayVideo(0));
        }
        else if(prizes.Length == 2)
        {
            boxesText[1].text = GetPrizeText(prizes[0]);
            boxesText[2].text = GetPrizeText(prizes[1]);
            //Debug.Log(boxesText[1].text +" "+ boxesText[2].text);
            _ = StartCoroutine(PlayVideo(1));
        }
        else
        {
            boxesText[3].text = GetPrizeText(prizes[0]);
            boxesText[4].text = GetPrizeText(prizes[1]);
            boxesText[5].text = GetPrizeText(prizes[2]);
            //Debug.Log(boxesText[3].text + " " + boxesText[4].text + " "+ boxesText[5].text);
            _ = StartCoroutine(PlayVideo(2));
        }
    }

    private IEnumerator PlayVideo(int index)
    {
        VideoPlayer videoPlayer = boxOpeningVideoPlayer[index];
        ClearVideoTexture(videoPlayer);
        videoCanvas.gameObject.SetActive(true);

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);

        teamDrawingText.gameObject.SetActive(true);
        videoPlayer.Play();

        yield return new WaitForSeconds((float)videoPlayer.length + 0.1f);

        ShowPrizeObjects(index);

        yield return new WaitForSeconds(5f);

        DeactivateAll();
        ReleaseVideoTexture(videoPlayer);
    }

    private void ShowPrizeObjects(int index)
    {
        prizesObjects[index].gameObject.SetActive(true);
    }

    private void HandleBlackBoxBadges(ulong clientId, PrizeDataList prizeDataList)
    {
        foreach (PrizeData item in prizeDataList.prizes)
        {
            if(item.money == 1)
            {
                teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge("Symboliczna złotówka");
            }

            if( item.badge == "Ogórek")
            {
                teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge("Łowcy ogórka");
            }

            if(item.badge == "Samochód")
            {
                teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge("Samochód");
            }

            if( item.money == 5000 && winnerId == clientId)
            {
                teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge("Nagroda + 5000zł");
            }

            if(item.money == 10000 && winnerId == clientId)
            {
                teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge("Nagroda + 10000zł");
            }
        }
    }

    private void DeactivateAll()
    {
        foreach (GameObject obiekt in prizesObjects)
        {
            obiekt.gameObject.SetActive(false);
        }

        teamDrawingText.gameObject.SetActive(false);
        videoCanvas.gameObject.SetActive(false);
    }

    private PrizeData DrawPrize(TeamManager team)
    {
        bool isMoneyPrize = _random.NextDouble() < 0.8;
        return isMoneyPrize ? CreateMoneyPrize(team) : CreateBadgePrize(team);
    }

    private PrizeData CreateMoneyPrize(TeamManager team)
    {
        int money = DrawFromProbability(_moneyTiers, _moneyChances);
        team.Money += money;

        return new PrizeData { teamName = team.name, money = money, badge = string.Empty };
    }

    private PrizeData CreateBadgePrize(TeamManager team)
    {
        string badge = DrawFromProbability(_badges, _badgeChances);
        return new PrizeData { teamName = team.name, money = 0, badge = badge };
    }

    private T DrawFromProbability<T>(T[] items, double[] probabilities)
    {
        double randomValue = _random.NextDouble();
        double cumulativeProbability = 0;

        for (int i = 0; i < items.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return items[i];
            }
        }

        return items.Last();
    }

    private string GetPrizeText(PrizeData prize)
    {
        return prize.money > 0 ? prize.money.ToString() : prize.badge;
    }

    private void ClearVideoTexture(VideoPlayer videoPlayer)
    {
        if (videoPlayer.targetTexture != null)
        {
            RenderTexture renderTexture = videoPlayer.targetTexture;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }
    }

    private void ReleaseVideoTexture(VideoPlayer videoPlayer)
    {
        videoPlayer.targetTexture?.Release();
    }

    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click

    private void UnlockBadge(string name)
    {
        teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge(name);
    }
}
