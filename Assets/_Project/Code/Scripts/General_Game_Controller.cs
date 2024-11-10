using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Text;

/*
public class Team:  
{
    public ulong ID;
    public string Name;
    public int Money = 10000;
    public int bid = 0;
    public Team(string name)
    {
        this.Name = name;
    }
    
}*/


public static class General_Game_Data
{
    public static ulong ID;
    public static bool _is_host;
    public static NetworkManager NetMan = new NetworkManager();
    public static List<Team> Teams = new List<Team>();
    public static int[] Team_Balance;
    public static string[] Team_Names;

    //lot = list of teams
    //musia³em zrobiæ funkcjê przemieniej¹c¹ listê dru¿yn w strina poniewa¿ przez RPC nie mo¿na wysy³aæ list ani stringów. W ogóle
    // symbole: "@" = kolejny field, 
     public static string Team_List_Serializer(List<Team> LoT)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        while (i < LoT.Count) 
        {
            sb.Append(LoT[i].Serialize());
            i += 1;
            if (i < LoT.Count) 
            {
                sb.Append(',');
            }
        }
        return sb.ToString();
    
    }

     public static List<Team> Team_List_Deserializer(string source) 
    {
        List<Team> res = new List<Team>();        
        int i = 0;
        StringBuilder var = new StringBuilder();
        while (i < source.Length)
        {
            if (source[i] == ',') 
            {
                
                Debug.Log("deserializing list element: " + var.ToString());
                res.Add(Team.Deserialize(var.ToString()));

                var.Clear();
            }
            else
            {
                var.Append(source[i]);
            }
            i += 1;
        }
        res.Add(Team.Deserialize(var.ToString()));
        return res;
    }
}
