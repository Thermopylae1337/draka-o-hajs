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
    public class TemporaryBadgeClass
    {
        public TemporaryBadgeClass(string title, string description, Sprite sprite, bool isUnlocked)
        {
            Title = title;
            Description = description;
            Sprite = sprite;
            IsUnlocked = isUnlocked;
        }
        public bool IsUnlocked { get; }
        public string Title { get; }
        public string Description { get; }
        public Sprite Sprite { get; }
    }
    public List<TemporaryBadgeClass> badges = new();

    private void Start()
    {
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", badgesSprites[0], true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", badgesSprites[1], false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", badgesSprites[2], true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", badgesSprites[3], false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", badgesSprites[4], true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", badgesSprites[5], true));
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", badgesSprites[6], true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", badgesSprites[7], false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", badgesSprites[8], true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", badgesSprites[9], false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", badgesSprites[10], true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", badgesSprites[11], true));
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", badgesSprites[12], true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", badgesSprites[13], false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", basicSprite, true));
        GenerateBadges();
    }

    private void GenerateBadges()
    {
        foreach (TemporaryBadgeClass badge in badges)
        {
            GameObject badgeObject = Instantiate(badgePrefab, contentParent);
            Image badgeImage = badgeObject.transform.Find("BadgeImage").GetComponent<Image>();
            TextMeshProUGUI badgeText = badgeObject.transform.Find("BadgeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
            Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
            badgeDescriptionBackground.color = Color.clear;
            badgeDescription.gameObject.SetActive(false);
            badgeDescription.text = badge.Description;
            badgeImage.sprite = badge.Sprite;
            badgeText.text = badge.Title;
            badgeImage.color = badge.IsUnlocked ? Color.white : Color.gray;
            AddHoverEvents(badgeObject, badge.Description);
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
