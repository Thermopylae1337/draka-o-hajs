using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa odpowiedzialna za zarządzanie sceną z odznakami, umożliwiająca powrót do menu głównego.
/// </summary>
public class BadgesController : MonoBehaviour
{
    /// <summary>
    /// Metoda wywołująca załadowanie sceny "MainMenu".
    /// </summary>
    public void LoadMainMenuScene() => SceneManager.LoadScene("MainMenu");
}
