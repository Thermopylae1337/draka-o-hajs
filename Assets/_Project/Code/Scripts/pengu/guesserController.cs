using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class guesser : MonoBehaviour
{
    public TMP_Text text;

    private readonly System.Random generator = new System.Random();

    private int guess_top, guess_bottom, guess = -1;
    // Start is called before the first frame update
    void Start()
    {
        OnNewGame();
    }

    private void OnNewGame(string prefix = "")
    {
        guess_top = -1;
        guess_bottom = -1;
        guess = generator.Next(1, 100);
        text.text = $"{prefix}Spróbujmy {guess}";
    }

    public void OnGuessHigher()
    {
        guess_bottom = guess;
        if (IsInvalidGuess()) return;

        if (guess_top != -1)
            guess = generator.Next(guess_bottom, guess_top);
        else
            guess = generator.Next(guess_bottom, guess + 100);
        text.text = $"Aha troche nie za duża?? Spróbujmy {guess}";
    }

    public void OnGuessLower()
    {
        if (IsInvalidGuess()) return;
        guess_top = guess;
        if (guess_bottom != -1)
            guess = generator.Next(guess_bottom, guess_top);
        else
            guess = generator.Next(guess_top - 100, guess_top);
        text.text = $"Mniej?? No to niech będzie {guess}";
    }

    private bool IsInvalidGuess()
    {
        if (guess_top == guess_bottom && guess_top != -1)
        {
            OnNewGame("Oszukiwałeś! Jeszcze raz. ");
            return true;
        }
        return false;
    }

    public void OnGuessCorrect()
    {
        OnNewGame("Łatwo, nowa gra? Uwaga... ");
    }
}
