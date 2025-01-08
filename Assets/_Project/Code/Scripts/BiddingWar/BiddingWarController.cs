using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Klasa zarządzająca bitwą licytacyjną pomiędzy drużynami.
/// </summary>
public class BiddingWarController : NetworkBehaviour
{
    #region variables
    /// <summary>
    /// Lista przechowująca nazwy drużyn jako tekst.
    /// </summary>
    public List<TextMeshProUGUI> teamNamesText;
    /// <summary>
    /// Lista przechowująca kwotę licytacji drużyny jako tekst.
    /// </summary>
    public List<TextMeshProUGUI> teamBidText;
    /// <summary>
    /// Lista przechowująca tekst z saldem całkowitym drużyny.
    /// </summary>
    public List<TextMeshProUGUI> teamBalanceText;
    /// <summary>
    /// Lista tekstów wyświetlanych na przyciskach licytacji, umożliwiająca dynamiczną aktualizację wartości ofert na przyciskach.
    /// </summary>
    public List<TextMeshProUGUI> bidButtonText;
    /// <summary>
    /// Obiekt wyświetlający czas w grze.
    /// </summary>
    public TextMeshProUGUI timerText;
    /// <summary>
    /// Obiekt wyświetlający nazwę kategorii.
    /// </summary>
    public TextMeshProUGUI categoryNameText;
    /// <summary>
    /// Obiekt wyświetlający ostrzeżenie.
    /// </summary>
    public TextMeshProUGUI warningText;
    /// <summary>
    /// Obiekt wyświetlający karę za brak aktywności.
    /// </summary>
    public TextMeshProUGUI punishmentText;
    /// <summary>
    /// Lista przechowująca drużyny.
    /// </summary>
    public List<TeamManager> teams;
    /// <summary>
    /// Obiekt wyświetlający ilość pieniedzy w puli.
    /// </summary>
    public TextMeshProUGUI totalBidText;
    /// <summary>
    /// Zmienna ilości pieniedzy w puli.
    /// </summary>
    int totalBid;
    /// <summary>
    /// Zmienna czasomierzu.
    /// </summary>
    float timer;
    /// <summary>
    /// Zmienna czasu, który został dany na udzielenie odpowiedzi.
    /// </summary>
    float timeGiven = 5f;

    /// <summary>
    /// Zmienna identyfikatora wygranej drużyny.
    /// </summary>
    int winningTeamID = 0;
    /// <summary>
    /// Enum odpowiadający kolorowy wygranej drużyny.
    /// </summary>
    ColourEnum winningTeamColour;
    int winningBidAmount = 500;
    bool hasSetUp = false;
    bool gameOngoing = false;
    private int defaultSceneChangeDelay = 5;
    //przyciski kolejno mają wartość: 100,200,300,400,500,1000zł
    //no i va banque
    //wartość przycisku= wartość o jaką drużyna *przebija stawkę*
    //możnaby też zrobić z każdego przycisku oddzielny var ale imo tak jest ładniej.
    /// <summary>
    /// Lista przycisków licytacyjnych z określoną kwotą.
    /// </summary>
    public List<Button> bidButtons;
    public List<Button> lockOutButtons;
    public Button vbButton;
    /// <summary>
    /// Przycisk odpowiadający za wyjście.
    /// </summary>
    public Button exitButton;
    /*
    ///make it so each event removes itself by using the id
    List<Timer> Active_Timers
    public delegate void My_Timer_Delegate(int )
    */
    /// <summary>
    /// Zmienna przechowująca ilość drużyn w grze.
    /// </summary>
    private uint _teamsInGame;
    //used for storing information about which team this instance of the program represents
    private uint localTeamId;
    public GameObject uderzenieImage;
    public VideoPlayer uderzenieVideoPlayer;


    public class Timer
    {
        float StartTime;
        float DesiredGap;
    }
    #endregion variables
    #region disconnection_handling

