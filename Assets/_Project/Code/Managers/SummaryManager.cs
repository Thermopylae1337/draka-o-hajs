using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;

    private void Start()
    {
        foreach (TeamManager team in GameObject.Find("GameManager").GetComponent<GameManager>().Teams)
        {
            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(team);
        }
    }

    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click
}
