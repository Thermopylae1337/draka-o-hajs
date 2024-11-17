using Assets._Project.Code.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static LobbyType Type = LobbyType.NotSelected;
    public void OnHostLobby()
    {
        Type = LobbyType.Host;
        SceneManager.LoadScene("TeamCreator"); 
    }

    public void OnJoinLobby()
    {
        Type = LobbyType.Join;
        SceneManager.LoadScene("TeamCreator"); 
    }
}
