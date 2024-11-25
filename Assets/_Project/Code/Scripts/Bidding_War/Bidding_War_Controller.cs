using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bidding_War_Controller : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject obj;
    NetworkManager netMan;
    public List<TextMeshProUGUI> teamNamesText;
    public List<TextMeshProUGUI> teamBidText;
    public List<TextMeshProUGUI> teamBalanceText;
    public List<TextMeshProUGUI> bidButtonText;
    public TextMeshProUGUI timerText;
    public List<Team> teams;
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

    public class Timer
    {
        float StartTime;
        float DesiredGap;
    }

    public void Exit_To_Lobby()
    {
        Disconnect_Player_Rpc(NetworkManager.Singleton.LocalClientId);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    [Rpc(SendTo.Server)]
    public void Disconnect_Player_Rpc(ulong playerid)
    {
        NetworkManager.DisconnectClient((ulong)playerid);
    }
    void Start()
    {
        teams = General_Game_Data.Teams;
        if (!NetworkManager.Singleton.IsHost)
        {
            netMan.StartClient();
        }
        else
        {
            netMan.StartHost();
        }
        if (teams.Count < 4)
        {
            totalBidText.transform.position = teamBalanceText[teams.Count].transform.position;
            totalBidText.text = "aaaaa";
        }
        int i = teams.Count;
        while (i < teamNamesText.Count)
        {
            Destroy(teamNamesText[i]);
            Destroy(teamBidText[i]);
            Destroy(teamBalanceText[i]);
            i += 1;
        }
        Setup(); ;
        Add_Listners();
    }



    public void Add_Listners()
    {
        bidButtons[0].onClick.AddListener(delegate { Bid(100); });
        bidButtons[1].onClick.AddListener(delegate { Bid(200); });
        bidButtons[2].onClick.AddListener(delegate { Bid(300); });
        bidButtons[3].onClick.AddListener(delegate { Bid(400); });
        bidButtons[4].onClick.AddListener(delegate { Bid(500); });
        bidButtons[5].onClick.AddListener(delegate { Bid(1000); });
        vbButton.onClick.AddListener(delegate { Va_Banque(); });
        exitButton.onClick.AddListener(delegate { Exit_To_Lobby(); });
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
        Reset_Timer();
    }

    [Rpc(SendTo.Everyone)]
    void Setup_Stage_2_Rpc()
    {
        int i = 0;
        while (i < teams.Count)
        {
            teams[i].Raise_Bid(500);
            Update_Money_Status_For_Team(i);
            i += 1;
            winningBidAmount = 500;
            totalBid = teams.Count * 500;
            totalBidText.text = totalBid.ToString();
        }
        gameOngoing = true;
    }

    public void Update_Money_Status_For_Team(int i)
    {
        teamBalanceText[i].text = teams[i].Money.ToString();
        teamBidText[i].text = teams[i].Bid.ToString();
    }

    public void Update_Money_Status()
    {
        int i = 0;
        while (i < teams.Count)
        {
            Update_Money_Status_For_Team(i);
            i += 1;
        }
        Update_Buttons();
        totalBidText.text = totalBid.ToString();
    }

    public void Update_Buttons()
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

            bidButtonText[0].text += "(" + (difference + 100).ToString() + ")";
            bidButtonText[1].text += "(" + (difference + 200).ToString() + ")";
            bidButtonText[2].text += "(" + (difference + 300).ToString() + ")";
            bidButtonText[3].text += "(" + (difference + 400).ToString() + ")";
            bidButtonText[4].text += "(" + (difference + 500).ToString() + ")";
            bidButtonText[5].text += "(" + (difference + 1000).ToString() + ")";
        }


    }
    public void Va_Banque()
    {
        int amount = teams[(int)NetworkManager.Singleton.LocalClientId].Money + teams[(int)NetworkManager.Singleton.LocalClientId].Bid - winningBidAmount;
        Bid(amount);
    }

    public void Bid(int amount)
    {
        if (gameOngoing)
        {
            Team_Bid_Rpc(NetworkManager.Singleton.LocalClientId, amount);
        }
    }

    [Rpc(SendTo.Server)]
    public void Team_Bid_Rpc(ulong teamid, int amount)
    {
        int team_id = (int)teamid;
        int difference = winningBidAmount + amount - teams[team_id].Bid;
        if ((teams[team_id].Money >= difference && teams[team_id].Bid != winningBidAmount) || teams[team_id].Money >= difference && winningBidAmount == 500)
        {
            winningBidAmount += amount;
            Update_Bids_Rpc(team_id, difference, winningBidAmount, team_id);
            if (teams[team_id].Money == 0)
            {
                Sell(team_id);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void Update_Bids_Rpc(int team_id, int difference, int winning_bid, int winning_team_id)
    {
        teams[team_id].Raise_Bid(difference);
        totalBid += difference;
        winningBidAmount = winning_bid;
        winningTeamID = winning_team_id;
        Reset_Timer();
        Update_Money_Status();
    }

    public void Reset_Timer()
    {
        timer = Time.time;
    }

    void Update()
    {

        if (gameOngoing)
        {
            if (winningBidAmount != 500)
            {
                timerText.text = (timeGiven - (Time.time - timer)).ToString();
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
                Setup_Stage_2_Rpc();
            }
        }
    }
    void Sell(int team_id)
    {
        Sell_Rpc(team_id);
    }

    [Rpc(SendTo.Everyone)]
    void Sell_Rpc(int team_id)
    {
        foreach (Team t in teams)
        {
            t.Reset_Bid();
        }
        gameOngoing = false;
        timerText.text = "Wygrywa drużyna " + teams[team_id].Colour;
        //na razie team_id nie jest na nic potrzebne ale jest na później żeby można było w następnej scenie stwierdzić kto wygrał licytację
    }
}

