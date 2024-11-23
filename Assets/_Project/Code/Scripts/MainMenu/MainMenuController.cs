using Assets._Project.Code.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static LobbyType lobbyType = LobbyType.NotSelected;
    public void OnHostLobby()
    {
        lobbyType = LobbyType.Host;
        SceneManager.LoadScene("TeamCreator");
    }

    public void OnJoinLobby()
    {
        lobbyType = LobbyType.Join;
        SceneManager.LoadScene("TeamCreator");
    }
}
