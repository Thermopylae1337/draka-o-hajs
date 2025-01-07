using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa kontrolera tablicy wyników, umożliwiająca przełączanie scen w grze.
/// </summary>
public class LeaderboardController : MonoBehaviour
{
    /// <summary>
    /// Metoda ładuje scenę głównego menu.
    /// </summary>
    public void LoadMainMenuScene() => SceneManager.LoadScene("MainMenu");
}
