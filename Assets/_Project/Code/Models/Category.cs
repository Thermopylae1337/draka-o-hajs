using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;

/// <summary>
/// Klasa reprezentująca informacje odnośnie kategori wylosowanego pytania. 
/// </summary>
public class Category : INetworkSerializable, IEquatable<Category>
{
    /// <summary>
    /// Zmienna przechowująca informacje nazwy kategorii.
    /// </summary>
    [JsonProperty("nazwa")]
    public string Name => name;

    /// <summary>
    /// Lista przychowująca pytania.
    /// </summary>
    [JsonProperty("pytania", Order = 2)]
    public List<Question> questionList = new();

    /// <summary>
    /// Statyczna instancja klasy używana do generowania losowych wartości.
    /// </summary>
    private static readonly System.Random _random = new();
    private string name = string.Empty;

    /// <summary>
    /// Konstruktor inicjalizujący klasę z podaną nazwą kategorii.
    /// </summary>
    /// <param name="name">Nazwa kategorii.</param>
    public Category(string name) => this.name = name;

    /// <summary>
    /// Konstruktor kopiujący inicjalizujący klasę z podaną nazwą kategorii i listą pytań.
    /// </summary>
    /// <param name="nazwa">Nazwa kategorii.</param>
    /// <param name="list">Lista pytań</param>
    [JsonConstructor]
    public Category(string nazwa, List<Question> list)
    {
        name = nazwa;
        questionList = list;
    }

    /// <summary>
    /// Konstruktor kopiujący.
    /// </summary>
    public Category()
    {
    }

    /// <summary>
    /// Metoda dodająca pytanie do listy.
    /// </summary>
    /// <param name="question">Pytanie.</param>
    public void AddQuestionToList(Question question)
    {
        if (question == null)
        {
            return;
        }

        questionList.Add(question);
    }

    /// <summary>
    /// Losuje pytanie z dostępnej listy pytań i usuwa je z listy.
    /// </summary>
    /// <returns>Wylosowane pytanie typu <see cref="Question"/> lub null, jeśli lista pytań jest pusta.</returns>
    public Question DrawQuestion()
    {
        Question question = questionList.Count == 0 ? null : questionList[_random.Next(questionList.Count)];

        _ = questionList.Remove(question);
        return question;
    }

    /// <summary>
    /// Metoda wykonująca serializacje bieżącego obiektu TeamManager do formatu JSON, która zapisuję go do wskazanego pliku.
    /// </summary>
    /// <param name="path">Ścieżka do pliku, w którym dane JSON mają zostać zapisane.</param>
    public void Serialize(string path)
    {
        JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Deserializuje obiekt typu Category z pliku JSON znajdującego się pod podaną ścieżką.
    /// </summary>
    /// <param name="path">Ścieżka do pliku JSON, który ma zostać zdeserializowany</param>
    /// <returns>>Obiekt typu Category odtworzony z danych JSON.</returns>
    public static Category Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", path);
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Category>(json);
    }

    /// <summary>
    /// Serializuje dane obiektu przez sieć przy użyciu dostarczonego serializatora.
    /// </summary>
    /// <typeparam name="T">Typ serializatora implementującego interfejs <see cref="IReaderWriter"/>.</typeparam>
    /// <param name="serializer">Zmienna przechowująca serializator używana do serializacji danych.</param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);

        _ = Utils.NetworkSerializeList(serializer, questionList);
    }
    /// <summary>
    /// Porównuje bieżący obiekt z innym obiektem na podstawie nazwy.
    /// </summary>
    /// <param name="category">Nazwa kategorii.</param>
    /// <returns>True, jeśli nazwy kategorii są identyczne; w przeciwnym razie false.</returns>
    public bool Equals(Category category) => name == category.name;
}
