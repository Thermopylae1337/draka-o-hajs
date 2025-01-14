using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa zarządzająca procesem tworzenia drużyny oraz logiką sieciową związaną z tworzeniem i zarządzaniem drużynami.
/// </summary>
public class TeamCreatorController : NetworkBehaviour // dodac back to main menu
{
    /// <summary>
    /// Obiekt przechowujący pole tekstowe umożliwiające wprowadzenie znaków.
    /// </summary>
    public TMP_InputField inputField;
    /// <summary>
    /// Obiekt reprezentujący koło fortuny w grze, w których zawarte są pytania
    /// </summary>
    public GameObject spinner;
    /// <summary>
    /// Obiekt reprezentujący numer identyfikacyjny pola.
    /// </summary>
    public GameObject ipField;
    /// <summary>
    /// Obiekt reprezentujący błąd w grze.
    /// </summary>
    public GameObject errorMessage;
    /// <summary>
    /// Zmienna statyczna przechowująca nazwę wybranej drużyny.
    /// </summary>
    public static string chosenTeamName;
    /// <summary>
    /// Obiekt reprezentujący przycisk umożliwiający powrót.
    /// </summary>
    public Button returnButton;
    /// <summary>
    /// Obiekt reprezentujący przycisk umożlwiający zatwierdzenie.
    /// </summary>
    public Button submitButton;
    /// <summary>
    /// Obiekt wykorzystywany do przyjmowania tekstu w grze, szczególnie w przypadkach, gdy użytkownik musi wprowadzić dane.
    /// </summary>
    TMP_InputField tMP_InputField;

    /// <summary>
    /// Metoda inicjalizująca komponenty i ustawiająca reakcje podczas klikniecia przycisku.
    /// Konfiguruję pole wejściowe i przyciski oraz decyduję, czy pole identyfikacyjne (IP) jest widoczne na podstawie typu lobby.  
    /// </summary> 
    private void Start()
    {
        tMP_InputField = ipField.GetComponentInChildren<TMP_InputField>();
        submitButton.onClick.AddListener(OnInputSubmit);
        returnButton.onClick.AddListener(OnReturnToMenu);

        if (MainMenuController.lobbyType != LobbyTypeEnum.Join)
        {
            ipField.SetActive(false);
        }
        else
        {
            tMP_InputField.text = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
            tMP_InputField.onValueChanged.AddListener((newValue) =>
            {
                errorMessage.SetActive(false);
            });
        }
    }

    /// <summary>
    ///  Metoda pozwalająca na wprowadzenie nazwy drużyny przez użytkownika.
    ///  Nastepnie zapisuje nazwę drużyny, dezaktywuje pola wejściowe i uruchamia grę. 
    /// </summary>
    public void OnInputSubmit()
    {

        submitButton.interactable = false;
        string userInput = inputField.text;

        if (!string.IsNullOrEmpty(userInput))
        {
            //dodanie zapisu, odczytu teamu?
            // jakis check na zakazane słowa? XDD
            inputField.interactable = false;
            tMP_InputField.interactable = false;
            chosenTeamName = userInput;
            StartGame();
        }

        submitButton.interactable = true;
    }

    /// <summary>
    /// Umożliwia użytkownikowi powrócić do menu po naciśnięciu odpowiedniego przycisku.
    /// </summary>
    public void OnReturnToMenu()
    {
        returnButton.interactable = false;
        MainMenuController.lobbyType = LobbyTypeEnum.NotSelected;

        // Check if client is running
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }


    /// <summary>
    ///  Obsługuje błąd klienta. Usuwa obsługiwane zdarzenia dla klienta i wyświetla komunikat o błędzie.
    /// </summary>
    /// <param name="value">Zmienna reprezentująca identyfikator klienta.</param>
    private void HandleErrorClient(ulong value)
    {
        RemoveClientHandlers();
        errorMessage.SetActive(true);

    }

    /// <summary>
    ///  Obsługuje sukces połączenia klienta. Usuwa odpowiednie obsługiwane zdarzenia.
    /// </summary>
    /// <param name="value">Zmeinna reprezentująca identyfikator klienta, który połączył się pomyślnie.</param>
    private void HandleSuccessClient(ulong value)
    {
        RemoveClientHandlers();
    }
    /// <summary>
    /// Usuwa obsługiwane zdarzenia klienta, przywraca interaktywność pól wejściowych i dezaktywuje spinner.
    /// Zapewnia, że nie pojawią się błędy przy przejściu do lobby.
    /// </summary>

    void RemoveClientHandlers()
    {
        //dodany if bo wyrzucało errory przy wchodzeniu do lobby
        if (spinner)
        {
            spinner.SetActive(false);
        };

        //dodany if bo wyrzucało errory przy wchodzeniu do lobby
        if (inputField != null)
        {
            inputField.interactable = true;
        }
        //dodany if bo wyrzucało errory przy wchodzeniu do lobby
        if (tMP_InputField != null)
        {
            tMP_InputField.interactable = true;
        }
    }

    /// <summary>
    /// Inicjuje rozpoczęciu gry w zależności od typu lobby.
    /// Dla hosta uruchamia serwrt i ładuje scenę lobby,a dla klienta ustawia adres IP, nastepnie rozpoczyna połącznie z serwerem i wyświetla spinner.
    /// </summary>
    private void StartGame()
    {
        switch (MainMenuController.lobbyType)
        {
            case LobbyTypeEnum.Host:
                try
                {
                    if (!NetworkManager.Singleton.StartHost())
                    {
                        errorMessage.SetActive(true);
                        return;
                    }
                }
                catch (System.Exception)
                {
                    errorMessage.SetActive(true);
                    return;
                }

                NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
                _ = NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
                break;
            case LobbyTypeEnum.Join:
                NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipField.GetComponentInChildren<TMP_InputField>().text;

                spinner.SetActive(true);
                _ = NetworkManager.Singleton.StartClient();
                break;
        }
    }
}
