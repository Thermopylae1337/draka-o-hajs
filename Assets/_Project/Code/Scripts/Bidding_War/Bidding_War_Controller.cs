using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bidding_War_Controller : NetworkBehaviour
{
    public List<TextMeshProUGUI> teamNamesText;
    public List<TextMeshProUGUI> teamBidText;
    public List<TextMeshProUGUI> teamBalanceText;
    public List<TextMeshProUGUI> bidButtonText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI categoryNameText;
    public List<TeamManager> teams;
    private GameManager gameManager;
    public TextMeshProUGUI totalBidText;
    int totalBid;
    float timer;
    float timeGiven = 5;
    int winningTeamID = 0;
    int winningBidAmount = 0;
    bool hasSetUp = false;
    bool gameOngoing = false;
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

    public class Timer
    {
        float StartTime;
        float DesiredGap;
    }

    public void ExitToLobby()
    {
        DisconnectPlayerRpc(NetworkManager.Singleton.LocalClientId);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    [Rpc(SendTo.Server)]
    public void DisconnectPlayerRpc(ulong playerid)
    {
        NetworkManager.Singleton.DisconnectClient((ulong)playerid);
    }
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        teams = gameManager.Teams;

        if (teams.Count < 4)
        {
            totalBidText.transform.position = teamBalanceText[teams.Count].transform.position;
            totalBidText.text = "Loading...";
        }

        int i = teams.Count;
        while (i < teamNamesText.Count)
        {
            Destroy(teamNamesText[i]);
            Destroy(teamBidText[i]);
            Destroy(teamBalanceText[i]);
            i += 1;
        }

        categoryNameText.text = gameManager.Category.Value.Name;
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
            teamNamesText[i].text = teams[i].Colour;
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
            teams[i].RaiseBid(500);
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

        if (winningBidAmount != teams[(int)NetworkManager.Singleton.LocalClientId].Bid)
        {
            int difference = winningBidAmount - teams[(int)NetworkManager.Singleton.LocalClientId].Bid;

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
        int amount = teams[(int)NetworkManager.Singleton.LocalClientId].Money + teams[(int)NetworkManager.Singleton.LocalClientId].Bid - winningBidAmount;
        Bid(amount);
    }

    public void Bid(int amount)
    {
        if (gameOngoing)
        {
            TeamBidRpc(NetworkManager.Singleton.LocalClientId, amount);
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
            UpdateBidsRpc(team_id, difference, winningBidAmount, team_id);
            if (teams[team_id].Money == 0)
            {
                Sell(team_id);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateBidsRpc(int team_id, int difference, int winning_bid, int winning_team_id)
    {
        teams[team_id].RaiseBid(difference);
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
        foreach (TeamManager t in teams)
        {
            t.ResetBid();
        }

        gameOngoing = false;
        timerText.text = "Wygrywa drużyna " + teams[team_id].Colour;
        //na razie team_id nie jest na nic potrzebne ale jest na później żeby można było w następnej scenie stwierdzić kto wygrał licytację

        if (IsServer)
        {
            GameManager.Instance.Winner.Value = (uint)team_id;
        }

        if (GameManager.Instance.Category.Value.Name is "Czarna skrzynka" or "Podpowiedź")
        {
            teams[team_id].Money -= totalBid;
            if(IsContinuingGamePossible())
            {
                StartCoroutine(OpenSceneWithDelay("CategoryDraw"));
            }
            else
            {
                StartCoroutine(OpenSceneWithDelay("Summary"));
            }
        }
        else
        {
            if (IsHost)
            {
                PassCurrentBidServerRpc(totalBid);
            }

            StartCoroutine(OpenSceneWithDelay("QuestionStage"));
        }
    }
    private IEnumerator OpenSceneWithDelay(string name)
    {
        yield return new WaitForSeconds(5);
        NetworkManager.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PassCurrentBidServerRpc(int currentBid)
    {
        GameManager.Instance.CurrentBid.Value += currentBid;
    }

    private bool IsContinuingGamePossible()
    {
        teams = GameObject.Find("GameManager").GetComponent<GameManager>().Teams;
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
}
