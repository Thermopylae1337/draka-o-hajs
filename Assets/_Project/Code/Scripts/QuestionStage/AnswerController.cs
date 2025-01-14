using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// Klasa odpowiedzialna za zarządzanie odpowiedziami.
/// </summary>
public class AnswerController : NetworkBehaviour
{
    /// <summary>
    /// Zmienna typu TMP_Text reprezentująca całkowite podbicie podczas.
    /// </summary>
    public TMP_Text totalBid;
    /// <summary>
    /// Zmienna reprezentująca cenę podpowiedzi.
    /// </summary>
    public TMP_Text hintPriceText;
    /// <summary>
    /// Zmienna reprezentująca pole, w którym drużyna odpowiada na pytanie.
    /// </summary>
    public TMP_InputField answerInput;
    /// <summary>
    /// Zmienna reprezentująca treść pytania.
    /// </summary>
    public TMP_Text questionText;
    /// <summary>
    /// Zmienna reprezentująca informację zwrotną.
    /// </summary>
    public TMP_Text feedbackText;
    /// <summary>
    /// Zmienna reprezentująca czasomierz.
    /// </summary>
    public TMP_Text timerText;
    /// <summary>
    /// Obiekt reprezentujący przycisk zatwierdzenia.
    /// </summary>
    public Button submitButton;
    /// <summary>
    /// Zmienna przechowująca informacje o numer rundy.
    /// </summary>
    public TMP_Text roundNumber;
    public GameObject hintButtonsContainer;
    /// <summary>
    /// Obiekt reprezentujący przycisk pozwalający skorzystać z podpowiedzi.
    /// </summary>
    public Button useHintsButton;

    public Image backgroundImage;
    public Image answerImage;
    public Image resultImage;
    public Image questionBackgroundImage;
    public Sprite artYellowTeamAnswering;
    public Sprite artGreenTeamAnswering;
    public Sprite artBlueTeamAnswering;
    public Sprite artQuestionBackgroundYellow;
    public Sprite artQuestionBackgroundGreen;
    public Sprite artQuestionBackgroundBlue;
    public Sprite artResultWrong;
    public AudioSource audioAnswerCorrect;
    public AudioSource audioAnswerWrong;
    public AudioSource audioMusic;
    public AudioSource audioVoice8;
    public AudioSource audioVoice9;
    public AudioSource audioVoice14;
    public AudioSource audioVoice6;

    /// <summary>
    /// Tablica zawierająca przyciski z podanymi odpowiedziami.
    /// </summary>

    private Button[] answerButtons;
    public static int currentQuestionIndex = 0;
    /// <summary>
    /// Zmienna reprezentująca czas jaki pozostał na udzielenie odpowiedzi.
    /// </summary>
    private float _timeRemaining;
    /// <summary>
    /// Zmienna informująca czy odpowiedź została sprawdzona.
    /// </summary>
    private bool _isAnswerChecked;
    /// <summary>
    /// Tablica podpowiedzi.
    /// </summary>
    private string[] hints;
    /// <summary>
    /// Zmienna reprezentująca losową cenę za podpowiedź.
    /// </summary>
    private int randomHintPrice;

    /// <summary>
    /// Obiekt statyczny reprezentujący kategorię pytania.
    /// </summary>
    public static Category category;
    /// <summary>
    /// Obiekt reprezentujący aktualne pytanie.
    /// </summary>
    public Question currentQuestion;
    /// <summary>
    /// Lista przechowująca drużyny.
    /// </summary>
    private List<TeamManager> _teams;
    /// <summary>
    /// Zmienna reprezentująca ilość drużyn w bieżącej rundzie.
    /// </summary>
    private uint _teamsInGame;

