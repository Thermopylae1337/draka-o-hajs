﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
public class AnswerControl : MonoBehaviour
{
    public TMP_InputField answerInput; 
    public TMP_Text questionText;     
    public TMP_Text feedbackText;    
    public Kategoria kategoria = new Kategoria();
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
        OpenQuestion();
    }

    private void OpenQuestion()
    {
        if (currentQuestionIndex < totalQuestions)
        {
            HintMode(false);
            isAnswerChecked = false;
            numerRundy.text = "Runda numer: " + (currentQuestionIndex + 1).ToString();
            questionText.text = kategoria.LosujPytanie();
            feedbackText.text = "";
            answerInput.text = "";
            timeRemaining = 30f;
            timerText.text = "Czas: 30s";
            StartCoroutine(StartCountdown());
        }
        else
        {
            interactivityOfItems(false);
            feedbackText.text = "Koniec gry";
        }
    }
    private IEnumerator StartCountdown()
    {
        while (timeRemaining > 0 && isAnswerChecked == false)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Czas: " + Mathf.Ceil(timeRemaining) + "s";
            yield return null;
        }
        if (timeRemaining <= 0)
        {
            interactivityOfItems(false);
            feedbackText.text = "Czas minął! Odpowiedź: " + kategoria.PobierzPoprawnaOdpowiedz();
            StartCoroutine(ChangeScene("TestScene", 4));
        }
    }
    public void CheckAnswer()
    {
        interactivityOfItems(false);
        isAnswerChecked = true;
        string playerAnswer = answerInput.text.Trim();
        string correctAnswer = kategoria.PobierzPoprawnaOdpowiedz();

        if (playerAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
        {
            feedbackText.text = $"Brawo! Poprawna odpowiedź.";
        }
        else
        {
            feedbackText.text = $"Niestety, to nie jest poprawna odpowiedź. Poprawna odpowiedz to: {kategoria.PobierzPoprawnaOdpowiedz()}";
        }
        StartCoroutine(ChangeScene("TestScene", 4));
    }

    public void CheckAnswer(string playerAnswer)
    {
        interactivityOfItems(false);
        isAnswerChecked = true;
        string correctAnswer = kategoria.PobierzPoprawnaOdpowiedz();

        if (playerAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
        {
            feedbackText.text = $"Brawo! Poprawna odpowiedź.";
        }
        else
        {
            feedbackText.text = $"Niestety, to nie jest poprawna odpowiedź. Poprawna odpowiedz to: {kategoria.PobierzPoprawnaOdpowiedz()}";
        }
        StartCoroutine(ChangeScene("TestScene", 4));
    }

    public void Hint()
    {
        HintMode(true);
        buttonText1.text = kategoria.PobierzPodpowiedzi()[0];
        buttonText2.text = kategoria.PobierzPodpowiedzi()[1];
        buttonText3.text = kategoria.PobierzPodpowiedzi()[2];
        buttonText4.text = kategoria.PobierzPodpowiedzi()[3];
    }
    //HintMode jest oddzielna funkcja niz Hint poniewaz Hint jest przypisany do buttona i nie może mieć parametrów
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

    /*private IEnumerator AskQuestions()
    {
        while (currentQuestionIndex < totalQuestions)
        {
            HintMode(false);
            isAnswerChecked = false;
            numerRundy.text = "Runda numer: " + (currentQuestionIndex+1).ToString();
            questionText.text = kategoria.LosujPytanie();
            feedbackText.text = "";
            answerInput.text = "";
            timeRemaining = 5f;
            timerText.text = "Czas: 30s";

            while (timeRemaining > 0 && isAnswerChecked == false)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = "Czas: " + Mathf.Ceil(timeRemaining) + "s";
                yield return null;
            }
            if(isAnswerChecked == true)
            {
                yield return new WaitForSeconds(4);
                continue;
            }
            feedbackText.text = "Czas minął! Odpowiedź: " + kategoria.PobierzPoprawnaOdpowiedz();
            yield return new WaitForSeconds(4);
            ChangeScene("TestScene", 4);
        }

        feedbackText.text = "Koniec gry! Dziękujemy za udział.";
        submitButton.gameObject.SetActive(false);
    }*/

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
        SceneManager.LoadScene(sceneName);
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
    void Update()
    {
       
    }
}