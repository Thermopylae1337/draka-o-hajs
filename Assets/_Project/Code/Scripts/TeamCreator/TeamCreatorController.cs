using Assets._Project.Code.Models;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamCreatorController : NetworkBehaviour // dodac back to main menu
{
    public TMP_InputField inputField;
    public static string chosenTeamName;
    public Button returnButton;
    public Button submitButton;

    private void Start()
    {
        submitButton.onClick.AddListener(OnInputSubmit);
        returnButton.onClick.AddListener(OnReturnToMenu);
    }

    public void OnInputSubmit()
    {

        submitButton.interactable = false;
        string userInput = inputField.text;

        if (!string.IsNullOrEmpty(userInput))
        {
            //dodanie zapisu, odczytu teamu?
            // jakis check na zakazane słowa? XDD
            inputField.interactable = false;
            chosenTeamName = userInput;
            StartGame();
        }

        submitButton.interactable = true;
    }

    public void OnReturnToMenu()
    {
        returnButton.interactable = false;
        MainMenuController.lobbyType = LobbyTypeEnum.NotSelected;
        SceneManager.LoadScene("MainMenu");
    }

    private void StartGame()
    {
        switch (MainMenuController.lobbyType)
        {
            case LobbyTypeEnum.Host:
                _ = NetworkManager.StartHost();
                NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
                _ = NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
                break;
            case LobbyTypeEnum.Join:
                _ = NetworkManager.StartClient();
                // SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
                break;
            default:
                break;
        }
    }
}