    /// <summary>
    /// Metoda przygotowuje wszystkie niezbędne elementy do rozpoczęcia gry (wyświetlanie tła, ustawianie puli pytania, inicjalizację przycisków odpowiedzi, przypisanie akcji do przycisków, przygotowanie informacji o zespołach oraz dostosowanie interfejsu w zależności od roli gracza (wygrana przegrana licytacja)).
    /// </summary>
    private void Start()
    {
        ShowBackgroundImages();
        Invoke("PlayVoiceTimeRemaining", 15.0f);
        audioMusic.Play();
        totalBid.text = "PULA: " + GameManager.Instance.CurrentBid.Value.ToString();
        answerButtons = hintButtonsContainer.GetComponentsInChildren<Button>();
        _isAnswerChecked = false;
        SetHintMode(false);
        _teams = NetworkManager.Singleton.ConnectedClients.Select((teamClient) => teamClient.Value.PlayerObject.GetComponent<TeamManager>()).ToList(); 
        foreach (Button button in answerButtons)
        {
            button.onClick.AddListener(() => OnSelectButton(button));
        }

        if (IsHost)
        {
            QuestionAskedIncrementServerRpc();

            SetCategoryServerRpc();
            StartRoundServerRpc();
        }
        //orygianlnie miało to być w SendQuestionToClientRpc ale wtedy wyskakiwał
        //NullReferenceException: Object reference not set to an instance of an object (dla GameManagera)
        //ale tylko dla klientów, nie dla hosta, więc najprawdopodobniej GameManager nie był dostatecznie szybko przez nich ładowany
        //ewentualnie może zawsze widnieć koszt podpowiedzi ale wydaje mi się, że lepiej żeby pokazane było ile drużyna ma podpowiedzi (jeżeli ma)
        if (_teams[(int)GameManager.Instance.Winner.Value].Clues > 0)
        {
            hintPriceText.text = "Liczba podpowiedzi:" + Convert.ToString(_teams[(int)GameManager.Instance.Winner.Value].Clues);
        }

        feedbackText.text = GameManager.Instance.Winner.Value == NetworkManager.Singleton.LocalClientId
            ? "Wygrałeś(aś) licytację. Odpowiadasz na pytanie."
            : "Przegrałeś(aś) licytację. Jesteś obserwatorem.";
    }

    private void PlayVoiceTimeRemaining()
    {
        if (!_isAnswerChecked && GameManager.Instance.Winner.Value == NetworkManager.Singleton.LocalClientId)
        {
            audioVoice6.Play();
        }
    }
    /// <summary>
    /// Metoda ustawiająca tło w zależności od koloru zespołu, który udziela odpowiedzi. 
    /// </summary>
    private void ShowBackgroundImages()

    {
        TeamManager _answeringTeam = NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>();
        ColourEnum _colour = _answeringTeam.Colour;

        switch (_colour)
        {
            case ColourEnum.YELLOW:
                backgroundImage.sprite = artYellowTeamAnswering;
                questionBackgroundImage.sprite = artQuestionBackgroundYellow;
                break;
            case ColourEnum.GREEN:
                backgroundImage.sprite = artGreenTeamAnswering;
                questionBackgroundImage.sprite = artQuestionBackgroundGreen;
                break;
            case ColourEnum.BLUE:
                backgroundImage.sprite = artBlueTeamAnswering;
                questionBackgroundImage.sprite = artQuestionBackgroundBlue;
                break;
        }
    }

