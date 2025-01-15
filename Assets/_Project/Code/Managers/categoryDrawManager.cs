using Newtonsoft.Json;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static Utils;

/// <summary>
/// Klasa odpowiadająca za wylosowanie danej kategorii pytania.
/// </summary>
public class CategoryDrawManager : NetworkBehaviour
{
    /// <summary>
    /// Zmienna reprezentująca konkretną rundę z początkową wartością 0.
    /// </summary>
    private int currentRound = 0;
    /// <summary>
    /// Prywatna zmienna przechowująca obiekt reprezentujący koło.
    /// </summary>
    private Wheel wheel;
    /// <summary>
    /// Zmienna przechowująca referencję do komponentu tekstowego (TMP_Text) odpowiedzialnego za wyświetlanie nazwy kategorii.
    /// Używane do dynamicznego wyświetlania tekstu w interfejsie użytkownika, reprezentującego nazwę kategorii.
    /// </summary>
    private TMP_Text categoryDisplayText;
    /// <summary>
    /// Zmienna przechowująca referencję do komponentu tekstowego (TMP_Text) odpowiedzialnego za wyświetlanie informacji o aktualnej rundzie.
    /// </summary>
    private TMP_Text roundDisplayText;
    /// <summary>
    /// Prywatna zmienna przechowująca obiekt reprezentujący listę kategorii.
    /// </summary>
    private CategoryList categoryList;
    /// <summary>
    /// Zmienna reprezentująca czas rozpoczęcia rundy.
    /// </summary>
    private float startTime;
    /// <summary>
    /// Zmienna reprezentująca informację czy zakręcono kołem.
    /// </summary>
    private bool wheelSpinned;

    public AudioSource audioSpinWheel;
    public AudioSource audioRevealCategory;
    public AudioSource audioVoice1;
    public AudioSource audioVoice2;
    public AudioSource audioVoice3;
    public AudioSource audioVoice3a;
    public AudioSource audioVoice4;

    /// <summary>
    /// Metoda odpowiedzialna za inicjalizację komponentów, załadowanie zasobów.
    /// </summary>
    private void Start()
    {
        wheelSpinned = false;
        if (IsHost)
        {
            TextAsset categoryAssets = Resources.Load<TextAsset>("questions");
            categoryList = JsonConvert.DeserializeObject<CategoryList>(categoryAssets.text);
        }

        wheel = GameObject.Find("Wheel").GetComponent<Wheel>();
        categoryDisplayText = GameObject.Find("CategoryDisplay").GetComponent<TMP_Text>();
        roundDisplayText = GameObject.Find("RoundCounter").GetComponent<TMP_Text>();
        roundDisplayText.text = "Runda: " + GameManager.Instance.Round.Value;
        wheel.OnWheelStopped += HandleWheelStopped;
        startTime = Time.time;
        Invoke("AudioPlaySpinWheel", 1.5f); //delay aby zsynchronizowac z kolem fortuny
        AudioPlayEarlyVoice();
        Invoke("AudioPlayLateVoice", 7.0f);
    }

    private void AudioPlaySpinWheel()
    {
        audioSpinWheel.Play();
    }

    private void AudioPlayEarlyVoice()
    {
        if (GameManager.Instance.Round.Value == 1) audioVoice1.Play();
        else if (GameManager.Instance.Round.Value == 7) audioVoice4.Play();
    }
    private void AudioPlayLateVoice()
    {
        if (GameManager.Instance.Round.Value == 1) audioVoice3.Play();
        else
        {
            float random = Random.value;
            if (random < 0.33) audioVoice3a.Play();
            else if (random > 0.66) audioVoice2.Play();
        }
    }

    /// <summary>
    /// Metoda obsługuję zdarzenie zatrzymania koła i wykonuje odpowiednie działania w zależności od wylosowanej kategorii.
    /// - Wyświetla nazwę wylosowanej kategorii na UI,
    /// - Ustawia odpowiednią kategorię w systemie gry (w tym specjalne kategorie takie jak "Czarna skrzynka" i "Podpowiedź"),
    /// - Zwiększa numer rundy oraz wyświetla go na UI,
    /// - Ładuje nową scenę "BiddingWar", która uruchamia etap licytacji między drużynami.
    /// </summary>
    /// <param name="result">Zmienna reprezentująca indeks wylosowanej kategorii z listy kategorii</param>
    private void HandleWheelStopped(int result)
    { 
        categoryDisplayText.text = "Wylosowano: " + categoryNames[result];
        audioRevealCategory.Play();

        if (categoryNames[result] == "Czarna skrzynka")
        {
            roundDisplayText.text = "Runda Bonusowa";
            if (IsHost)
            {
                GameManager.Instance.Category.Value = new Category("Czarna skrzynka", new System.Collections.Generic.List<Question>());
            }
        }
        else if (categoryNames[result] == "Podpowiedź")
        {
            roundDisplayText.text = "Runda Bonusowa";
            if (IsHost)
            {
                GameManager.Instance.Category.Value = new Category("Podpowiedź", new System.Collections.Generic.List<Question>());
            }
        }
        else
        { 
            
            if (IsHost)
            {    
                GameManager.Instance.Category.Value = categoryList.FindCategory(categoryNames[result]); 
            }
            // WyświetlPytanie(category)
        }

        if (NetworkManager.Singleton.IsHost)
        {
            Invoke("LoadBiddingWar", 3.0f);
        }
    }

    /// <summary>
    /// Metoda odpowiedzialna za zmianę sceny, w której odbywa się licytacja.
    /// </summary>
    private void LoadBiddingWar()
    {
        _ = NetworkManager.Singleton.SceneManager.LoadScene("BiddingWar", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    /// <summary>
    /// Metoda odpowiedzialna za animację koła (wylosowanie następnej kategorii) po spełnieniu określonych warunków.
    /// </summary>
    private void Update()
    {
        if (!wheelSpinned && IsHost && Time.time - startTime > 2)
        {
            wheelSpinned = true;
            CalculateAngle();
        }
    }
    /// <summary>
    /// Metoda ustawiająca losowy kąt obrotu koła w zakresie od 720 do 1440 stopni i wywołuje metodę RPC odpowiedzialną za uruchomienie obrotu koła w tej wartości kąta.
    /// </summary>
    public void CalculateAngle() => SpinWheelRpc(Random.Range(720, 1440));

    /// <summary>
    /// RPC wywoływany na wszystkich graczach, aby wykonąc obrót koła o podany kąt. Metoda wykona się gdy liczba rund nie przekroczy limitu.
    /// </summary>
    /// <param name="angle">Zmienna reprezentująca kąt obrotu koła, który określa, o ile stopni koło ma się obrócić.</param>
    [Rpc(SendTo.Everyone)]
    void SpinWheelRpc(float angle)
    {
        if(GameManager.Instance.Round.Value <= Utils.ROUNDS_LIMIT)
        {
            wheel.SpinWheel(angle);
        }
    }
}
