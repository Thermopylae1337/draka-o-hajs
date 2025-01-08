using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa odpowiedzialna za załadowanie głownego menu gry podczas uruchamiania sceny. 
/// </summary>
public class LoadMainMenuOnStartup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
