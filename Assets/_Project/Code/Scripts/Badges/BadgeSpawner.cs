using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BadgeSpawner : MonoBehaviour
{
    public GameObject badgePrefab;
    public Transform contentParent;
    public Sprite basicSprite;
    public class TemporaryBadgeClass
    {
        string title;
        string description;
        bool isUnlocked;
        Sprite sprite;
        public TemporaryBadgeClass(string title, string description, Sprite sprite, bool isUnlocked)
        {
            this.title = title;
            this.description = description;
            this.sprite = sprite;
            this.isUnlocked = isUnlocked;
        }
        public bool IsUnlocked
        {
            get => this.isUnlocked;
        }
        public string Title
        {
            get => this.title;
        }
        public string Description
        {
            get => this.description;
        }
        public Sprite Sprite
        {
            get=>this.sprite;
        }
    }
    public List<TemporaryBadgeClass> badges = new List<TemporaryBadgeClass>();
    void Start()
    {
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", basicSprite, false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", basicSprite, true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", basicSprite, true));
        GenerateBadges();
    }

    void GenerateBadges()
    {
        foreach (var badge in badges)
        {
            GameObject badgeObject = Instantiate(badgePrefab,contentParent);
            Image badgeImage = badgeObject.transform.Find("BadgeImage").GetComponent<Image>();
            TextMeshProUGUI badgeText = badgeObject.transform.Find("BadgeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
            Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
            badgeDescriptionBackground.color = Color.clear;
            badgeDescription.gameObject.SetActive(false);
            badgeDescription.text = badge.Description;
            badgeImage.sprite = badge.Sprite;
            badgeText.text = badge.Title;
            if(badge.IsUnlocked)
            {
                badgeImage.color = Color.white;
            }
            else
            {
                badgeImage.color = Color.gray;
            }
            AddHoverEvents(badgeObject, badge.Description);
        }
    }
    void AddHoverEvents(GameObject badgeObject, string badgeDescription)
    {
        EventTrigger trigger = badgeObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = badgeObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => ShowDescription(badgeObject));

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => HideDescription(badgeObject));

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    void ShowDescription(GameObject badgeObject)
    {
        Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
        badgeDescriptionBackground.color = Color.black;
        TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
        badgeDescription.gameObject.SetActive(true);
    }

    void HideDescription(GameObject badgeObject)
    {
        Image badgeDescriptionBackground = badgeObject.transform.Find("BadgeDescriptionBackground").GetComponent<Image>();
        badgeDescriptionBackground.color = Color.clear;
        TextMeshProUGUI badgeDescription = badgeObject.transform.Find("BadgeDescription").GetComponent<TextMeshProUGUI>();
        badgeDescription.gameObject.SetActive(false);
    }
}
