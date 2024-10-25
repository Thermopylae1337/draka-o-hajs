using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerConnector : MonoBehaviour
{
    public NetworkManager NetworkManager;
    public void OnHostLobby()
    {
        if (NetworkManager.StartHost())
            NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void OnJoinLobby()
    {
        if (NetworkManager.StartClient())
            NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}
