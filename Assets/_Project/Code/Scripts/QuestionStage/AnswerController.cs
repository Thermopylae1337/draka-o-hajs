using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class AnswerController : NetworkBehaviour
{
    public TMP_InputField answerInput;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text timerText;
    public Button submitButton;
    public TMP_Text roundNumber;
    public GameObject hintButtonsContainer;
    public Button useHintsButton;

    private Button[] answerButtons;
    public static Team winner;
    public static int currentQuestionIndex = 0;
    private float _timeRemaining;
    private bool _isAnswerChecked;

    public static Category category;
    public Question currentQuestion;

    private void Start()
    {
        answerButtons = hintButtonsContainer.GetComponentsInChildren<Button>();
        _isAnswerChecked = false;
        SetHintMode(false);

        foreach (Button button in answerButtons)
        {
            button.onClick.AddListener(() => OnSelectButton(button));
        }

        if (IsHost)
        {
            SetCategoryServerRpc();
            StartRoundServerRpc();
        }

        feedbackText.text = Utils.CurrentTeam == winner
            ? "Jesteś graczem ktory wygrał licytacje"
            : "Jesteś graczem ktory przegrał licytacje. Tryb obserwatora";
    }

    private IEnumerator StartCountdown()
    {
        while (_timeRemaining > 0 && _isAnswerChecked == false)
        {
            _timeRemaining -= Time.deltaTime;
            ShowCurrentTimeRpc(_timeRemaining);
            yield return null;
        }

        if (_timeRemaining <= 0)
        {
            SetItemsInteractivity(false);
            feedbackText.text = "Czas minął! Odpowiedzi: " + string.Join(", ", currentQuestion.CorrectAnswers);
            _ = StartCoroutine(ChangeScene("Lobby", 4));
        }
    }

    public void CheckAnswer()
    {
        SetItemsInteractivity(false);
        _isAnswerChecked = true;
        string playerAnswer = answerInput.text.Trim();
        CheckAnswerServerRpc(playerAnswer);
    }

    public void CheckAnswer(string playerAnswer)
    {
        SetItemsInteractivity(false);
        _isAnswerChecked = true;
        CheckAnswerServerRpc(playerAnswer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckAnswerServerRpc(string playerAnswer)
    {
        if (currentQuestion.IsCorrect(playerAnswer))
        {
            SendFeedbackToClientsRpc("Brawo! Poprawna odpowiedź.");
        }
        else
        {
            SendFeedbackToClientsRpc($"Niestety, to nie jest poprawna odpowiedź. " +
                $"Poprawne odpowiedzi to: {string.Join(", ", currentQuestion.CorrectAnswers)}");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendFeedbackToClientsRpc(string feedback)
    {
        feedbackText.text = feedback;
        _ = StartCoroutine(ChangeScene("Lobby", 4));
    }

    public void AskForHint() => UseHintNotifyServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void UseHintNotifyServerRpc() => ShowHintRpc(currentQuestion);

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowHintRpc(Question question)
    {
        SetHintMode(true);
        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = question.Hints[i];
        }
    }
    private void SetHintMode(bool active)
    {
        if (active)
        {
            SetButtonsDefaultColor();
        }

        answerInput.gameObject.SetActive(!active);

        hintButtonsContainer.SetActive(active);
    }

    private void OnSelectButton(Button button)
    {
        SetButtonsDefaultColor();
        button.GetComponent<Image>().color = Color.blue;
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        string buttonValue = buttonText.text;
        CheckAnswer(buttonValue);
    }

    private void SetButtonsDefaultColor()
    {
        foreach (Button button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }

    private IEnumerator ChangeScene(string sceneName, float time)
    {
        yield return new WaitForSeconds(time);
        _ = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void SetItemsInteractivity(bool set)
    {
        foreach (Button button in answerButtons)
        {
            button.interactable = set;
        }

        answerInput.interactable = set;
        useHintsButton.interactable = set;
        submitButton.interactable = set;
    }

    [Rpc(SendTo.Everyone)]
    public void SendQuestionToClientRpc(string questionText, int currentQuestionIndex)
    {
        answerButtons = hintButtonsContainer.GetComponentsInChildren<Button>();
        SetItemsInteractivity(false);
        this.questionText.text = questionText;
        _isAnswerChecked = false;
        roundNumber.text = "Runda numer: " + currentQuestionIndex.ToString();
        feedbackText.text = "";
        answerInput.text = "";
        timerText.text = "";
    }

    [Rpc(SendTo.Server)]
    private void SetCategoryServerRpc()
    {
        category = Category.Deserialize("Assets/_Project/Code/Models/Historia.json");
        currentQuestion = category.DrawQuestion();
    }

    [Rpc(SendTo.Server)]
    private void StartRoundServerRpc()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex <= Utils.QUESTIONS_AMOUNT)
        {
            SendQuestionToClientRpc(currentQuestion.Content, currentQuestionIndex);
            _timeRemaining = 30f;
            AnsweringModeRpc(winner);
            SetHintMode(false);
            _ = StartCoroutine(StartCountdown());
        }
        else
        {
            SetItemsInteractivity(false);
            feedbackText.text = "Koniec gry";
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void ShowCurrentTimeRpc(float timeRemaining) => timerText.text = "Czas: " + Mathf.Ceil(timeRemaining) + "s";

    [Rpc(SendTo.ClientsAndHost)]
    private void AnsweringModeRpc(Team winner)
    {
        if (Utils.CurrentTeam == winner)
        {
            SetItemsInteractivity(true);
        }
    }
}