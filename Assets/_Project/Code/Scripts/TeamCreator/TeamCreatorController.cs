using Assets._Project.Code.Models;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TeamCreatorController : MonoBehaviour // dodac backto mainmenu
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
            // jakiœ check na zakazane s³owa? XDD
            Utils.CurrentTeam.Name = userInput;
            inputField.interactable = false;
            StartGame();
        }
    }

    public void OnReturnToMenu()
    {
        MainMenuController.Type = LobbyType.NotSelected;
        returnButton.interactable = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void StartGame()
    {

        switch (MainMenuController.Type)
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
