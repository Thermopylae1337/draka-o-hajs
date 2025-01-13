using Newtonsoft.Json;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static Utils;

public class CategoryDrawManager : NetworkBehaviour
{
 
    private Wheel wheel;
    private TMP_Text categoryDisplayText;
    private TMP_Text roundDisplayText;
    private CategoryList categoryList;
    private float startTime;
    private bool wheelSpinned;

    public AudioSource audioSpinWheel;
    public AudioSource audioRevealCategory;
    public AudioSource audioVoice1;
    public AudioSource audioVoice2;
    public AudioSource audioVoice3;
    public AudioSource audioVoice3a;
    public AudioSource audioVoice4;

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

    private void LoadBiddingWar()
    {
        _ = NetworkManager.Singleton.SceneManager.LoadScene("BiddingWar", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void Update()
    {
        if (!wheelSpinned && IsHost && Time.time - startTime > 2)
        {
            wheelSpinned = true;
            CalculateAngle();
        }
    }

    public void CalculateAngle() => SpinWheelRpc(Random.Range(720, 1440));

    [Rpc(SendTo.Everyone)]
    void SpinWheelRpc(float angle)
    {
        if(GameManager.Instance.Round.Value <= Utils.ROUNDS_LIMIT)
        {
            wheel.SpinWheel(angle);
        }
    }
}
