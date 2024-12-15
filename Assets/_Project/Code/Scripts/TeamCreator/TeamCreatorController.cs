using Assets._Project.Code.Models;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamCreatorController : NetworkBehaviour // dodac back to main menu
{
    public TMP_InputField inputField;
    public GameObject spinner;
    public GameObject ipField;
    public GameObject errorMessage;
    public static string chosenTeamName;
    public Button returnButton;
    public Button submitButton;
    TMP_InputField tMP_InputField;

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

    public void OnReturnToMenu()
    {
        returnButton.interactable = false;
        MainMenuController.lobbyType = LobbyTypeEnum.NotSelected;
        SceneManager.LoadScene("MainMenu");
    }

    private void HandleErrorClient(ulong value)
    {
        RemoveClientHandlers();
        errorMessage.SetActive(true);

    }

    private void HandleSuccessClient(ulong value)
    {
        RemoveClientHandlers();
    }
    void RemoveClientHandlers()
    {
        spinner.SetActive(false);
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleSuccessClient;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleErrorClient;
        inputField.interactable = true;
        tMP_InputField.interactable = true;
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
                NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipField.GetComponentInChildren<TMP_InputField>().text;

                NetworkManager.Singleton.OnClientDisconnectCallback += HandleErrorClient;
                NetworkManager.Singleton.OnClientConnectedCallback += HandleSuccessClient;

                spinner.SetActive(true);
                bool success = NetworkManager.StartClient();

                // SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
                break;
            default:
                break;
        }
    }
}
