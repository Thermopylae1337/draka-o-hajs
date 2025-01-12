using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEditor;

public class BadgeSpawner : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> badgesSprites = new List<Sprite> ();

    public GameObject badgePrefab;
    public Transform contentParent;
    public Sprite basicSprite;
    public TMP_Dropdown dropdown;
    LeaderboardList leaderboard = new();

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
    public void ClearBadges()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

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

    private void ShowDescription(GameObject badgeObject)
    {
        Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
        badgeDescriptionBackground.color = Color.black;
        TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
        badgeDescription.gameObject.SetActive(true);
    }

    private void HideDescription(GameObject badgeObject)
    {
        Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
        badgeDescriptionBackground.color = Color.clear;
        TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
        badgeDescription.gameObject.SetActive(false);
    }
}
