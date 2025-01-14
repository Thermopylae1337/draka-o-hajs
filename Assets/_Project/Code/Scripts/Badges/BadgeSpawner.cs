using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEditor;

/// <summary>
/// Klasa odpowiedzialna za zarządzanie i tworzenie odznak w grze.
/// </summary>
public class BadgeSpawner : MonoBehaviour
{
    /// <summary>
    /// Lista sprite'ów odznak przypisanych w edytorze Unity.
    /// </summary>
    [SerializeField]
    public List<Sprite> badgesSprites = new List<Sprite> ();

    /// <summary>
    /// Prefab odznaki, który jest instancjonowany w grze.
    /// </summary>
    public GameObject badgePrefab;
    /// <summary>
    /// Kontener UI, do którego dodawane będą stworzone odznaki.
    /// </summary>
    public Transform contentParent;
    /// <summary>
    /// Domyślny sprite odznaki
    /// </summary>
    public Sprite basicSprite;
    public TMP_Dropdown dropdown;
    LeaderboardList leaderboard = new();

    /// <summary>
    /// Metoda dodająca na starcie kilka odznak do listy.
    /// </summary>
    private void Start()
    {
        leaderboard.Deserializuj();
        List<LeaderboardTeam> teams = leaderboard.TeamList;
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (LeaderboardTeam team in teams)
        {
            options.Add(team.Name);
        }
        if(options.Count == 0)
        {
            options.Add("Brak drużyn");
        }

        dropdown.AddOptions(options);
        GenerateBadges();
    }

    /// <summary>
    /// Tworzy odznaki na podstawie danych przechowywanych w liście 'badges'
    /// Dla każdej odznaki generowany jest obiekt UI (zawierający obrazek, tytuł i opis), a także przypisywane są odpowiednie dane oraz kolory w zależności od stanu odznaki.
    /// </summary>
    public void GenerateBadges()
    {
        ClearBadges();
        List<LeaderboardTeam> teams = leaderboard.TeamList;
        int counter = 0;
        foreach(LeaderboardTeam team in teams)
        {
            Debug.Log(team.Name);
            foreach(Badge b in team.Badges)
            {
                if(b.Unlocked==true)
                    Debug.Log(b.Name+" "+b.Unlocked);
            }
        }
        foreach (Badge badge in leaderboard.FindTeam(dropdown.options[dropdown.value].text).Badges)
        {
            GameObject badgeObject = Instantiate(badgePrefab, contentParent);
            Image badgeImage = badgeObject.transform.Find("BadgeImage").GetComponent<Image>();
            TextMeshProUGUI badgeText = badgeObject.transform.Find("BadgeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
            Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
            badgeDescriptionBackground.color = Color.clear;
            badgeDescription.gameObject.SetActive(false);
            badgeDescription.text = badge.UnlockCondition;
            badgeImage.sprite = badgesSprites[counter];
            counter++;
            badgeText.text = badge.Name;
            badgeImage.color = badge.Unlocked ? Color.white : Color.gray;
            AddHoverEvents(badgeObject, badge.UnlockCondition);
        }
    }
    /// <summary>
    /// Metoda usuwająca odznaki.
    /// </summary>
    public void ClearBadges()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Dodaje zdarzenia interakcji (hover) do obiektu odznaki, umożliwiając wyświetlanie opisu odznaki podczas najechania myszką (PointerEnter) oraz ukrywanie opisu po jej opuszczeniu (PointerExit).
    /// </summary>
    /// <param name="badgeObject">Zmienna reprezentująca obiekt odznaki.</param>
    /// <param name="badgeDescription">Zmienna opisująca odznaki, które będą wyświetlane podczas najechania na obiekt.</param>
    private void AddHoverEvents(GameObject badgeObject, string badgeDescription)
    {
        EventTrigger trigger = badgeObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = badgeObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryEnter = new()
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((eventData) => ShowDescription(badgeObject));

        EventTrigger.Entry entryExit = new()
        {
            eventID = EventTriggerType.PointerExit
        };
        entryExit.callback.AddListener((eventData) => HideDescription(badgeObject));

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    /// <summary>
    /// Metoda wyświetlająca opis danej odznaki (np. w jaki sposób ją zdobyć).
    /// </summary>
    /// <param name="badgeObject">Obiekt odznaki, którego opis ma zostać pokazany.</param>
    private void ShowDescription(GameObject badgeObject)
    {
        Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
        badgeDescriptionBackground.color = Color.black;
        TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
        badgeDescription.gameObject.SetActive(true);
    }

    /// <summary>
    /// Metoda ukrywająca opis danej odznaki.
    /// </summary>
    /// <param name="badgeObject">Obiekt odznaki, którego opis ma zostać ukryty.</param>
    private void HideDescription(GameObject badgeObject)
    {
        Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
        badgeDescriptionBackground.color = Color.clear;
        TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
        badgeDescription.gameObject.SetActive(false);
    }
}
