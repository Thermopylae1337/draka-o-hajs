using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa zarządzająca etapem podsumowania.
/// </summary>
public class SummaryManager : NetworkBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform grid;
    /// <summary>
    /// Zmienna przechowujaca listę drużyn.
    /// </summary>
    public List<TeamManager> teams;
    /// <summary>
    /// Obiekt przechowujący najbogatszą drużynę.
    /// </summary>
    TeamManager richestTeam;
    /// <summary>
    /// Zmienna przechowująca numer identyfikujący daną drużynę.
    /// </summary>
    ulong winnerId;

    /// <summary>
    /// Metoda zajmująca się logiką gry jak i interfejsem użytkownika np. odpowiedzialna za:
    /// - tworzenie listy drużyn na podstawie połączonych graczy
    /// - znalezienie drużyny z najwiekszą ilościa pieniędzy i przypisanie jej jako zwycięzca
    /// - odblokowanie odznak na podstawie różnych osiągnięć.
    /// - tworzenie i inicjowanie paneli dla drużyn w interfejsie użytkownika.
    /// </summary>
    private void Start()
    {
        teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();
        richestTeam = teams.OrderByDescending(team => team.Money).FirstOrDefault();
        winnerId = richestTeam.OwnerClientId;

        if(NetworkManager.Singleton.LocalClientId == winnerId && teams[(int)NetworkManager.Singleton.LocalClientId].CluesUsed == 0)
        {
            UnlockBadge("Samodzielni Geniusze");
        }

        if (teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered == 0 && teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAsked > 0)
        {
            UnlockBadge("Mistrzowie pomyłek");
        }

        if (teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered == teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAsked && teams[(int)NetworkManager.Singleton.LocalClientId].QuestionsAnswered > 0)
        {
            UnlockBadge("As opowiedzi");
        }

        if (teams[(int)NetworkManager.Singleton.LocalClientId].Money >= 19000)
        {
            UnlockBadge("Królowie skarbca");
        }

        foreach (NetworkClient teamClient in NetworkManager.ConnectedClientsList)
        {

            Panel panel = Instantiate(panelPrefab, grid).GetComponent<Panel>();
            panel.Initialize(teamClient.PlayerObject.GetComponent<TeamManager>());
        }
    }

    /// <summary>
    /// Metoda pozwalająca na przejście do menu głownego gry.
    /// </summary>
    public void ChangeScene() => SceneManager.LoadScene("MainMenu");   //utils jest statyczne i nie wyswietlaja się w inspektorze w On Click

    /// <summary>
    /// Odblokowuje odznakę o podanej nazwie dla drużyny gracza. Gracz jest identyfikowany za pomocą jego unikalnego identyfikatora (LocalClientId).
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę odznaki.</param>
    private void UnlockBadge(string name)
    {
        teams[(int)NetworkManager.Singleton.LocalClientId].BadgeList.UnlockBadge(name);
    }
}