    #endregion disconnection_handling
    #region setup_functions
    void Start()
    {

        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();

        localTeamId = (uint)NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<TeamManager>().Colour;

        if (teams.Count < GameManager.Instance.StartingTeamCount.Value)
        {
            //drugi gracz będzie pierwszy na liście, jeżeli nie ma gracza pierwszego (a więc drugi gracz jest pierwszy na liście)
            if (localTeamId == 1 && NetworkManager.Singleton.ConnectedClients[0].ClientId != NetworkManager.Singleton.LocalClientId) { localTeamId = 0; }
            //wyjaśnienie: jeżeli jest mniej graczy niż było na początku, to trzeci gracz zawsze znajdzie się drugi na liście
            if (localTeamId == 2)
            {
                localTeamId = 1;
            }
        }

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<TeamManager>().TeamId = localTeamId;

        if (teams.Count < 4)
        {
            totalBidText.transform.position = teamBalanceText[GameManager.Instance.StartingTeamCount.Value].transform.position;
        }

        int i = GameManager.Instance.StartingTeamCount.Value;
        while (i < teamNamesText.Count)
        {
            Destroy(teamNamesText[i]);
            Destroy(teamBidText[i]);
            Destroy(teamBalanceText[i]);
            i += 1;
        }

        timerText.text = "5";
        categoryNameText.text = GameManager.Instance.Category.Value.Name.ToUpper();
        Setup();
        AddListeners();
    }

