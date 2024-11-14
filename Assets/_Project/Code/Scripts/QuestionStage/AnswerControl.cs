using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
public class AnswerControl : NetworkBehaviour
{
    public TMP_InputField answerInput;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text timerText;
    public Button submitButton;
    public TMP_Text numerRundy;
    public TMP_Text buttonText1, buttonText2, buttonText3, buttonText4;
    public Button answerButton1, answerButton2, answerButton3, answerButton4;
    public Button hintButton;

    private float timeRemaining;
    public static int currentQuestionIndex = -1;
    private const int totalQuestions = 8;
    private bool isAnswerChecked = false;

    public static Category category;
    public Question currentQuestion;

    void Start()
    {
        interactivityOfItems(true);
        currentQuestionIndex++;
        isAnswerChecked = false;
        HintMode(false);

        answerButton1.onClick.AddListener(() => SelectButton(answerButton1));
        answerButton2.onClick.AddListener(() => SelectButton(answerButton2));
        answerButton3.onClick.AddListener(() => SelectButton(answerButton3));
        answerButton4.onClick.AddListener(() => SelectButton(answerButton4));

        numerRundy.text = "Runda numer: " + currentQuestionIndex.ToString();
        Debug.Log("Start");
        if (IsHost)
        {
            Debug.Log("Przeszlo ifa");
            SendCategoryToServerRpc();
        }
    }

    /*private void OpenQuestion()
    {
        if (currentQuestionIndex < totalQuestions)
        {
            HintMode(false);
            isAnswerChecked = false;
            numerRundy.text = "Runda numer: " + (currentQuestionIndex + 1).ToString();
            questionText.text = currentQuestion.Tresc;
            feedbackText.text = "";
            answerInput.text = "";
            timeRemaining = 30f;
            timerText.text = "Czas: 30s";
            StartCoroutine(StartCountdownRpc());
        }
        else
        {
            interactivityOfItems(false);
            feedbackText.text = "Koniec gry";
        }
    }*/

    private IEnumerator StartCountdown()
    {
        while (timeRemaining > 0 && isAnswerChecked == false)
        {
            timeRemaining -= Time.deltaTime;
            //timerText.text = "Czas: " + Mathf.Ceil(timeRemaining) + "s";
            showCurrentTimeRpc(timeRemaining);
            yield return null;
        }
        if (timeRemaining <= 0)
        {
            interactivityOfItems(false);
            feedbackText.text = "Czas minął! Odpowiedź: " + currentQuestion.giveCorrectAnswer();
            StartCoroutine(ChangeScene("Lobby", 4));
        }
    }
    [Rpc(SendTo.Server)]
    void timeControllerRpc()
    {
        StartCoroutine(StartCountdown());
    }

    public void CheckAnswer()
    {
        interactivityOfItems(false);
        isAnswerChecked = true;
        string playerAnswer = answerInput.text.Trim();
        CheckAnswerServerRpc(playerAnswer);
    }

    public void CheckAnswer(string playerAnswer)
    {
        interactivityOfItems(false);
        isAnswerChecked = true;
        CheckAnswerServerRpc(playerAnswer);
    }

    [ServerRpc(RequireOwnership = false)]
    void CheckAnswerServerRpc(string playerAnswer)
    {
        bool isCorrect = currentQuestion.IsCorrect(playerAnswer);
        string feedback = isCorrect
            ? "Brawo! Poprawna odpowiedź."
            : $"Niestety, to nie jest poprawna odpowiedź. Poprawna odpowiedz to: {currentQuestion.giveCorrectAnswer()}";

        SendFeedbackToClientsRpc(feedback);    
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
        //Nieoptymalne w chuj ale nie mam serializacji jeszcze 
        showHintRpc(currentQuestion.Hint()[0], currentQuestion.Hint()[1], currentQuestion.Hint()[2], currentQuestion.Hint()[3]);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void showHintRpc(string answer, string answer1, string answer2, string answer3)
    {
        HintMode(true);
        buttonText1.text = answer;
        buttonText2.text = answer1;
        buttonText3.text = answer2;
        buttonText4.text = answer3;
    }
    private void HintMode(bool active)
    {
        if (active == false)
        {
            answerInput.gameObject.SetActive(true);
            answerButton1.gameObject.SetActive(false);
            answerButton2.gameObject.SetActive(false);
            answerButton3.gameObject.SetActive(false);
            answerButton4.gameObject.SetActive(false);
        }
        else
        {
            setButtonsDefaultColor();
            answerInput.gameObject.SetActive(false);
            answerButton1.gameObject.SetActive(true);
            answerButton2.gameObject.SetActive(true);
            answerButton3.gameObject.SetActive(true);
            answerButton4.gameObject.SetActive(true);
        }
    }

    private void SelectButton(Button button)
    {
        setButtonsDefaultColor();
        button.GetComponent<Image>().color = Color.blue;
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        string buttonValue = buttonText.text;
        CheckAnswer(buttonValue);
    }

    private void setButtonsDefaultColor()
    {
        answerButton1.GetComponent<Image>().color = Color.white;
        answerButton2.GetComponent<Image>().color = Color.white;
        answerButton3.GetComponent<Image>().color = Color.white;
        answerButton4.GetComponent<Image>().color = Color.white;
    }

    private IEnumerator ChangeScene(string sceneName, float time)
    {
        yield return new WaitForSeconds(time);
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void interactivityOfItems(bool set)
    {
        answerButton1.interactable = set;
        answerButton2.interactable = set;
        answerButton3.interactable = set;
        answerButton4.interactable = set;
        answerInput.interactable = set;
        hintButton.interactable = set;
        submitButton.interactable = set;
    }


    [Rpc(SendTo.Everyone)]
    public void SendQuestionToClientRpc(string questionText, string answer1, string answer2, string answer3, string answer4)
    {
        Debug.Log("Otrzymano pytanie od serwera...");
        this.questionText.text = questionText;
        buttonText1.text = answer1;
        buttonText2.text = answer2;
        buttonText3.text = answer3;
        buttonText4.text = answer4;
        isAnswerChecked = false;
        numerRundy.text = "Runda numer: " + (currentQuestionIndex + 1).ToString();
        feedbackText.text = "";
        answerInput.text = "";
        timerText.text = "Czas: 30s";
    }
    [Rpc(SendTo.Server)]
    void SendCategoryToServerRpc()
    {
        Debug.Log("Wysyłam kategorię do serwera");
        if (currentQuestionIndex < totalQuestions)
        {
            category = Category.Deserializuj("Assets/_Project/Code/Models/Historia.json");
            currentQuestion = category.LosujPytanie();
            string[] answers = currentQuestion.Hint().ToArray();
            SendQuestionToClientRpc(currentQuestion.Tresc, answers[0], answers[1], answers[2], answers[3]);
            timeRemaining = 30f;
            HintMode(false);
            timeControllerRpc();
        }
        else
        {
            interactivityOfItems(false);
            feedbackText.text = "Koniec gry";
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void showCurrentTimeRpc(float timeRemaining)
    {
        timerText.text = "Czas: " + Mathf.Ceil(timeRemaining) + "s";
    }
    void Update()
    {
    }
}
