using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;

    private void Start()
    {
        //foreach (Team team in listOfTeams)
        foreach (Team team in new Team[] { new(), new(), new() })     // temp
        {
            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(team);
        }
    }

    public void ChangeScene() => SceneManager.LoadScene("Lobby");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click
}
