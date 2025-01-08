using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Kontroler menu głównego odpowiedzialny za wybór typu lobby (Gospodarz lub zwykły gracz)
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// Typ lobby z domyślną wartością NotSelected (odpowiednik null), co oznacza brak wyboru.
    /// </summary>
    public static LobbyTypeEnum lobbyType = LobbyTypeEnum.NotSelected;
    /// <summary>
    /// Ustawia typ lobby na 'Host' i ładuję scenę 'TeamCreator'.
    /// </summary>
    public void OnHostLobby()
    {
        lobbyType = LobbyTypeEnum.Host;
        SceneManager.LoadScene("TeamCreator");
    }

    /// <summary>
    /// Ustawia typ lobby na 'Join' i ładuje scenę 'TeamCreator'.
    /// </summary>
    public void OnJoinLobby()
    {
        lobbyType = LobbyTypeEnum.Join;
        SceneManager.LoadScene("TeamCreator");
    }
    /// <summary>
    /// Ładuję scenę 'Badges', która odpowiada za wyświetlanie odznak.
    /// </summary>
    public void LoadBadgesScene() => SceneManager.LoadScene("Badges");
    public void LoadLeaderboardScene() => SceneManager.LoadScene("Leaderboard");
}
