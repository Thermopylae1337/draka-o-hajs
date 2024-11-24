using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Text;




public static class General_Game_Data
{
    public static ulong ID;
    public static bool _is_host;
    public static NetworkManager NetMan = new NetworkManager();
    public static List<Team> Teams = new List<Team>();
    public static int[] Team_Balance;
    public static string[] Team_Names;
}
