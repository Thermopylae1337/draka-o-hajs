using Newtonsoft.Json;
using TMPro;
using Unity.Netcode;
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
    }

    private void HandleWheelStopped(int result)
    { 
        categoryDisplayText.text = "Wylosowano: " + categoryNames[result];

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
        wheel.SpinWheel(angle);
    }
}
