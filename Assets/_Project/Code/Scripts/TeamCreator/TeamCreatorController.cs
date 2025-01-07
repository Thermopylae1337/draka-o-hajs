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

        // Check if client is running
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }

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
