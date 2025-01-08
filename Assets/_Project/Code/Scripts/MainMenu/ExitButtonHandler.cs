using UnityEngine;

/// <summary>
/// Klasa obsługująca działanie przycisku wyjścia z gry. 
/// </summary>
public class ExitButtonHandler : MonoBehaviour
{
    /// <summary>
    /// Metoda wywoływana po kliknięciu przycisku wyjścia, która zamyka aplikację.
    /// </summary>
    public void OnExitRequested() => Application.Quit();
}