    void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnection;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnection;
    }

    /// <summary>
    /// Metoda przypisująca określoną wartość pieniędzy do danego przycisku.
    /// </summary>
    public void AddListeners()
    {
        bidButtons[0].onClick.AddListener(delegate { Bid(100); });
        bidButtons[1].onClick.AddListener(delegate { Bid(200); });
        bidButtons[2].onClick.AddListener(delegate { Bid(300); });
        bidButtons[3].onClick.AddListener(delegate { Bid(400); });
        bidButtons[4].onClick.AddListener(delegate { Bid(500); });
        bidButtons[5].onClick.AddListener(delegate { Bid(1000); });
        vbButton.onClick.AddListener(VaBanque);
        exitButton.onClick.AddListener(delegate { NetworkManager.Shutdown(); });

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientId"></param>
    private void HandleDisconnection(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            return;
        }

        int disconnectedIndex = teams.FindIndex(team => team.NetworkId == clientId);

        Destroy(teamNamesText[disconnectedIndex]);
        Destroy(teamBidText[disconnectedIndex]);
        Destroy(teamBalanceText[disconnectedIndex]);
    }

    /// <summary>
    /// Metoda ustawiająca interfejs do wyświetlania aktualnych informacji o drużynach w trakcie gry.
    /// </summary>
    void Setup()
    {
        totalBid = GameManager.Instance.CurrentBid.Value;
        totalBidText.text = totalBid.ToString();
        foreach (TeamManager team in teams)
        {

            teamBalanceText[(int)team.Colour].text = team.Money.ToString();
            teamBidText[(int)team.Colour].text = team.Bid.ToString();
            teamNamesText[(int)team.Colour].text = team.TeamName.ToString();
            teamNamesText[(int)team.Colour].color = ColorHelper.ToUnityColor(team.Colour);
        }

        int i = GameManager.Instance.StartingTeamCount.Value;
        while (i < teamNamesText.Count)
        {
            Destroy(teamNamesText[i]);
            Destroy(teamBidText[i]);
            Destroy(teamBalanceText[i]);
            i += 1;
        }

        ResetTimer();
    }

    /// <summary>
    /// Metoda RPC przygotowująca etap 2 gry, aktualizując status drużyn, licytacje oraz kwoty pieniędzy.
    /// </summary>
    [Rpc(SendTo.Everyone)]
    void SetupStage2Rpc()
    {
        foreach (TeamManager t in teams)
        {
            if (t.InGame)
            {
                SetupLockOutButtons(t);
                totalBid += 500;
                if (IsHost)
                {
                    t.RaiseBid(500);
                }
            }

            UpdateMoneyStatusForTeam((int)t.TeamId);
        }

        totalBidText.text = totalBid.ToString();
        winningBidAmount = 500;
        hasSetUp = true;
        gameOngoing = true;
    }
    /// <summary>
    /// Metoda ustawiająca przyciski blokady licytacji dla drużyny, która przegrała.
    /// </summary>
    /// <param name="team">Drużyna, dla której przyciski blokady są konfigurowane.</param>
    public void SetupLockOutButtons(TeamManager team)
    {
        if (team.NetworkId != NetworkManager.Singleton.LocalClientId && team.InGame)
        {
            lockOutButtons[(int)team.Colour].enabled = true;
            lockOutButtons[(int)team.Colour].image.enabled = true;
            lockOutButtons[(int)team.Colour].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            lockOutButtons[(int)team.Colour].onClick.AddListener(delegate { LockOutBid((int)team.TeamId); });
        }
    }

    #endregion setup_functions
    #region updates
    /// <summary>
    /// Metoda zmieniająca ilość zgromadzonej gotówki  oraz kwotę licytacji dla drużyny.
    /// </summary>
    /// <param name="i">Indeks drużyny, której dane maja zostać zaktualizowane.</param>
    public void UpdateMoneyStatusForTeam(int i)
    {
        teamBalanceText[(int)teams[i].Colour].text = teams[i].Money.ToString();
        teamBidText[(int)teams[i].Colour].text = teams[i].Bid.ToString();
    }

    /// <summary>
    /// Metoda aktualizująca dane dla wszystkich drużyn.
    /// </summary>
    public void UpdateMoneyStatus()
    {
        foreach (TeamManager t in teams)
        {
            UpdateMoneyStatusForTeam((int)t.TeamId);
        }

        UpdateButtons();
        totalBidText.text = totalBid.ToString();
    }

    /// <summary>
    /// Metoda ustawiająca wartości dla przycisków.
    /// </summary>
    public void UpdateButtons()
    {
        bidButtonText[0].text = "100";
        bidButtonText[1].text = "200";
        bidButtonText[2].text = "300";
        bidButtonText[3].text = "400";
        bidButtonText[4].text = "500";
        bidButtonText[5].text = "1000";

        if (winningBidAmount != teams[(int)localTeamId].Bid)
        {
            int difference = winningBidAmount - teams[(int)localTeamId].Bid;

            bidButtonText[0].text += "(" + ( difference + 100 ).ToString() + ")";
            bidButtonText[1].text += "(" + ( difference + 200 ).ToString() + ")";
            bidButtonText[2].text += "(" + ( difference + 300 ).ToString() + ")";
            bidButtonText[3].text += "(" + ( difference + 400 ).ToString() + ")";
            bidButtonText[4].text += "(" + ( difference + 500 ).ToString() + ")";
            bidButtonText[5].text += "(" + ( difference + 1000 ).ToString() + ")";
        }
    }

    /// <summary>
    /// Metoda RPC, która aktualizuje licytacje dla wszystkich graczy.
    /// </summary>
    /// <param name="difference">Zmienna reprezentująca różnice między nową i poprzednia ofertą zaproponowaną przez drużynę.</param>
    /// <param name="winning_bid">Zmienna reprezentująca najwyższa ofertę.</param>
    /// <param name="winning_team_id">Identyfikator drużyny, która wygra licytację.</param>
    [Rpc(SendTo.Everyone)]
    public void UpdateBidsRpc(int difference, int winning_bid, int winning_team_id)
    {
        totalBid += difference;
        winningBidAmount = winning_bid;
        winningTeamID = winning_team_id;
        ResetTimer();
        UpdateMoneyStatus();
    }
    #endregion updates
    #region bidding_functions
    /// <summary>
    /// Metoda, któa blokuje możliwość złożenia oferty przez drużynę.
    /// </summary>
    /// <param name="locked_out_team">Indeks drużyny, która zostaje zablokowana z licytacji.</param>
    public void LockOutBid(int locked_out_team)
    {
        int amount = teams[locked_out_team].Money + teams[locked_out_team].Bid - teams[(int)localTeamId].Bid;
        Bid(amount);
    }
    /// <summary>
    /// Metoda umożliwiająca użycie w licytacji VaBanque.
    /// </summary>
    public void VaBanque()
    {
        int amount = teams[(int)localTeamId].Money + teams[(int)localTeamId].Bid - winningBidAmount;
        VaBanqueIncrementServerRpc((int)localTeamId);
        Bid(amount);
    }

    /// <summary>
    /// Metoda umożliwiająca podbicie stawki w bieżącej licytacji.
    /// </summary>
    /// <param name="amount">Zmienna przechowująca kwotę, którą drużyna chce dodać do licytacji.</param>
    public void Bid(int amount)
    {
        if (gameOngoing)
        {
            TeamBidRpc(localTeamId, amount);
        }
    }

    /// <summary>
    ///  Metoda RPC, która przetwarza na serwerze ofertę licytacyjną danej drużyny.
    /// </summary>
    /// <param name="teamid">Identyfikator drużyny, która składa ofertę licytacyjną.</param>
    /// <param name="amount">Kwota, którą drużyna chce dodać do licytacji.</param>
    [Rpc(SendTo.Server)]
    public void TeamBidRpc(ulong teamid, int amount)
    {
        int team_id = (int)teamid;
        int difference = winningBidAmount + amount - teams[team_id].Bid;
        //                                                                                                                    Z powodu wprowadzenia lekkiego opóźnienia, trzeba też sprawdzać czy ktoś o mniejszej liczbie pieniędzy nie zrobił va banque, ten sposób wydaje się rozwiązywać problem
        if (teams[team_id].Money >= difference && ( teams[team_id].Bid != winningBidAmount || winningBidAmount == 500 ) && !( teams[winningTeamID].Money == 0 && teams[winningTeamID].Bid != 0 ))
        {
            winningBidAmount += amount;
            teams[team_id].RaiseBid(difference);
            UpdateBidsRpc(difference, winningBidAmount, team_id);
            if (teams[team_id].Money == 0)
            {
                //przy niewywoływaniu Sell() wszystko było ok, tak samo jest teraz przy wprowadzeniu lekkiego, praktycznie niezauważalnego opóźnienia
                //zgaduję że NetworkVariable team.bid nie jest updatowany dostatecznie szybko i przez to updatebids rpc nie updatowało?
                //wydaje się to być dziwne ale takie zachowanie było tylko wtedy gdy to host va banqueował więc wydaje mi się być prawdopodobne
                //note: 0.001f jest już za krótkim czasem
                timer = Time.time - timeGiven + 0.06f;
                //Sell(team_id);
            }
        }
    }
    #endregion bidding_functions

    /// <summary>
    /// Metoda resetująca czasomierz.
    /// </summary>
    public void ResetTimer()
    {
        timer = Time.time;
    }

    /// <summary>
    /// Metoda sprzedająca drużynie pytanie.
    /// </summary>
    /// <param name="team_id">Zmienna przechowująca identyfikator drużyny, której zostanie sprzedane pytanie.</param>
    void Sell(int team_id)
    {
        timer = 0;
        SellRpc(team_id);
    }

    /// <summary>
    /// Metoda RPC, która przetwarza zakończenie rundy aukcji i decyduje o dalszym przebiegu gry.
    /// </summary>
    /// <param name="team_id">Identyfikator drużyny, która wygrała licytację.</param>
    [Rpc(SendTo.Everyone)]
    void SellRpc(int team_id)
    {
        int scene_change_delay = defaultSceneChangeDelay;
        gameOngoing = false;
        timerText.text = "Wygrywa drużyna " + teams[team_id].TeamName;
        timerText.color = ColorHelper.ToUnityColor(teams[team_id].Colour);

        ShowVideo();

        List<int> teams_warned = CheckForInactivity();
        if (teams_warned.Count > 0)
        {
            _ = StartCoroutine(PunishInactivity(teams_warned, 3));
            //jeżeli warnujemy/karzemy drużyny to potrzebują one dodatkowego czasu żeby to wszystko przeczytać
            scene_change_delay = 16;
        }

        List<int> losers = CheckForLosers(team_id);
        if (losers.Count >= 0)
        {
            if (scene_change_delay == 16) { scene_change_delay = 23; }
        }

        if (IsHost)
        {
            WonBidIncrementServerRpc(team_id);

            foreach (TeamManager team in teams)
            {
                team.ResetBid();
            }
        }
        //  yield return new WaitForSeconds(10f);
        if (IsServer)
        {
            GameManager.Instance.Winner.Value = teams[team_id].NetworkId;
        }

        if (teams[(int)localTeamId].VaBanque == 3)
        {
            teams[(int)localTeamId].BadgeList.UnlockBadge("Ryzykanci");
        }

        if (teams[(int)localTeamId].WonBid == 5)
        {
            teams[(int)localTeamId].BadgeList.UnlockBadge("Mistrzowie Aukcji");
        }

        if (GameManager.Instance.Category.Value.Name is "Czarna skrzynka" or "Podpowiedź")
        {
            if (IsHost)
            {
                GameManager.Instance.CurrentBid.Value = 0;
                if (GameManager.Instance.Category.Value.Name is "Czarna skrzynka")
                {
                    BlackBoxesIncrementServerRpc(team_id);
                }
                else
                {
                    CluesIncrementServerRpc(team_id);
                }
            }

            //teams[team_id].Money -= totalBid; //to chyba nie jest potrzebne, bo pieniądze są na bieżąco pobierane z konta podczas licytacji.
            _ = IsContinuingGamePossible()
                ? StartCoroutine(OpenSceneWithDelay("CategoryDraw", scene_change_delay))
                : StartCoroutine(OpenSceneWithDelay("Summary", scene_change_delay));

            if (teams[(int)localTeamId].BlackBoxes == 2)
            {
                teams[(int)localTeamId].BadgeList.UnlockBadge("Czarni Łowcy");
            }

            CheckBadgeUnlockRpc();
        }
        else
        {
            CheckBadgeUnlockRpc();
            _ = StartCoroutine(OpenSceneWithDelay("QuestionStage", scene_change_delay));
        }

        if (IsHost)
        {
            PassCurrentBidServerRpc(totalBid);
        }
    }
    void Update()
    {
        if (gameOngoing)
        {
            UpdateMoneyStatus();
            if (winningBidAmount != 500)
            {
                timerText.text = ( timeGiven - ( Time.time - timer ) ).ToString();
                if (Time.time - timer > timeGiven && NetworkManager.Singleton.IsHost)
                {

                    Sell(winningTeamID);

                }
            }
        }
        else
        {
            if (Time.time - timer > timeGiven && NetworkManager.Singleton.IsHost && !hasSetUp & NetworkManager.Singleton.IsHost)
            {
                SetupStage2Rpc();
            }
        }
    }
    /// <summary>
    /// Metoda do opóźnionego przeładowania sceny.
    /// </summary>
    /// <param name="name">Nazwa sceny, która ma zostać załadowana.</param>
    /// <param name="delay">Opóźnienie w sekundach przed załadowaniem sceny.</param>
    /// <returns></returns>
    private IEnumerator OpenSceneWithDelay(string name, int delay)
    {
        yield return new WaitForSeconds(delay);
        _ = NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    /// <summary>
    /// Metoda RPC, która ustawia bieżącą stawkę licytacji na serwerze.
    /// </summary>
    /// <param name="currentBid">Zmienna przechowująca aktualną stawke licytacji.</param>
    [ServerRpc(RequireOwnership = false)]
    private void PassCurrentBidServerRpc(int currentBid)
    {
        GameManager.Instance.CurrentBid.Value = currentBid;
    }

    #region loss_handling
    //IEnumerator
    /// <summary>
    /// Metoda sprawdzająca, które drużyny zostały uznane za przegrane na podstawie stanu ich pieniedzy 
    /// </summary>
    /// <param name="winner_id">Identyfikator drużyny, która wygrała dane pytanie lub rundę.</param>
    /// <returns>Lista identyfikatorów drużyn, któe zostały uznane za przegrane.</returns>
    public List<int> CheckForLosers(int winner_id)
    {
        List<int> losers = new();
        foreach (TeamManager team in teams)
        {
            if (team.InGame)
            {
                //jeżeli drużyna kończy z <600zł oraz [nie wygrała pytania, lub kategoria nie da jej pieniędzy] to kończy grę (jeżeli się w niej znajduje
                if (team.Money < 600 && ( ( team.TeamId != winner_id ) || ( GameManager.Instance.Category.Value.Name is "Czarna skrzynka" or "Podpowiedź" ) ))
                {
                    losers.Add((int)team.TeamId);
                    if (IsHost)
                    {
                        team.InGame = false;
                    }
                }
            }
        }

        return losers;
    }

    #endregion loss_handling

    #region inactivity_handling
    /// <summary>
    /// Metoda sprawdzająca, które drużyny są nieaktywne (w paru rundach) podczas licytacji.
    /// </summary>
    /// <returns>Lista identyfikatorów drużyn, które nie brały udziału w licytacji.</returns>
    public List<int> CheckForInactivity()
    {
        List<int> teams_warned = new();
        foreach (TeamManager team in teams)
        {
            if (team.Bid == 500 && team.InGame == true)
            {
                if (IsHost)
                { team.InactiveRounds += 1; }

                teams_warned.Add((int)team.TeamId);
            }
        }

        return teams_warned;
    }
    /// <summary>
    /// Metoda odpowiadająca za przydzielenie kary lub ostrzeżenia w zależności od nieaktywności w licytacjach.
    /// </summary>
    /// <param name="teams_warned">Lista drużyn, które zostały ostrzeżone za brak aktywności.</param>
    /// <param name="delay">Zmienna przechowująca czas opóźnienia przed przydzieleniem kar i ostrzeżeń.</param>
    /// <returns></returns>
    private IEnumerator PunishInactivity(List<int> teams_warned, float delay)
    {
        yield return new WaitForSeconds(delay);
        List<int> teams_punished = new();
        foreach (int i in teams_warned)
        {
            if (teams[i].InactiveRounds >= 2)
            {
                //może by tu dać jakiegoś angry ibisza?
                if (IsHost)
                {
                    teams[i].Money -= 500 * teams[i].InactiveRounds;
                }

                teams_punished.Add(i);
            }
        }
        //ostrzegamy
        if (teams_warned.Count == 1)
        {
            warningText.text = "Drużyna <color=" + teams[teams_warned[0]].Colour.ToString().ToLower() + ">" + teams[teams_warned[0]].teamName.Value + " </color> nie licytowała w tej rundzie! ";
        }

        if (teams_warned.Count > 1)
        {
            warningText.text = "Drużyny ";
            foreach (int i in teams_warned)
            {
                warningText.text += "<color=" + teams[i].Colour.ToString().ToLower() + ">" + teams[i].teamName.Value + "</color> ";
            }

            warningText.text += " nie licytowały w tej rundzie! ";
        }
        //karamy
        if (teams_punished.Count == 1)
        {
            punishmentText.text = "Drużyna <color=" + teams[teams_punished[0]].Colour.ToString().ToLower() + ">" + teams[teams_punished[0]].teamName.Value + " </color> została ukarana, 500zł za każdą pasywną rundę! ";
        }

        if (teams_punished.Count > 1)
        {
            warningText.text = "Drużyny ";
            foreach (int i in teams_punished)
            {
                punishmentText.text += "<color=" + teams[i].Colour.ToString().ToLower() + ">" + teams[i].teamName.Value + "</color> ";
            }

            punishmentText.text += " \n zostały ukarane, 500zł za każdą pasywną rundę! ";
        }
    }
    #endregion inactivity_handling
    /// <summary>
    /// Metoda sprawdzjąca czy drużyna pozostaje w grze i czy ma możliwość brania udziału w następnych licytacjach.
    /// </summary>
    /// <returns></returns>
    private bool IsContinuingGamePossible()
    {
        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();

        _teamsInGame = 0;
        foreach (TeamManager team in teams)
        {
            //tu było >=500 ale drużyna z 500zł nie może przebić stawki więc przegrywa,
            if (team.Money > 500)
            {
                _teamsInGame++;
            }
            else
            {
                team.InGame = false;
            }
        }

        return _teamsInGame >= 2;
    }
    /// <summary>
    /// Metoda pokazująca filmik.
    /// </summary>
    private void ShowVideo()
    {
        _ = new WaitForSeconds(0.5f);
        uderzenieImage.SetActive(true);
        uderzenieVideoPlayer.Play();
    }

    /// <summary>
    /// Metoda RPC wysyłana na serwer, któa zwieksza liczbę wygranych licytacji przez drużynę.
    /// </summary>
    /// <param name="teamid">Zmienna przechowująca identyfikator drużyny.</param>
    [Rpc(SendTo.Server)]
    private void WonBidIncrementServerRpc(int teamid)
    {
        teams[(int)teamid].WonBid++;
    }

    /// <summary>
    /// Metoda RPC wysyłana na serwer, zwiększająca ilość użyć VaBanque
    /// </summary>
    /// <param name="teamid">Zmienna przechowująca identyfikator drużyny.</param>
    [Rpc(SendTo.Server)]
    private void VaBanqueIncrementServerRpc(int teamid)
    {
        teams[(int)teamid].VaBanque++;
    }

    /// <summary>
    /// Metoda RPC wysyłana na serwer, zwiększająca ilość czarnych skrzynek.
    /// </summary>
    /// <param name="teamid">Zmienna przechowująca identyfikator drużyny.</param>
    [Rpc(SendTo.Server)]
    private void BlackBoxesIncrementServerRpc(int teamid)
    {
        teams[(int)teamid].BlackBoxes++;
    }

    /// <summary>
    /// Metoda RPC wysyłana na serwer, która zwieksza liczbę podpowiedzi.
    /// </summary>
    /// <param name="teamid">Zmienna przechowująca identyfikator drużyny.</param>
    [Rpc(SendTo.Server)]
    private void CluesIncrementServerRpc(int teamid)
    {
        teams[(int)teamid].Clues++;
    }

    /// <summary>
    /// Metoda RPC wysyłana klientom i hostowi, która odblokowuje odznakę dla drużyny, jeśli jej identyfikator odpowiada lokalnemu identyfikatorowi klienta.
    /// </summary>
    /// <param name="name">Zmienna przechowująca nazwę odznaki.</param>
    /// <param name="teamid">Zmienna przechowująca identyfikator drużyny, której ma zostać przypisana odznaka.</param>
    [Rpc(SendTo.ClientsAndHost)]
    private void UnlockBadgeRpc(string name, int teamid)
    {
        if (teamid == (int)NetworkManager.Singleton.LocalClientId)
        {
            teams[(int)GameManager.Instance.Winner.Value].BadgeList.UnlockBadge(name);
        }
    }

    /// <summary>
    /// Metoda RPC sprawdzająca, czy drużyna odblokowała określone odznaki, na podstawie spełnionych warunków.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    private void CheckBadgeUnlockRpc()
    {
        if (!teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.IsBadgeUnlocked("Mistrzowie Aukcji"))
        {
            if (teams[(int)NetworkManager.Singleton.LocalClientId].WonBid == 5)
            {
                UnlockBadgeRpc("Mistrzowie Aukcji", (int)NetworkManager.Singleton.LocalClientId);
            }
        }

        if (!teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.IsBadgeUnlocked("Ryzykanci"))
        {
            if (teams[(int)NetworkManager.Singleton.LocalClientId].VaBanque == 3)
            {
                UnlockBadgeRpc("Ryzykanci", (int)NetworkManager.Singleton.LocalClientId);
            }
        }

        if (!teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.IsBadgeUnlocked("Czarni Łowcy"))
        {
            if (teams[(int)NetworkManager.Singleton.LocalClientId].BlackBoxes == 2)
            {
                UnlockBadgeRpc("Czarni Łowcy", (int)NetworkManager.Singleton.LocalClientId);
            }
        }
    }
}
