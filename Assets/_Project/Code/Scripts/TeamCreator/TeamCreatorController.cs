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

    void Start()
    {
        inputField.onEndEdit.AddListener(OnInputSubmit);
        returnButton.onClick.AddListener(OnReturnToMenu);
    }

    public void OnInputSubmit(string userInput)
    {
        if (!string.IsNullOrEmpty(userInput))
        {
            //dodanie zapisu, odczytu teamu?
            // jaki� check na zakazane s�owa? XDD
            Utils.CurrentTeam.Name = userInput;
            inputField.interactable = false;
            StartGame();
        }
    }

    public void OnReturnToMenu()
    {
        MainMenuController.lobbyType = LobbyType.NotSelected;
        returnButton.interactable = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void StartGame()
    {

        switch (MainMenuController.lobbyType)
        {
            case LobbyType.Host:
                NetworkManager.Singleton.StartHost();
                break;
            case LobbyType.Join:
                NetworkManager.Singleton.StartClient();
                break;
            default:
                throw new System.Exception("Lobby type not selected");
        }

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}
