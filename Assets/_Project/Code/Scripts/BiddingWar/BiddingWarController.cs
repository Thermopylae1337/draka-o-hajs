using Assets._Project.Code.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class BiddingWarController : NetworkBehaviour
{
    public List<TextMeshProUGUI> teamNamesText;
    public List<TextMeshProUGUI> teamBidText;
    public List<TextMeshProUGUI> teamBalanceText;
    public List<TextMeshProUGUI> bidButtonText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI categoryNameText;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI punishmentText;
    public List<TeamManager> teams;
    public TextMeshProUGUI totalBidText;
    int totalBid;
    float timer;
    float timeGiven = 5f;

    int winningTeamID = 0;
    ColourEnum winningTeamColour;
    int winningBidAmount = 0;
    bool hasSetUp = false;
    bool gameOngoing = false;
    private int defaultSceneChangeDelay = 5;
    //przyciski kolejno mają wartość: 100,200,300,400,500,1000zł
    //no i va banque
    //wartość przycisku= wartość o jaką drużyna *przebija stawkę*
    //możnaby też zrobić z każdego przycisku oddzielny var ale imo tak jest ładniej.
    public List<Button> bidButtons;
    public Button vbButton;
    public Button exitButton;
    /*
    ///make it so each event removes itself by using the id
    List<Timer> Active_Timers
    public delegate void My_Timer_Delegate(int )
    */
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

    public void ExitToLobby()
    {
        DisconnectPlayerRpc(NetworkManager.Singleton.LocalClientId,  localTeamId);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    [Rpc(SendTo.Server)]
    public void DisconnectPlayerRpc(ulong networkid, uint playerid)
    {
        NetworkManager.Singleton.DisconnectClient((ulong)networkid);

    }
    void Start()
    {
        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();
        localTeamId = (uint)NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<TeamManager>().Colour;
        if (teams.Count < 4)
        {
            totalBidText.transform.position = teamBalanceText[teams.Count].transform.position;
        }

        int i = teams.Count;
        while (i < teamNamesText.Count)
        {
            Destroy(teamNamesText[i]);
            Destroy(teamBidText[i]);
            Destroy(teamBalanceText[i]);
            i += 1;
        }

        categoryNameText.text = GameManager.Instance.Category.Value.Name.ToUpper();
        timerText.text = "5";
        Setup();
        AddListeners();
    }

    public void AddListeners()
    {
        bidButtons[0].onClick.AddListener(delegate { Bid(100); });
        bidButtons[1].onClick.AddListener(delegate { Bid(200); });
        bidButtons[2].onClick.AddListener(delegate { Bid(300); });
        bidButtons[3].onClick.AddListener(delegate { Bid(400); });
        bidButtons[4].onClick.AddListener(delegate { Bid(500); });
        bidButtons[5].onClick.AddListener(delegate { Bid(1000); });
        vbButton.onClick.AddListener(delegate { VaBanque(); });
        exitButton.onClick.AddListener(delegate { ExitToLobby(); });
    }

    void Setup()
    {
        int i = 0;
        while (i < teams.Count)
        {
            teamBalanceText[i].text = teams[i].Money.ToString();
            teamBidText[i].text = teams[i].Bid.ToString();
            teamNamesText[i].text = teams[i].TeamName.ToString();
            teamNamesText[i].color = ColorHelper.ToUnityColor(teams[i].Colour);
            i += 1;
        }

        while (i < teamNamesText.Count)
        {
            Destroy(teamNamesText[i]);
            Destroy(teamBidText[i]);
            Destroy(teamBalanceText[i]);
            i += 1;
        }

        ResetTimer();
    }

    [Rpc(SendTo.Everyone)]
    void SetupStage2Rpc()
    {
        int i = 0;
        while (i < teams.Count)
        {
            if (IsHost)
            {
                teams[i].RaiseBid(500);
            }

            UpdateMoneyStatusForTeam(i);
            i += 1;
            winningBidAmount = 500;
            totalBid = teams.Count * 500;
            totalBidText.text = totalBid.ToString();
        }

        hasSetUp = true;
        gameOngoing = true;
    }

    public void UpdateMoneyStatusForTeam(int i)
    {
        teamBalanceText[i].text = teams[i].Money.ToString();
        teamBidText[i].text = teams[i].Bid.ToString();
    }

    public void UpdateMoneyStatus()
    {
        int i = 0;
        while (i < teams.Count)
        {
            UpdateMoneyStatusForTeam(i);
            i += 1;
        }

        UpdateButtons();
        totalBidText.text = totalBid.ToString();
    }

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
    public void VaBanque()
    {
        int amount = teams[(int)localTeamId].Money + teams[(int)localTeamId].Bid - winningBidAmount;
        Bid(amount);
    }

    public void Bid(int amount)
    {
        if (gameOngoing)
        {
            TeamBidRpc(localTeamId, amount);
        }
    }

    [Rpc(SendTo.Server)]
    public void TeamBidRpc(ulong teamid, int amount)
    {
        int team_id = (int)teamid;
        int difference = winningBidAmount + amount - teams[team_id].Bid;
        if (( teams[team_id].Money >= difference && teams[team_id].Bid != winningBidAmount ) || ( teams[team_id].Money >= difference && winningBidAmount == 500 ))
        {
            winningBidAmount += amount;
            teams[team_id].RaiseBid(difference);
            UpdateBidsRpc(difference, winningBidAmount, team_id);
            if (teams[team_id].Money == 0)
            {
                Sell(team_id);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateBidsRpc(int difference, int winning_bid, int winning_team_id)
    {
        totalBid += difference;
        winningBidAmount = winning_bid;
        winningTeamID = winning_team_id;
        ResetTimer();
        UpdateMoneyStatus();
    }

    public void ResetTimer()
    {
        timer = Time.time;
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
    void Sell(int team_id)
    {
        timer = 0;
        SellRpc(team_id);
    }

    [Rpc(SendTo.Everyone)]
    void SellRpc(int team_id)
    {

        int scene_change_delay = defaultSceneChangeDelay;
        gameOngoing = false;
        timerText.text = "Wygrywa drużyna " + teams[team_id].TeamName;
        timerText.color = ColorHelper.ToUnityColor(teams[team_id].Colour);

        ShowVideo();

        List<int> teams_warned= CheckForInactivity();
        if (teams_warned.Count > 0) {
            StartCoroutine( PunishInactivity(teams_warned,3));
            scene_change_delay = 30;
            //jeżeli warnujemy/karzemy drużyny to potrzebują one dodatkowego czasu żeby to wszystko przeczytać


                }
       //yield return new WaitForSeconds(10f);
          
        
        Debug.Log("post post post");
        if (IsHost)
        {
            foreach (TeamManager team in teams)
            {
                team.ResetBid();
            }
        }
        //  yield return new WaitForSeconds(10f);    
        if (IsServer)
        {
            GameManager.Instance.Winner.Value = (uint)team_id;
        }

        if (GameManager.Instance.Category.Value.Name is "Czarna skrzynka" or "Podpowiedź")
        {
            //teams[team_id].Money -= totalBid; //to chyba nie jest potrzebne, bo pieniądze są na bieżąco pobierane z konta podczas licytacji.
            if (GameManager.Instance.Category.Value.Name is "Czarna skrzynka")
            {
                teams[team_id].BlackBoxes += 1;
            }
            else
            {
                teams[team_id].Clues += 1;
            }

            if (IsContinuingGamePossible())
            {
                StartCoroutine(OpenSceneWithDelay("CategoryDraw",scene_change_delay));
            }
            else
            {
                StartCoroutine(OpenSceneWithDelay("Summary",scene_change_delay));
            }
        }
        else
        {
            if (IsHost)
            {
                PassCurrentBidServerRpc(totalBid);
            }

            StartCoroutine(OpenSceneWithDelay("QuestionStage",scene_change_delay));
        }
         
    }
    private IEnumerator OpenSceneWithDelay(string name, int delay)
    {
        yield return new WaitForSeconds(delay);
        NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PassCurrentBidServerRpc(int currentBid)
    {
        GameManager.Instance.CurrentBid.Value += currentBid;
    }

    public List<int> CheckForInactivity()
    {
        List<int> teams_warned = new();
        foreach (TeamManager team in teams)
        {
            if (team.Bid == 500)
            {
                if (IsHost)
                { team.InactiveRounds += 1; }
                teams_warned.Add((int)team.Colour); 
            }
        }

            return teams_warned;
    }

    private IEnumerator PunishInactivity(List<int> teams_warned, float delay )
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
            warningText.text = "Drużyna <color=" + teams[teams_warned[0]].Colour.ToString().ToLower() + ">" + teams[teams_warned[0]].name + " </color> nie licytowała w tej rundzie! ";
        }
        if (teams_warned.Count > 1)
        {
            warningText.text = "Drużyny ";
            foreach (int i in teams_warned)
            {
                warningText.text += "<color=" + teams[i].Colour.ToString().ToLower() + ">" + teams[i].name + "</color> ";
            }
            warningText.text += " nie licytowały w tej rundzie! ";
        }
        //karamy
        if (teams_punished.Count == 1)
        {
            warningText.text = "Drużyna <color=" + teams[teams_warned[0]].Colour.ToString().ToLower() + ">" + teams[teams_punished[0]].name + " </color> została ukarana, 500zł za każdą pasywną rundę! ";
        }
        if (teams_punished.Count > 1)
        {
            warningText.text = "Drużyny ";
            foreach (int i in teams_punished)
            {
                warningText.text += "<color=" + teams[i].Colour.ToString().ToLower() + ">" + teams[i].name + "</color> ";
            }
            warningText.text += " \n zostały ukarane, 500zł za każdą pasywną rundę! ";
        }
    }
    private bool IsContinuingGamePossible()
    {
        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();

        _teamsInGame = 0;
        foreach (TeamManager team in teams)
        {
            if (team.Money >= 500)
            {
                _teamsInGame++;
            }
        }

        return _teamsInGame >= 2;
    }
    private void ShowVideo()
    {
        _ = new WaitForSeconds(0.5f);
        uderzenieImage.SetActive(true);
        uderzenieVideoPlayer.Play();
    }
}
