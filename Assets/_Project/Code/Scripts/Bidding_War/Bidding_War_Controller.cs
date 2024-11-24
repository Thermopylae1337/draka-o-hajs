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
    NetworkManager NetMan;
    public List<TextMeshProUGUI> Team_Names_Text;
    public List<TextMeshProUGUI> Team_Bid_Text;
    public List<TextMeshProUGUI> Team_Balance_Text;
    public List<TextMeshProUGUI> Bid_Button_Text;
    public TextMeshProUGUI Timer_Text;
    public List<Team> Teams;
    public TextMeshProUGUI Total_Bid_Text;
    int _total_bid;
    float _timer;
    float _time_given = 5;
    int _Winning_Team_ID = 0;
    int _winning_bid_amount = 0;
    bool _has_Set_Up = false;
    bool _is_host;
    ulong _player_id;
    bool _game_ongoing = false;
    //przyciski kolejno maj� warto��: 100,200,300,400,500,1000z�
    //no i va banque
    //warto�� przycisku= warto�� o jak� dru�yna *przebija stawk�*
    //mo�naby te� zrobi� z ka�dego przycisku oddzielny var ale imo tak jest �adniej.
    public List<Button> Bid_Buttons;
    public Button VB_Button;
    public Button ExitButton;
    /*
    ///make it so each event removes itself by using the id
    List<Timer> Active_Timers
    public delegate void My_Timer_Delegate(int )
    */

    public class Timer
    {
        float start_time;
        float desired_gap;
    }

    public void Exit_To_Lobby()
    {
        Disconnect_Player_Rpc(this._player_id);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    [Rpc(SendTo.Server)]
    public void Disconnect_Player_Rpc(ulong playerid)
    {
        NetworkManager.DisconnectClient((ulong)playerid);
    }
    void Start()
    {

        _player_id = General_Game_Data.ID;
        NetMan = General_Game_Data.NetMan;
        _is_host = General_Game_Data._is_host;
        Teams = General_Game_Data.Teams;
        if (!_is_host)
        {
            NetMan.StartClient();
        }
        else
        {
            NetMan.StartHost();
        }
        if (Teams.Count < 4)
        {
            Total_Bid_Text.transform.position = Team_Balance_Text[Teams.Count].transform.position;
            Total_Bid_Text.text = "aaaaa";
        }
        int i = Teams.Count;
        while (i < Team_Names_Text.Count)
        {
            Destroy(Team_Names_Text[i]);
            Destroy(Team_Bid_Text[i]);
            Destroy(Team_Balance_Text[i]);
            i += 1;
        }
        Setup(); ;
        Add_Listners();
    }



    public void Add_Listners()
    {
        Bid_Buttons[0].onClick.AddListener(delegate { Bid(100); });
        Bid_Buttons[1].onClick.AddListener(delegate { Bid(200); });
        Bid_Buttons[2].onClick.AddListener(delegate { Bid(300); });
        Bid_Buttons[3].onClick.AddListener(delegate { Bid(400); });
        Bid_Buttons[4].onClick.AddListener(delegate { Bid(500); });
        Bid_Buttons[5].onClick.AddListener(delegate { Bid(1000); });
        VB_Button.onClick.AddListener(delegate { Va_Banque(); });
        ExitButton.onClick.AddListener(delegate { Exit_To_Lobby(); });
    }

    void Setup()
    {
        int i = 0;
        while (i < Teams.Count)
        {
            Team_Balance_Text[i].text = Teams[i].Money.ToString();
            Team_Bid_Text[i].text = Teams[i].Bid.ToString();
            Team_Names_Text[i].text = Teams[i].Colour;
            i += 1;
        }
        Reset_Timer();
    }

    [Rpc(SendTo.Everyone)]
    void Setup_Stage_2_Rpc()
    {
        int i = 0;
        while (i < Teams.Count)
        {
            Teams[i].Raise_Bid(500);
            Update_Money_Status_For_Team(i);
            i += 1;
            _winning_bid_amount = 500;
            _total_bid = Teams.Count * 500;
            Total_Bid_Text.text = _total_bid.ToString();
        }
        _game_ongoing = true;
    }

    public void Update_Money_Status_For_Team(int i)
    {
        Team_Balance_Text[i].text = Teams[i].Money.ToString();
        Team_Bid_Text[i].text = Teams[i].Bid.ToString();
    }

    public void Update_Money_Status()
    {
        int i = 0;
        while (i < Teams.Count)
        {
            Update_Money_Status_For_Team(i);
            i += 1;
        }
        Update_Buttons();
        Total_Bid_Text.text = _total_bid.ToString();
    }

    public void Update_Buttons()
    {
        if (_winning_bid_amount != Teams[(int)_player_id].Bid)
        {
            int difference = _winning_bid_amount - Teams[(int)_player_id].Bid;

            Bid_Button_Text[0].text = "100(" + (difference + 100).ToString() + ")";
            Bid_Button_Text[1].text = "200(" + (difference + 200).ToString() + ")";
            Bid_Button_Text[2].text = "300(" + (difference + 300).ToString() + ")";
            Bid_Button_Text[3].text = "400(" + (difference + 400).ToString() + ")";
            Bid_Button_Text[4].text = "500(" + (difference + 500).ToString() + ")";
            Bid_Button_Text[5].text = "1000(" + (difference + 1000).ToString() + ")";
        }
        else
        {
            Bid_Button_Text[0].text = "100";
            Bid_Button_Text[1].text = "200";
            Bid_Button_Text[2].text = "300";
            Bid_Button_Text[3].text = "400";
            Bid_Button_Text[4].text = "500";
            Bid_Button_Text[5].text = "1000";

        }
    }
    public void Va_Banque()
    {
        int amount = Teams[(int)_player_id].Money + Teams[(int)_player_id].Bid - _winning_bid_amount;
        Bid(amount);
    }

    public void Bid(int amount)
    {
        if (_game_ongoing)
        {
            Team_Bid_Rpc(_player_id, amount);
        }
    }

    [Rpc(SendTo.Server)]
    public void Team_Bid_Rpc(ulong teamid, int amount)
    {
        int team_id = (int)teamid;
        int difference = _winning_bid_amount + amount - Teams[team_id].Bid;
        if ((Teams[team_id].Money >= difference && Teams[team_id].Bid != _winning_bid_amount) || Teams[team_id].Money >= difference && _winning_bid_amount == 500)
        {
            _winning_bid_amount += amount;
            Update_Bids_Rpc(team_id, difference, _winning_bid_amount, team_id);
            if (Teams[team_id].Money == 0)
            {
                Sell(team_id);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void Update_Bids_Rpc(int team_id, int difference, int winning_bid, int winning_team_id)
    {
        Teams[team_id].Raise_Bid(difference);
        _total_bid += difference;
        _winning_bid_amount = winning_bid;
        _Winning_Team_ID = winning_team_id;
        Reset_Timer();
        Update_Money_Status();
    }

    public void Reset_Timer()
    {
        _timer = Time.time;
    }

    void Update()
    {

        if (_game_ongoing)
        {
            if (_winning_bid_amount != 500)
            {
                Timer_Text.text = (_time_given - (Time.time - _timer)).ToString();
                if (Time.time - _timer > _time_given && _is_host)
                {

                    Sell(_Winning_Team_ID);

                }
            }
        }
        else
        {
            if (Time.time - _timer > _time_given && _is_host && !_has_Set_Up & _is_host)
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
        foreach (Team t in Teams)
        {
            t.Reset_Bid();
        }
        _game_ongoing = false;
        Timer_Text.text = "Wygrywa dru�yna " + Teams[team_id].Colour;
        //na razie team_id nie jest na nic potrzebne ale jest na p�niej �eby mo�na by�o w nast�pnej scenie stwierdzi� kto wygra� licytacj�
    }
}
