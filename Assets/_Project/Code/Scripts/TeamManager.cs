using System;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Główna klasa zarządzająca drużyną, umożliwiająca udzielanie odpowiedzi, podbijanie stawek w licytacjach oraz korzystanie ze wskazówek.
/// </summary>
public class TeamManager : NetworkBehaviour
{
    /// <summary>
    /// Prywatna zmienna reprezentująca ilość pieniędzy drużyny, synchronizowana przez sieć.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> money = new(Utils.START_MONEY, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna reprezentująca liczbę dostępnych podpowiedzi, synchronizowana przez sieć.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> clues = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna reprezentująca ilość użytych podpowiedzi przez drużynę.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> cluesUsed = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna reprezentująca ilość zdobytych czarnych skrzynek.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> blackBoxes = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna reprezentująca ilość wygranych licytacji.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> wonBid = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna przechowująca liczbę odpowiedzi udzielonych przez drużynę.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> questionsAnswered = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna przechowująca liczbę pytań zadanych drużynie.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> questionsAsked = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna przechowująca liczbę użytych opcji "VaBanque" przez drużynę.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> vaBanque = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna przechowująca liczbę rund, w których drużyna była nieaktywna w licytacji.
    /// </summary>
    [SerializeField]
    private NetworkVariable<int> inactiveRounds = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone); //licznik rund bierności w licytacji

    /// <summary>
    /// 
    /// </summary>
    private BadgeList badgeList = new();

    /// <summary>
    /// Prywatna zmienna przechowująca nazwę danej drużyny.
    /// </summary>
    [SerializeField]
    public NetworkVariable<FixedString64Bytes> teamName = new(Utils.TEAM_DEFAULT_NAME, writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField]
    public string TeamName => teamName.Value.ToString();

    /// <summary>
    /// Prywatna zmienna przechowująca aktualną stawkę licytacji.
    /// </summary>
    private NetworkVariable<int> bid = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    [SerializeField]
    private ColourEnum colour;

    [SerializeField]
    private NetworkVariable<bool> inGame = new(true, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna przechowująca numer przypisany do danej drużyny.
    /// </summary>
    [SerializeField]
    private NetworkVariable<uint> teamId = new(0, writePerm: NetworkVariableWritePermission.Owner, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Prywatna zmienna przechująca numer przypisany do sieci.
    /// </summary>
    [SerializeField]
    private NetworkVariable<uint> networkId = new(0, writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    /// <summary>
    /// Zmienna przechowująca numer sieci.
    /// </summary>
    public uint NetworkId
    {
        get => networkId.Value;
        set => networkId.Value = value;
    }

    /// <summary>
    /// Zmienna przechowująca numer drużyny
    /// </summary>
    public uint TeamId
    {
        get => teamId.Value;
        set => teamId.Value = value;
    }
    /// <summary>
    /// Zmienna przechowująca informacje czy dana drużyna uczestniczy w rozgrywce.
    /// </summary>
    public bool InGame
    {
        get => inGame.Value;
        set => inGame.Value = value;
    }

    //gettery i settery
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Zmienna przechowująca ilość pieniedzy drużyny.
    /// </summary>
    public int Money
    {
        get => money.Value;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("Pieniądze zeszły poniżej zera, zeruje.");
                money.Value = 0;
                return;
            }

            money.Value = value;
        }
    }

    public int Bid
    {
        get => bid.Value;
        set => bid.Value = value;
    }
    public ColourEnum Colour
    {
        get => colour;
        set => colour = value;
    }

    /// <summary>
    /// Zmienna przechowująca ilość dostepnych wskazówek dla kokretnej drużyny.
    /// </summary>
    public int Clues
    {
        get => clues.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Dostępnych wskazówek nie może być mniej niż 0.");
            }

            clues.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca ilośc użytych wskazówek przez drużynę.
    /// </summary>
    public int CluesUsed
    {
        get => cluesUsed.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Zużyte wskazówki nie mogą być na minusie.");
            }

            cluesUsed.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca informacje odnośnie czarnych skrzynek.
    /// </summary>
    public int BlackBoxes
    {
        get => blackBoxes.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Czarne Skrzynki nie mogą być na minusie.");
            }

            blackBoxes.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca informacje wygrania rundy.
    /// </summary>
    public int WonBid
    {
        get => wonBid.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("WonBid cannot be negative.");
            }

            wonBid.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca informacje na ile pytań odpowiedziano.
    /// </summary>
    public int QuestionsAnswered
    {
        get => questionsAnswered.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("QuestionsAnswered cannot be negative.");
            }

            questionsAnswered.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowujaca informacje ile było pytań.
    /// </summary>
    public int QuestionsAsked
    {
        get => questionsAsked.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("QuestionsAsked cannot be negative.");
            }

            questionsAsked.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca informacje odnośnie VaBanque.
    /// </summary>
    public int VaBanque
    {
        get => vaBanque.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("VaBanque cannot be negative.");
            }

            vaBanque.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca informacje nieaktywności w licytacji.
    /// </summary>
    public int InactiveRounds
    {
        get => inactiveRounds.Value;
        set
        {
            if (value < 0)
            {
                throw new Exception("Rundy bierności w licytacji nie mogą być na minusie.");
            }

            inactiveRounds.Value = value;
        }
    }

    /// <summary>
    /// Zmienna przechowująca zgromadzoną ilość pieniedzy przez drużynę.
    /// </summary>
    public int TotalMoney { get; set; }
    public BadgeList BadgeList
    {
        get => badgeList;
        set => badgeList = value;
    }

    /// <summary>
    /// Metoda wywoływana podczas licytacji umożliwiająca zwiekszenie stawki.
    /// </summary>
    /// <param name="amount">Zmienna reprezentująca kwotę.</param>
    public void RaiseBid(int amount)
    {
        if (money.Value >= amount)
        {
            money.Value -= amount;
            bid.Value += amount;
        }
    }
    public void ResetBid()
    {
        //dodałem funkcję reset bid żeby można było np zrobić odznakę "zakończ licytację z jakąśtam kwotą na końcu"
        bid.Value = 0;
    }

    public void Start()
    {
        teamName.OnValueChanged = delegate (FixedString64Bytes oldName, FixedString64Bytes newName)
        {
            gameObject.name = newName.ToString();
        };
        if (IsOwner)
        {
            teamName.Value = TeamCreatorController.chosenTeamName;
        }
    }


    /// <summary>
    /// Metoda wykonująca serializacje bieżącego obiektu TeamManager do formatu JSON, która zapisuję go do wskazanego pliku.
    /// </summary>
    /// <param name="path">Ścieżka do pliku, w którym dane JSON mają zostać zapisane.</param>
    public void Serialize(string path)
    {
        string jsonString = JsonUtility.ToJson(this);
        File.WriteAllText(path, jsonString);
    }

    /// <summary>
    /// Deserializuje obiekt typu TeamManager z pliku JSON znajdującego się pod podaną ścieżką.
    /// </summary>
    /// <param name="path">Ścieżka do pliku JSON, który ma zostać zdeserializowany</param>
    /// <returns>Obiekt typu TeamManager odtworzony z danych JSON.</returns>
    public static TeamManager Deserialize(string path)
    {
        string jsonFromFile = File.ReadAllText(path);
        return JsonUtility.FromJson<TeamManager>(jsonFromFile);
    }
}