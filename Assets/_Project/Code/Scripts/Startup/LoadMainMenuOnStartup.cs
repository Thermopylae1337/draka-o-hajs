using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenuOnStartup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Utils.LoadTeamFromDisk();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
