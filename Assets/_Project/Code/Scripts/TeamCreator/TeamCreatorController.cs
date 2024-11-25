using Assets._Project.Code.Models;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamCreatorController : MonoBehaviour // dodac back to main menu
{
    public TMP_InputField inputField;
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
            Utils.CurrentTeam.Name = userInput;
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

        _ = MainMenuController.lobbyType switch
        {
            LobbyTypeEnum.Host => NetworkManager.Singleton.StartHost(),
            LobbyTypeEnum.Join => NetworkManager.Singleton.StartClient(),
            _ => throw new System.Exception("Lobby type not selected"),
        };
        _ = NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}
