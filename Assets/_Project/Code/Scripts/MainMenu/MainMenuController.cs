using Assets._Project.Code.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject playerNameDialog;
    public void OnHostLobby()
    {
        LobbyType.Type = 0; 
        SceneManager.LoadScene("TeamCreator"); 
    }

    public void OnJoinLobby()
    {
        LobbyType.Type = 1; 
        SceneManager.LoadScene("TeamCreator"); 
    }
}
