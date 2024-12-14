using Assets._Project.Code.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class AnswerController : NetworkBehaviour
{
    public TMP_Text totalBid;
    public TMP_Text hintPriceText;
    public TMP_InputField answerInput;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text timerText;
    public Button submitButton;
    public TMP_Text roundNumber;
    public GameObject hintButtonsContainer;
    public Button useHintsButton;

    public Image backgroundImage;
    public Sprite artYellowTeamAnswering;
    public Sprite artGreenTeamAnswering;
    public Sprite artBlueTeamAnswering;

    private Button[] answerButtons;
    public static int currentQuestionIndex = 0;
    private float _timeRemaining;
    private bool _isAnswerChecked;
    private string[] hints;
    private int randomHintPrice;

    public static Category category;
    public Question currentQuestion;
    private List<TeamManager> _teams;
    private uint _teamsInGame;

    private void Start()
    {
        ShowBackgroundImage();
        totalBid.text = "Pula pytania: " + GameManager.Instance.CurrentBid.Value.ToString();
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

        feedbackText.text = GameManager.Instance.Winner.Value == NetworkManager.Singleton.LocalClientId
            ? "Jesteś graczem ktory wygrał licytacje"
            : "Jesteś graczem ktory przegrał licytacje. Tryb obserwatora";
    }
    private void ShowBackgroundImage()
    {
        TeamManager _answeringTeam = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<TeamManager>();
        ColourEnum _colour = _answeringTeam.Colour;

        switch (_colour)
        {
            case ColourEnum.YELLOW:
                backgroundImage.sprite = artYellowTeamAnswering;
                break;
            case ColourEnum.GREEN:
                backgroundImage.sprite = artGreenTeamAnswering;
                break;
            case ColourEnum.BLUE:
                backgroundImage.sprite = artBlueTeamAnswering;
                break;
        }
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
            _ = currentQuestionIndex < Utils.ROUNDS_LIMIT && IsContinuingGamePossible()
                ? StartCoroutine(ChangeScene("CategoryDraw", 4))
                : StartCoroutine(ChangeScene("Summary", 4));
        }
    }

    public void CheckAnswer()
    {
        SetItemsInteractivity(false);
        _isAnswerChecked = true;
        string playerAnswer = answerInput.text.Trim();
        CheckAnswerServerRpc(playerAnswer);
        NotifyAnswerCheckedServerRpc(playerAnswer);
    }

    public void CheckAnswer(string playerAnswer)
    {
        SetItemsInteractivity(false);
        _isAnswerChecked = true;
        CheckAnswerServerRpc(playerAnswer);
        NotifyAnswerCheckedServerRpc(playerAnswer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyAnswerCheckedServerRpc(string playerAnswer)
    {
        _isAnswerChecked = true;
        CheckAnswerServerRpc(playerAnswer);
        NotifyClientsAnswerCheckedRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyClientsAnswerCheckedRpc()
    {
        _isAnswerChecked = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckAnswerServerRpc(string playerAnswer)
    {
        if (currentQuestion.IsCorrect(playerAnswer))
        {
            NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money += GameManager.Instance.CurrentBid.Value;
            GameManager.Instance.CurrentBid.Value = 0;
            SendFeedbackToClientsRpc("Brawo! Poprawna odpowiedź.", currentQuestionIndex < Utils.ROUNDS_LIMIT && IsContinuingGamePossible());
        }
        else
        {
            SendFeedbackToClientsRpc($"Niestety, to nie jest poprawna odpowiedź. " +
                $"Poprawne odpowiedzi to: {string.Join(", ", currentQuestion.CorrectAnswers)}",
                currentQuestionIndex < Utils.ROUNDS_LIMIT && IsContinuingGamePossible());
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendFeedbackToClientsRpc(string feedback, bool gameContinuing)
    {
        if (gameContinuing)
        {
            feedbackText.text = feedback;
            _ = StartCoroutine(ChangeScene("CategoryDraw", 4));
        }
        else
        {
            feedbackText.text = feedback;
            _ = StartCoroutine(ChangeScene("Summary", 4));
        }
    }

    public void AskForHint() => UseHintNotifyServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void UseHintNotifyServerRpc()
    {
        if (NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money >= randomHintPrice)
        {
            NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money -= randomHintPrice;
            hints = currentQuestion.Hints;
            ShowHintRpc(hints[0], hints[1], hints[2], hints[3]);
            _timeRemaining = 30f;
        }
        else
        {
            HintAskRejectionRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HintAskRejectionRpc()
    {
        hintPriceText.text = "Nie stać Cię na podpowiedź";
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowHintRpc(string h1, string h2, string h3, string h4)
    {
        SetHintMode(true);
        answerButtons[0].GetComponentInChildren<TMP_Text>().text = h1;
        answerButtons[1].GetComponentInChildren<TMP_Text>().text = h2;
        answerButtons[2].GetComponentInChildren<TMP_Text>().text = h3;
        answerButtons[3].GetComponentInChildren<TMP_Text>().text = h4;
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
    public void SendQuestionToClientRpc(string questionText, int currentQuestionIndex, float hintPrice)
    {
        answerButtons = hintButtonsContainer.GetComponentsInChildren<Button>();
        SetItemsInteractivity(false);
        this.questionText.text = questionText;
        hintPriceText.text = "Cena podpowiedzi: " + Convert.ToString(hintPrice);
        _isAnswerChecked = false;
        roundNumber.text = "Runda numer: " + currentQuestionIndex.ToString();
        feedbackText.text = "";
        answerInput.text = "";
        timerText.text = "";
    }

    [Rpc(SendTo.Server)]
    private void SetCategoryServerRpc()
    {
        category = GameManager.Instance.Category.Value;
        currentQuestion = category.DrawQuestion();
    }

    [Rpc(SendTo.Server)]
    private void StartRoundServerRpc()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex <= Utils.ROUNDS_LIMIT)
        {
            randomHintPrice = Convert.ToInt32(Mathf.Round(Convert.ToSingle(UnityEngine.Random.Range(20, 31)) / 100 * GameManager.Instance.CurrentBid.Value / 100f) * 100f);
            SendQuestionToClientRpc(currentQuestion.Content, currentQuestionIndex, randomHintPrice);
            _timeRemaining = 30f;
            AnsweringModeRpc();
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
    private void AnsweringModeRpc()
    {
        if (NetworkManager.Singleton.LocalClientId == GameManager.Instance.Winner.Value)
        {
            SetItemsInteractivity(true);
        }
    }
    private bool IsContinuingGamePossible()
    {
        _teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList();
        _teamsInGame = 0;
        foreach (TeamManager team in _teams)
        {
            if (team.Money >= 500)
            {
                _teamsInGame++;
            }
        }

        return _teamsInGame >= 2;
    }
}