using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;

    private void Start()
    {
        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {

            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(teamClient.PlayerObject.GetComponent<TeamManager>());
        }
    }

    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click
}
