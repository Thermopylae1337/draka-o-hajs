using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa zarządzająca etapem podsumowania.
/// </summary>
public class SummaryManager : NetworkBehaviour
{
    /// <summary>
    /// Pole przechowujące referencję do prefabrykatu panelu, używane podczas tworzenia nowych instancji panelu w interfejsie użytkownika.
    /// </summary>
    [SerializeField] private GameObject panelPrefab;
    /// <summary>
    /// Zmienna reprezentująca siatkę.
    /// </summary>
    [SerializeField] private Transform grid;
    /// <summary>
    /// Zmienna przechowująca listę drużyn.
    /// </summary>
    [SerializeField] private TextMeshProUGUI teamDrawingText;
    /// <summary>
    /// Zmienna przechowywująca listę obiektów tekstowych potrzebnych do wyświetlania nagród z czarnej skrzynki.
    /// </summary>
    [SerializeField] private TextMeshProUGUI[] boxesText;
    /// <summary>
    /// Zmienna przechowywująca listę animacji czarnych skrzynek.
    /// </summary>
    [SerializeField] private VideoPlayer[] boxOpeningVideoPlayer;
    /// <summary>
    /// Zmienna przechowywująca obiekt, na którym wyświetlana jest animacja losowania.
    /// </summary>
    [SerializeField] private RawImage videoCanvas;
    /// <summary>
    /// Zmienna przechowywująca listę obiektów odpowiedzalnych za wyświetlanie tekstu nagród z czarnej skrzynki.
    /// </summary>
    [SerializeField] private GameObject[] prizesObjects;
    /// <summary>
    /// Zmienna przechowywująca listę drużyn.
    /// </summary>
    public List<TeamManager> teams;
    /// <summary>
    /// Obiekt przechowujący najbogatszą drużynę.
    /// </summary>
    TeamManager richestTeam;
    /// <summary>
    /// Zmienna przechowująca numer identyfikujący daną drużynę.
    /// </summary>
    ulong winnerId;
    /// <summary>
    /// Zmienna wykorzystywana do losowania nagród.
    /// </summary>
    private static readonly System.Random _random = new();
    /// <summary>
    /// Zmienna statyczna przechowywująca możliwe nagrody przedmiotowe.
    /// </summary>
    private static readonly string[] _badges = { "Samochód", "Ogórek" };
    /// <summary>
    /// Zmienna statyczna przechowywująca szanse na nagrody przedmiotowe.
    /// </summary>
    private static readonly double[] _badgeChances = { 0.1, 0.9 };
    /// <summary>
    /// Zmienna statyczna przechowywująca możliwe nagrody pieniężne.
    /// </summary>
    private static readonly int[] _moneyTiers = Enumerable.Range(0, 21).Select(i => i == 0 ? 1 : i * 500).ToArray();
    /// <summary>
    /// Zmienna statyczna przechowywujaca szanse na nagrody pienięzne.
    /// </summary>
    private static readonly double[] _moneyChances =
    {
        0.01, 0.09, 0.09, 0.09, 0.08, 0.07, 0.07, 0.07, 0.07, 0.06, 0.05, 0.05, 0.05,
        0.03, 0.03, 0.02, 0.02, 0.02, 0.01, 0.01, 0.01
    };
    /// <summary>
    /// Metoda wywoływana na samym początku włączenia skryptu.
    /// </summary>
    private void Start()
    {
        

        if (teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered == 0 && teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAsked > 0)
        {
                UpdateMoneyServerRpc();
            _ = StartCoroutine(HandleTeams());
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void UpdateMoneyServerRpc()
    {
        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();
        richestTeam = teams.OrderByDescending(team => team.Money).FirstOrDefault();
        winnerId = richestTeam.OwnerClientId;

        NetworkManager.ConnectedClients[winnerId].PlayerObject.GetComponent<TeamManager>().Money += GameManager.Instance.CurrentBid.Value;

    }
    /// <summary>
    /// Metoda zajmująca się obsługą drużyn na etapie podusmowania.
    /// </summary>
    /// <returns>Zwraca IEnumerator</returns>
    private IEnumerator HandleTeams()
    {
        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {

            TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();

            //test
            team.BlackBoxes = _random.Next(1, 4);
            Debug.Log(team.name);
            Debug.Log(team.BlackBoxes);

            if (team.BlackBoxes > 0)
            {
                CalculatePrizeServerRpc(clientId);
                yield return new WaitUntil(() => teamDrawingText.IsActive() == false);
                //yield return new WaitForSeconds(0.1f);
            }

            HandleBadgesClientRpc(clientId);
            SaveTeamClientRpc(clientId);
        }

        DeactivateVideoCanvasClientRpc();

        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {
            ulong clientId = teamClient.ClientId;
            CreatePanelClientRpc(clientId);
        }   
    }
    /// <summary>
    /// Rpc zajmujący się zapisem drużyny w leaderboardzie.
    /// </summary>
    /// <param name="clientId"></param>
    [ClientRpc]
    private void SaveTeamClientRpc(ulong clientId)
    {
        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();
        LeaderboardList leaderboard = new();
        leaderboard.Deserializuj();
        leaderboard.AddTeam(new LeaderboardTeam(team.TeamName, team.Money, team.BadgeList.Badges));
        leaderboard.Serializuj();
    }

        GameObject panelObject = Instantiate(panelPrefab, grid);
        Panel panel = panelObject.GetComponent<Panel>();
        panel.Initialize(team);
    }
    /// <summary>
    /// RPC odpowiedzalny za obliczanie i wyświetlanie nagród uzyskanych z czarnych skrzynek przez drużynę.
    /// </summary>
    /// <param name="clientId">Zmienna przechowywująca ID drużyny.</param>
    [ServerRpc(RequireOwnership = false)]
    private void CalculatePrizeServerRpc(ulong clientId)
    {
        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();

        PrizeData[] prizes = Enumerable.Range(0, team.BlackBoxes)
                                .Select(_ => DrawPrize(team))
                                .ToArray();

        HandleBlackBoxBadgesClientRpc(clientId, new PrizeDataList { prizes = prizes });
        DisplayPrizeClientRpc(team.TeamName, new PrizeDataList { prizes = prizes });

    }
    /// <summary>
    /// RPC odpowiedzialny za przyporządkowywanie tekstu nagród i odtwarzanie odpowiedniej animacji dla każdego z klientów.
    /// </summary>
    /// <param name="teamName">Zmienna przechowująca nazwę drużyny.</param>
    /// <param name="prizeDataList">Zmienna przechowująca informację o liście wylosowanych nagród.</param>
    [ClientRpc]
    private void DisplayPrizeClientRpc(string teamName, PrizeDataList prizeDataList)
    {
        PrizeData[] prizes = prizeDataList.prizes;
        teamDrawingText.text = $"Drużyna {teamName} losuje czarną skrzynkę:";

        if (prizes.Length == 1)
        {
            boxesText[0].text = GetPrizeText(prizes[0]);
            //Debug.Log(boxesText[0].text);
            _ = StartCoroutine(PlayVideo(0));
        }
        else if (prizes.Length == 2)
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
    /// <summary>
    /// Metoda odpowiedzialna za przygotowanie i wyświetlenie animacji otwierania czarnej skrzynki.
    /// </summary>
    /// <param name="index">Zmienna określająca indeks wyświrtlanej animacjii.</param>
    /// <returns></returns>
    private IEnumerator PlayVideo(int index)
    {
        VideoPlayer videoPlayer = boxOpeningVideoPlayer[index];

        videoPlayer.Play();
        teamDrawingText.gameObject.SetActive(true);

        yield return new WaitForSeconds((float)videoPlayer.length + 0.1f);

        ShowPrizeObjects(index);

        yield return new WaitForSeconds(4f);

        DeactivateTextClientRpc();
    }
    /// <summary>
    /// Rpc zajmujący się deaktywacją obiektów uczestniczących w animacji.
    /// </summary>
    [ClientRpc]
    private void DeactivateTextClientRpc()
    {
        foreach (GameObject obiekt in prizesObjects)
        {
            obiekt.gameObject.SetActive(false);
        }

        teamDrawingText.gameObject.SetActive(false);
    }
    /// <summary>
    /// Rpc zajmujący się deaktywacją płótna, na którym renderowane jest video.
    /// </summary>
    [ClientRpc]
    private void DeactivateVideoCanvasClientRpc()
    {
        videoCanvas.gameObject.SetActive(false);
    }
    /// <summary>
    /// Metoda odpowiedzialna za aktywację obiektów tesktowych biorących udział w animacji.
    /// </summary>
    /// <param name="index">Zmienna określająca indeks obiektu.</param>
    private void ShowPrizeObjects(int index)
    {
        prizesObjects[index].gameObject.SetActive(true);
    }
    /// <summary>
    /// Metoda odpowiadająca za odblokowywanie odznaki pochodzącej z czarnej skrzynki.
    /// </summary>
    /// <param name="clientId">Zmienna przechowywująca ID drużyny, która ma szansę odblokować odznakę.</param>
    /// <param name="prizeDataList">Zmienna przechowywująca nagrody wylosowane z czarnych skrzynek.</param>
    [ClientRpc]
    private void HandleBlackBoxBadgesClientRpc(ulong clientId, PrizeDataList prizeDataList)
    {
        TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();
        foreach (PrizeData item in prizeDataList.prizes)
        {
            if (item.money == 1)
            {
                team.BadgeList.UnlockBadge("Symboliczna złotówka");
            }

            if (item.badge == "Ogórek")
            {
                team.BadgeList.UnlockBadge("Łowcy ogórka");
            }

            if (item.badge == "Samochód")
            {
                team.BadgeList.UnlockBadge("Samochód");
            }

            if (item.money == 5000 && winnerId == clientId)
            {
                team.BadgeList.UnlockBadge("Nagroda + 5000zł");
            }

            if (item.money == 10000 && winnerId == clientId)
            {
                team.BadgeList.UnlockBadge("Nagroda + 10000zł");
            }
        }
    }
    /// <summary>
    /// Rpc odpowiadający za odblokowanie odznak przez drużynę po spełnieniu odpowiednich warunków.
    /// </summary>
    /// <param name="clientId">Zmienna przechowywująca ID drużyny, która ma szansę odblokować odznakę.</param>
    [ClientRpc]
    private void HandleBadgesClientRpc(ulong clientId)
    {
            TeamManager team = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<TeamManager>();
            teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();
            richestTeam = teams.OrderByDescending(team => team.Money).FirstOrDefault();
            winnerId = richestTeam.OwnerClientId;
            
            if (clientId == winnerId && team.CluesUsed == 0)
            {
                team.BadgeList.UnlockBadge("Samodzielni Geniusze");
            }

            if (team.QuestionsAnswered == 0 && team.QuestionsAsked > 0)
            {
                team.BadgeList.UnlockBadge("Mistrzowie pomyłek");
            }

            if (team.QuestionsAnswered == team.QuestionsAsked && team.QuestionsAnswered > 0)
            {
                team.BadgeList.UnlockBadge("As opowiedzi");
            }

            if (team.Money >= 19000)
            {
                team.BadgeList.UnlockBadge("Królowie skarbca");
            }

            if(team.CzasToPieniadz == true)
            {
                team.BadgeList.UnlockBadge("Czas to pieniądz");
            }

            if (team.Bankruci == true)
            {
                team.BadgeList.UnlockBadge("Bankruci");
            }

            if (team.WonBid >= 5)
            {
                team.BadgeList.UnlockBadge("Mistrzowie Aukcji");
            }

            if (team.VaBanque >= 3)
            {
                team.BadgeList.UnlockBadge("Ryzykanci");
            }

            if (team.BlackBoxes >= 2)
            {
                team.BadgeList.UnlockBadge("Czarni Łowcy");
            }
    }
    /// <summary>
    /// Metoda zajmująca się deaktywacją obiektów uczestniczących w animacji.
    /// </summary>
    private void DeactivateAll()
    {
        foreach (GameObject obiekt in prizesObjects)
        {
            obiekt.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Metoda odpowiedzialna za logikę losowania między nagrodą pieniężną a odznaką.
    /// </summary>
    /// <param name="team">Zmienna reprezentująca drużynę.</param>
    /// <returns>Zwraca informacje o nagrodzie jaką wylosowała drużyna.</returns>
    private PrizeData DrawPrize(TeamManager team)
    {
        bool isMoneyPrize = _random.NextDouble() < 0.9;
        return isMoneyPrize ? CreateMoneyPrize(team) : CreateBadgePrize(team);
    }
    /// <summary>
    /// Metoda odpowiedzialna za losowanie nagrody w postaci pieniędzy.
    /// </summary>
    /// <param name="team">Zmienna reprezentująca drużynę.</param>
    /// <returns>Zwraca nagrodę w postaci pieniędzy.</returns>
    private PrizeData CreateMoneyPrize(TeamManager team)
    {
        int money = DrawFromProbability(_moneyTiers, _moneyChances);
        team.Money += money;

        return new PrizeData { teamName = team.name, money = money, badge = string.Empty };
    }
    /// <summary>
    /// Metoda odpowiedzialna za losowanie nagrody będącą odznaką.
    /// </summary>
    /// <param name="team">Zmienna reprezentująca drużynę.</param>
    /// <returns>Zwraca nagrodę w postaci odznaki.</returns>
    private PrizeData CreateBadgePrize(TeamManager team)
    {
        string badge = DrawFromProbability(_badges, _badgeChances);
        return new PrizeData { teamName = team.name, money = 0, badge = badge };
    }
    /// <summary>
    /// Metoda odpowiedzialna za losowanie nagrody z czarnej skrzynki.
    /// </summary>
    /// <typeparam name="T">Parametr typu listy nagród.</typeparam>
    /// <param name="items">Zmienna przechowywująca nagrody.</param>
    /// <param name="probabilities">Zmienna przechowywująca prawdopodobieństwa wylosowania poszczególnych nagród.</param>
    /// <returns>Zwraca wylosowaną nagrodę.</returns>
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
    /// <summary>
    /// Metoda zajmująca się logiką generowania tekstu wyświetlanego podczas animacji czarnej skrzynki.
    /// </summary>
    /// <param name="prize">Zmienna przechowywująca informację o wylosowanej przez drużynę nagrody.</param>
    /// <returns>Metoda zwraca tekst jaki ma się pojawić podczas animacji czarnej skrzynki.</returns>
    private string GetPrizeText(PrizeData prize)
    {
        return prize.money > 0 ? prize.money.ToString() : prize.badge;
    }
    /// <summary>
    /// Metoda zajmująca się oczysczaniem tekstury na której renderowana jest animacja.
    /// </summary>
    /// <param name="videoPlayer">Zmienna przechowywująca informację o wyświetlonej animacji.</param>
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
    /// <summary>
    /// Metoda zwalniająca zasoby sprzętowe używane przez teksturę renderingu.
    /// </summary>
    /// <param name="videoPlayer">Zmienna przechowywująca informację o wyświetlonej animacji.</param>
    private void ReleaseVideoTexture(VideoPlayer videoPlayer)
    {
        videoPlayer.targetTexture?.Release();
    }
    /// <summary>
    /// Metoda pozwalająca na przejście do menu głownego gry.
    /// </summary>
    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click

    /// <summary>
    /// Odblokowuje odznakę o podanej nazwie dla drużyny gracza. Gracz jest identyfikowany za pomocą jego unikalnego identyfikatora (LocalClientId).
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki.</param>
    private void UnlockBadge(string name)
    {
        teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge(name);
    }
}

