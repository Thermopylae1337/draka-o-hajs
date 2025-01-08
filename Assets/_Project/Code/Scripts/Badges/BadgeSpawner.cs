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
    /// <summary>
    /// Tymczasowa klasa odznak.
    /// </summary>
    public class TemporaryBadgeClass
    {
        /// <summary>
        /// Konstruktor kopiujący inicjalizujący wszystkie pola składowe klasy.
        /// </summary>
        /// <param name="title">Przechowuje tytuł odznaki.</param>
        /// <param name="description">Przechowuje opis danej odznaki.</param>
        /// <param name="sprite"></param>
        /// <param name="isUnlocked">Przechowuje informacje czy odznaka została odblokowana.</param>
        public TemporaryBadgeClass(string title, string description, Sprite sprite, bool isUnlocked)
        {
            Title = title;
            Description = description;
            Sprite = sprite;
            IsUnlocked = isUnlocked;
        }
        /// <summary>
        /// Zmienna przechowująca informacje czy odznaka jest odblokowana.
        /// </summary>
        public bool IsUnlocked { get; }
        /// <summary>
        /// Zmienna przechowująca tytuł odznaki.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Zmienna przechowująca opis (odblokowania) odznaki.
        /// </summary>
        public string Description { get; }
        public Sprite Sprite { get; }
    }
    /// <summary>
    /// Lista zainicjowana z domyślną wartością null, która będzie przechowywać odznaki.
    /// </summary>
    public List<TemporaryBadgeClass> badges = new();

    /// <summary>
    /// Metoda dodająca na starcie kilka odznak do listy.
    /// </summary>
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

    /// <summary>
    /// Tworzy odznaki na podstawie danych przechowywanych w liście 'badges'
    /// Dla każdej odznaki generowany jest obiekt UI (zawierający obrazek, tytuł i opis), a także przypisywane są odpowiednie dane oraz kolory w zależności od stanu odznaki.
    /// </summary>
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