    /// <summary>
    /// Metoda uruchamiająca odliczanie czasu, która aktualizuje pozostały czas i wyświetla go na interfejsie użytkownika.
    /// Po upływie czasu, interakcje z elementami UI zostają wyłaczone, a na ekranie wyświetlana jest odpowiedź. W zależności od poprawności odpowiedzi następuje zmiana sceny. 
    /// </summary>
    /// <returns></returns>
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
            resultImage.gameObject.SetActive(true);
            resultImage.sprite = artResultWrong;
            feedbackText.text = "Czas minął! Odpowiedzi: " + string.Join(", ", currentQuestion.CorrectAnswers);
            _ =  IsContinuingGamePossible()
                ? StartCoroutine(ChangeScene("CategoryDraw", 4))
                : StartCoroutine(ChangeScene("Summary", 4));
        }
    }

    /// <summary>
    /// Metoda sprawdzająca odpowiedź gracza, pobierając ją z pola tekstowego i przesyłając na serwer.
    /// </summary>
    public void CheckAnswer()
    {
        SetItemsInteractivity(false);
        _isAnswerChecked = true;
        string playerAnswer = answerInput.text.Trim();
        CheckAnswerServerRpc(playerAnswer);
        NotifyAnswerCheckedServerRpc();
    }

    /// <summary>
    ///  Metoda sprawdzająca odpowiedź gracza, przekazaną jako argument, i przesyłająca ją na serwer.
    /// </summary>
    /// <param name="playerAnswer">Zmienna przechowująca odpowiedź gracza do sprawdzenia.</param>
    public void CheckAnswer(string playerAnswer)
    {
        SetItemsInteractivity(false);
        _isAnswerChecked = true;
        CheckAnswerServerRpc(playerAnswer);
        NotifyAnswerCheckedServerRpc();
    }

    /// <summary>
    /// Metoda RPC wywoływana na serwerze, aby zaktualizować stan odpowiedzi, a także powiadomić klientów o jej sprawdzeniu.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void NotifyAnswerCheckedServerRpc()
    {

        _isAnswerChecked = true;
        NotifyClientsAnswerCheckedRpc();
    }

    /// <summary>
    /// Metoda RPC wywoływana na klientach i hoście, aby zaktualizować stan odpowiedzi na wszystkich urządzeniach.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyClientsAnswerCheckedRpc()
    {
        _isAnswerChecked = true;
    }

    /// <summary>
    /// Metoda RPC wywoływana na serwerze w celu sprawdzenia odpowiedzi gracza.
    /// </summary>
    /// <param name="playerAnswer">Zmienna reprezentująca odpowiedź gracza.</param>
    [ServerRpc(RequireOwnership = false)]
    private void CheckAnswerServerRpc(string playerAnswer)
    {
        if (currentQuestion.IsCorrect(playerAnswer))
        {
            if(IsHost)
            {
                if (_timeRemaining > 27f)
                {
                    NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().CzasToPieniadz = true;
                }

                QuestionAnsweredIncrementServerRpc();
            }

            NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money += GameManager.Instance.CurrentBid.Value;
            GameManager.Instance.CurrentBid.Value = 0;
            SendFeedbackToClientsRpc("Brawo! Poprawna odpowiedź.", IsContinuingGamePossible(), true);
        }
        else
        {
            SendFeedbackToClientsRpc($"Niestety, to nie jest poprawna odpowiedź. " + 
                $"Poprawna odpowiedź to: "+currentQuestion.CorrectAnswers[0],
                IsContinuingGamePossible(), false);
 
            if (_teams[(int)GameManager.Instance.Winner.Value].Money <= 500)
            {
                //dodane po to, żeby nie sprawdzać na początku każdej rundy licytacji kto ma <600zł
                _teams[(int)GameManager.Instance.Winner.Value].InGame = false;
            }
        }
    }

    /// <summary>
    ///  Metoda RPC wysyłająca informacje zwrotne do wszystkich graczy oraz hosta, a także decydująca o dalszym przebiegu gry.
    /// </summary>
    /// <param name="feedback">Zmienna zawierająca informację zwrotną, która będzie wyświetlana drużynie.</param>
    /// <param name="gameContinuing">Flaga wskazująca, czy gra ma być kontynuowana czy zakończona.</param>
    [Rpc(SendTo.ClientsAndHost)]
    private void SendFeedbackToClientsRpc(string feedback, bool gameContinuing, bool correctAnswer)
    {
        resultImage.gameObject.SetActive(true);
        audioMusic.mute = true;
        audioVoice6.Stop();
        if (!correctAnswer)
        {
            resultImage.sprite = artResultWrong;
            audioAnswerWrong.Play();
            Invoke("PlayVoiceWrongAnswer", 1.0f);
        } else
        {
            audioAnswerCorrect.Play();
            Invoke("PlayVoiceCorrectAnswer", 0.5f);
        }

        if(IsHost)
        {
            if (GameManager.Instance.Round.Value <= 1 && NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money <= 0)
            {
                NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Bankruci = true;
            }
        }

        if (gameContinuing)
        {
            feedbackText.text = feedback;
            _ = StartCoroutine(ChangeScene("CategoryDraw", 4));
            if (IsHost)
            {
                GameManager.Instance.Round.Value += 1;
            }
        }
        else
        {
            feedbackText.text = feedback;
            _ = StartCoroutine(ChangeScene("Summary", 4));
        }
    }


    private void PlayVoiceCorrectAnswer()
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.5) audioVoice8.Play();
        else audioVoice14.Play();
    }
    private void PlayVoiceWrongAnswer()
    {
        audioVoice9.Play();
    }
    
    /// <summary>
    /// Metoda wywołująca zapytanie o podpowiedź, przekazująca kontrolę do serwera, aby sprawdzić, czy gracz może skorzystać z podpowiedzi.
    /// </summary>
    public void AskForHint() => UseHintNotifyServerRpc();

    /// <summary>
    ///  Metoda RPC wywoływana na serwerze w celu sprawdzenia, czy drużyna ma wystarczająco dużo pieniędzy, aby wykupić podpowiedź.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void UseHintNotifyServerRpc()
    { 
        if (NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money >= randomHintPrice || ( _teams[(int)GameManager.Instance.Winner.Value].Clues>0 )  )
        {
            if (_teams[(int)GameManager.Instance.Winner.Value].Clues > 0)
            {
                _teams[(int)GameManager.Instance.Winner.Value].Clues -= 1;
            }
            else
            { 
                NetworkManager.Singleton.ConnectedClients[GameManager.Instance.Winner.Value].PlayerObject.GetComponent<TeamManager>().Money -= randomHintPrice;
            }

             _teams[(int)GameManager.Instance.Winner.Value].CluesUsed++;
            hints = currentQuestion.Hints;
            ShowHintRpc(hints[0], hints[1], hints[2], hints[3]);
            _timeRemaining = 30f;
        }
        else
        {
            HintAskRejectionRpc();
        }
    }

    /// <summary>
    ///  Metoda RPC wywoływana w przypadku gdy gracz nie może sobie pozwolić na podpowiedź. Ustawia tekst zwrotny w interfejsie użytkownika.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    private void HintAskRejectionRpc()
    {
        hintPriceText.text = "Nie stać Cię na podpowiedź";
    }

    /// <summary>
    /// Metoda RPC wywoływana w celu wyświetlenia podpowiedzi na przyciskach odpowiedzi. Ustawia tryb podpowiedzi oraz przypisuje teksty do czterech przycisków odpowiedzi.
    /// </summary>
    /// <param name="h1">Zmienna przechowująca pierwszą podpowiedź wyświetlaną na przycisku 1.</param>
    /// <param name="h2">Zmienna przechowująca drugą podpowiedź wyświetlaną na przycisku 2.</param>
    /// <param name="h3">Zmienna przechowująca trzecią podpowiedź wyświetlaną na przycisku 3.</param>
    /// <param name="h4">Zmienna przechowująca czwartą podpowiedź wyświetlaną na przycisku 4.</param>
    [Rpc(SendTo.ClientsAndHost)]
    private void ShowHintRpc(string h1, string h2, string h3, string h4)
    {
        SetHintMode(true);
        answerButtons[0].GetComponentInChildren<TMP_Text>().text = h1;
        answerButtons[1].GetComponentInChildren<TMP_Text>().text = h2;
        answerButtons[2].GetComponentInChildren<TMP_Text>().text = h3;
        answerButtons[3].GetComponentInChildren<TMP_Text>().text = h4;
        hintPriceText.text = "";
    }
    /// <summary>
    /// Metoda ustawiająca tryb podpowiedzi w grze.
    /// </summary>
    /// <param name="active">Zmienna aktywacji trybu podpowiedzi.</param>
    private void SetHintMode(bool active)
    {
        if (active)
        {
            SetButtonsDefaultColor();
        }

 
        answerInput.gameObject.SetActive(!active);
        answerImage.gameObject.SetActive(!active); 
        hintButtonsContainer.SetActive(active);
        useHintsButton.gameObject.SetActive(!active);
    }

    /// <summary>
    /// Metoda obsługująca kliknięcie przycisku odpowiedzi.
    /// </summary>
    /// <param name="button">Obiekt reprezentujący przycisk.</param>
    private void OnSelectButton(Button button)
    {
        SetButtonsDefaultColor();
        button.GetComponent<Image>().color = Color.blue;
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        string buttonValue = buttonText.text;
        CheckAnswer(buttonValue);
    }

    /// <summary>
    /// Metoda ustawiająca domyślny kolor przycisku.
    /// </summary>
    private void SetButtonsDefaultColor()
    {
        foreach (Button button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// Metoda zmieniająca scenę po określonym czasie.
    /// </summary>
    /// <param name="sceneName">Zmienna przechowująca nazwę sceny.</param>
    /// <param name="time">Zmienna reprezentująca czas.</param>
    /// <returns></returns>
    private IEnumerator ChangeScene(string sceneName, float time)
    {
        yield return new WaitForSeconds(time);
        _ = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// Metoda służąca do ustawiania interaktywności wszystkich elementów odpowiedzi (przycisków, pola tekstowego i przycisku zatwierdzenia). W zależności od przekazanej wartości, metoda włącza lub wyłącza interakcję z tymi elementami.
    /// </summary>
    /// <param name="set">Zmienna przechowująca wartość określająca czy elementy mają być interaktywne czy nie.</param>
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

    /// <summary>
    /// Metoda RPC, która wysyła dane pytania do wszystkich klientów.
    /// </summary>
    /// <param name="questionText">Zmienna przechowująca treść wylosowanego pytania.</param>
    /// <param name="currentQuestionIndex"></param>
    /// <param name="hintPrice">Zmienna przechowująca cenę podpowiedzi.</param>
    [Rpc(SendTo.Everyone)]
    public void SendQuestionToClientRpc(string questionText, int currentQuestionIndex, float hintPrice)
    {
        answerButtons = hintButtonsContainer.GetComponentsInChildren<Button>();
        SetItemsInteractivity(false); 
        this.questionText.text = questionText; 
        hintPriceText.text = "Cena: " + Convert.ToString(hintPrice) + " PLN";  
        _isAnswerChecked = false;
        roundNumber.text = "PYTANIE " + currentQuestionIndex.ToString();
        feedbackText.text = "";
        answerInput.text = "";
        timerText.text = "";
    }

    /// <summary>
    /// Metoda RPC, ustawiająca kategorie wylosowanego pytania.
    /// </summary>
    [Rpc(SendTo.Server)]
    private void SetCategoryServerRpc()
    {
        category = GameManager.Instance.Category.Value;
        currentQuestion = category.DrawQuestion();
    }

    /// <summary>
    /// Metoda RPC uruchamiająca nową rundę w grze.
    /// Zwiększa numer pytania, ustawia cenę podpowiedzi, wysyła pytanie do klientów oraz rozpoczyna odliczanie czasu. Jeśli liczba rund przekroczy limit, kończy grę.
    /// </summary>
    [Rpc(SendTo.Server)]
    private void StartRoundServerRpc()
    { 
        if (GameManager.Instance.Round.Value <= Utils.ROUNDS_LIMIT)
        {
            randomHintPrice = Convert.ToInt32(Mathf.Round(Convert.ToSingle(UnityEngine.Random.Range(20, 31)) / 100 * GameManager.Instance.CurrentBid.Value / 100f) * 100f);
            SendQuestionToClientRpc(currentQuestion.Content, GameManager.Instance.Round.Value, randomHintPrice);
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
    /// <summary>
    /// Metoda RPC, która aktualizuje wyświetlany czas pozostały drużynie na odpowiedź.
    /// </summary>
    /// <param name="timeRemaining">Zmienna przechowująca pozostały czasu.</param>
    [Rpc(SendTo.ClientsAndHost)]
    private void ShowCurrentTimeRpc(float timeRemaining) => timerText.text = "Czas " + Mathf.Ceil(timeRemaining) + "s";

    /// <summary>
    /// Metoda RPC, która umożliwia włączenie trybu odpowiedzi dla drużyny, która wygrała licytacje pytania.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    private void AnsweringModeRpc()
    {
        if (NetworkManager.Singleton.LocalClientId == GameManager.Instance.Winner.Value)
        {
            SetItemsInteractivity(true);
        }
    }
    /// <summary>
    /// Metoda sprawdzająca czy drużyna posiada wystarczająco pieniędzy aby wejść do następnej rundy.
    /// </summary>
    /// <returns>True, jeśli przynajmniej dwie drużyny mają wystarczająco dużo pieniędzy, False w przeciwnym razie. </returns>
    private bool IsContinuingGamePossible()
    { 
        if (GameManager.Instance.Round.Value >= Utils.ROUNDS_LIMIT) return false;
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

    [Rpc(SendTo.Server)]
    private void QuestionAnsweredIncrementServerRpc()
    {
        _teams[(int)GameManager.Instance.Winner.Value].QuestionsAnswered++;
    }

    /// <summary>
    /// Metoda RPC wysyłająca na serwer zwiększenie licznika przechowującego ilość zadanych pytań drużynie.
    /// </summary>
    [Rpc(SendTo.Server)]
    private void QuestionAskedIncrementServerRpc()
    {
        _teams[(int)GameManager.Instance.Winner.Value].QuestionsAsked++;
    }
}
