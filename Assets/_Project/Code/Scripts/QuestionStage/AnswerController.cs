using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
public class AnswerController : NetworkBehaviour
{
    public TMP_InputField answerInput;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text timerText;
    public Button submitButton;
    public TMP_Text roundNumber;
    public List<TMP_Text> buttonTexts;
    public List<Button> answerButtons;
    public Button hintButton;

    public static ulong winnerID = 0;
    public static int currentQuestionIndex = 0;
    private float _timeRemaining;
    private bool _isAnswerChecked;

    public static Category category;
    public Question currentQuestion;

    void Start()
    {
        SetItemsInteractivity(false);
        currentQuestionIndex++;
        _isAnswerChecked = false;
        SetHintMode(false);

        answerButtons[0].onClick.AddListener(() => OnSelectButton(answerButtons[0]));
        answerButtons[1].onClick.AddListener(() => OnSelectButton(answerButtons[1]));
        answerButtons[2].onClick.AddListener(() => OnSelectButton(answerButtons[2]));
        answerButtons[3].onClick.AddListener(() => OnSelectButton(answerButtons[3]));

        roundNumber.text = "Runda numer: " + currentQuestionIndex.ToString();
        if (IsHost)
        {
            SetCategoryServerRpc();
            StartRoundServerRpc();
        }
        if (NetworkManager.Singleton.LocalClientId == winnerID)
        {
            feedbackText.text = "Jesteś graczem ktory wygrał licytacje";
        }
        else
        {
            feedbackText.text = "Jesteś graczem ktory przegrał licytacje. Tryb obserwatora";
        }
    }

    private IEnumerator StartCountdown()
    {
        while (_timeRemaining > 0 && _isAnswerChecked == false)
        {
            _timeRemaining -= Time.deltaTime;
            showCurrentTimeRpc(_timeRemaining);
            yield return null;
        }
        if (_timeRemaining <= 0)
        {
            SetItemsInteractivity(false);
            feedbackText.text = "Czas minął! Odpowiedzi: " + string.Join(", ", currentQuestion.CorrectAnswers);
            StartCoroutine(ChangeScene("Lobby", 4));
        }
    }

    void StartServerTime()
    {
        StartCoroutine(StartCountdown());
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
    void CheckAnswerServerRpc(string playerAnswer)
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
    void SendFeedbackToClientsRpc(string feedback)
    {
        feedbackText.text = feedback;
        StartCoroutine(ChangeScene("Lobby", 4));
    }

    public void AskForHint()
    {
        HintServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void HintServerRpc()
    {
        //Do poprawy gdy dostane finalna klase Question
        showHintRpc(currentQuestion.Hints[0], currentQuestion.Hints[1], currentQuestion.Hints[2], currentQuestion.Hints[3]);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void showHintRpc(string answer, string answer1, string answer2, string answer3)
    {
        SetHintMode(true);
        buttonTexts[0].text = answer;
        buttonTexts[1].text = answer1;
        buttonTexts[2].text = answer2;
        buttonTexts[3].text = answer3;
    }
    private void SetHintMode(bool active)
    {
        if (active == false)
        {
            answerInput.gameObject.SetActive(true);
            answerButtons[0].gameObject.SetActive(false);
            answerButtons[1].gameObject.SetActive(false);
            answerButtons[2].gameObject.SetActive(false);
            answerButtons[3].gameObject.SetActive(false);
        }
        else
        {
            setButtonsDefaultColor();
            answerInput.gameObject.SetActive(false);
            answerButtons[0].gameObject.SetActive(true);
            answerButtons[1].gameObject.SetActive(true);
            answerButtons[2].gameObject.SetActive(true);
            answerButtons[3].gameObject.SetActive(true);
        }
    }

    private void OnSelectButton(Button button)
    {
        setButtonsDefaultColor();
        button.GetComponent<Image>().color = Color.blue;
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        string buttonValue = buttonText.text;
        CheckAnswer(buttonValue);
    }

    private void setButtonsDefaultColor()
    {
        answerButtons[0].GetComponent<Image>().color = Color.white;
        answerButtons[1].GetComponent<Image>().color = Color.white;
        answerButtons[2].GetComponent<Image>().color = Color.white;
        answerButtons[3].GetComponent<Image>().color = Color.white;
    }

    private IEnumerator ChangeScene(string sceneName, float time)
    {
        yield return new WaitForSeconds(time);
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void SetItemsInteractivity(bool set)
    {
        answerButtons[0].interactable = set;
        answerButtons[1].interactable = set;
        answerButtons[2].interactable = set;
        answerButtons[3].interactable = set;
        answerInput.interactable = set;
        hintButton.interactable = set;
        submitButton.interactable = set;
    }

    [Rpc(SendTo.Everyone)]
    public void SendQuestionToClientRpc(string questionText)
    {
        this.questionText.text = questionText;
        _isAnswerChecked = false;
        roundNumber.text = "Runda numer: " + currentQuestionIndex.ToString();
        feedbackText.text = "";
        answerInput.text = "";
        timerText.text = "Czas: 30s";
    }

    [Rpc(SendTo.Server)]
    void SetCategoryServerRpc()
    {
        category = Category.Deserialize("Assets/_Project/Code/Models/Historia.json");
        currentQuestion = category.DrawQuestion();
    }

    [Rpc(SendTo.Server)]
    void StartRoundServerRpc()
    {
        if (currentQuestionIndex <= Utils.QUESTIONS_AMOUNT)
        {
            SendQuestionToClientRpc(currentQuestion.Content);
            _timeRemaining = 30f;
            AnsweringModeRpc(winnerID);
            SetHintMode(false);
            StartServerTime();
        }
        else
        {
            SetItemsInteractivity(false);
            feedbackText.text = "Koniec gry";
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    void showCurrentTimeRpc(float timeRemaining)
    {
        timerText.text = "Czas: " + Mathf.Ceil(timeRemaining) + "s";
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AnsweringModeRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == winnerID)
        {
            SetItemsInteractivity(true);
        }
    }
    void Update()
    {
    }
}
