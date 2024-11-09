using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject playerNameDialog;
    private int _lobbyType = -1;
    public void OnHostLobby()
    {
        _lobbyType = 0;
        ShowPlayerNameDialog();
    }

    public void OnJoinLobby()
    {
        _lobbyType = 1;
        ShowPlayerNameDialog();
    }

    public void ShowPlayerNameDialog()
    {
        playerNameDialog.SetActive(true);
    }

    public void HidePlayerNameDialog()
    {
        playerNameDialog.SetActive(false);
    }

    public void OnStartGame()
    {
        switch (_lobbyType)
        {
            case 0:
                NetworkManager.Singleton.StartHost();
                break;
            case 1:
                NetworkManager.Singleton.StartClient();
                break;
            default:
                throw new System.Exception("Lobby type not selected");
        }
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}
