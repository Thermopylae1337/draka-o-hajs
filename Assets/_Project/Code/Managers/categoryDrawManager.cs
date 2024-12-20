using Newtonsoft.Json;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public class CategoryDrawManager : NetworkBehaviour
{
    private int currentRound = 0;
    private Wheel wheel;
    private TMP_Text categoryDisplayText;
    private TMP_Text roundDisplayText;
    private GameManager gameManager;
    private CategoryList categoryList;
    private float startTime;
    private bool wheelSpinned;

    private void Start()
    {
        wheelSpinned = false;
        if (IsHost)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            TextAsset categoryAssets = Resources.Load<TextAsset>("questions");
            categoryList = JsonConvert.DeserializeObject<CategoryList>(categoryAssets.text);
        }

        wheel = GameObject.Find("Wheel").GetComponent<Wheel>();
        categoryDisplayText = GameObject.Find("CategoryDisplay").GetComponent<TMP_Text>();
        roundDisplayText = GameObject.Find("RoundCounter").GetComponent<TMP_Text>();

        wheel.OnWheelStopped += HandleWheelStopped;
        startTime = Time.time;
    }

    private void HandleWheelStopped(int result)
    {
        categoryDisplayText.text = "Wylosowano: " + categoryNames[result];

        if (categoryNames[result] == "Czarna skrzynka")
        {
            gameManager.Category.Value = new Category("Czarna skrzynka", new System.Collections.Generic.List<Question>());
        }
        else if (categoryNames[result] == "Podpowiedź")
        {
            gameManager.Category.Value = new Category("Podpowiedź", new System.Collections.Generic.List<Question>());
        }
        else
        {
            currentRound++;
            roundDisplayText.text = "Runda: " + currentRound;
            if (IsHost)
            {
                gameManager.Category.Value = categoryList.FindCategory(categoryNames[result]);
            }
            // WyświetlPytanie(category)
        }

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

    public void CalculateAngle() => SpinWheelRpc(Random.Range(500, 1500));

    [Rpc(SendTo.Everyone)]
    void SpinWheelRpc(float angle)
    {
        if (currentRound < Utils.ROUNDS_LIMIT)
        {
            wheel.SpinWheel(angle);
        }
    }
}
