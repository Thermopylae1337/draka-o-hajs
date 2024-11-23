using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [SerializeField] GameObject panelPrefab;
    [SerializeField] Transform grid;

    void Start()
    {
        //foreach (Team team in listOfteams)
        foreach (Team team in new Team[] { new Team(), new Team(), new Team() })     // temp
        {
            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(team);
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Lobby");   //utils jest statyczne i nie wyœwieltaja siê w inspektorze w On Click
    }
}
