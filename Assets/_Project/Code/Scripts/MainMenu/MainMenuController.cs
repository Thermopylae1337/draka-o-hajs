using Assets._Project.Code.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static LobbyTypeEnum lobbyType = LobbyTypeEnum.NotSelected;
    public void OnHostLobby()
    {
        lobbyType = LobbyTypeEnum.Host;
        SceneManager.LoadScene("TeamCreator");
    }

    public void OnJoinLobby()
    {
        lobbyType = LobbyTypeEnum.Join;
        SceneManager.LoadScene("TeamCreator");
    }
}
