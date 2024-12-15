using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

public class BadgeSpawner : MonoBehaviour
{
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
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/1.png"), true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/2.png"), false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/3.png"), true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/4.png"), false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/5.png"), true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/6.png"), true));
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/7.png"), true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/8.png"), false));
        badges.Add(new TemporaryBadgeClass("aaaaaaaaaa", "jeszcze dluzszy od poprzedniego opis odznaki 3", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/9.png"), true));
        badges.Add(new TemporaryBadgeClass("123456897", "opis odznaki 4", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/10.png"), false));
        badges.Add(new TemporaryBadgeClass("pomidor", "opis odznaki 5", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/11.png"), true));
        badges.Add(new TemporaryBadgeClass("odzn6", "opis odznaki 6", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/12.png"), true));
        badges.Add(new TemporaryBadgeClass("odzn1", "opis odznaki 1", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/13.png"), true));
        badges.Add(new TemporaryBadgeClass("odznaka2", "troche dluzszy opis odznaki 2", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Badges/14.png"), false));
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
