using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Klasa reprezentująca listę kategorii pytań.
/// </summary>
public class CategoryList
{
    /// <summary>
    /// Lista przechowująca kategorie pytań.
    /// </summary>
    [JsonProperty("listaKategorii")]
    private readonly List<Category> categoryList = new();

    /// <summary>
    /// Konstruktor kopiujący inicjalizujący podaną listą kategorii.
    /// </summary>
    /// <param name="list">Zmienna reprezentująca listę kategorii.</param>
    public CategoryList(List<Category> list) => categoryList = list;

    //public List<Question> FindCategory(string name)
    /// <summary>
    /// Metoda umożliwiająca znalezienie kategorii na podstawie nazwy.
    /// </summary>
    /// <param name="name">Zmienna reprezentująca nazwę kategorii, którą chcemy wyszukać.</param>
    /// <returns>Zwraca wyszukaną nazwę kategorii.</returns>
    public Category FindCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException("Nazwa kategorii nie może być pusta.");
        }

        foreach (Category item in categoryList)
        {
            if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                //return item.questionList;
                return item;
            }
        }

        throw new Exception("Nie znaleziono kategorii");
    }

    /// <summary>
    /// Metoda umożliwiająca dodanie do listy konkretnej kategorii.
    /// </summary>
    /// <param name="k">Zmienna reprezentująca kategorie. </param>
    public void AddCategory(Category k)
    {
        if (k == null)
        {
            throw new ArgumentNullException(nameof(k), "Kategoria nie może być null.");
        }

        categoryList.Add(k);
    }

    /// <summary>
    /// Metoda wykonująca serializacje bieżącego obiektu CategoryList do formatu JSON, która zapisuję go do wskazanego pliku.
    /// </summary>
    /// <param name="path">Ścieżka do pliku, w którym dane JSON mają zostać zapisane.</param>
    public void Serialize(string path)
    {
        JsonSerializerSettings settings = new()
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };
        string json = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Deserializuje obiekt typu CategoryList z pliku JSON znajdującego się pod podaną ścieżką.
    /// </summary>
    /// <param name="path">Ścieżka do pliku JSON, który ma zostać zdeserializowany</param>
    /// <returns>Obiekt typu CategoryList odtworzony z danych JSON.</returns>
    public static CategoryList Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Nie znaleziono pliku.", path);
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<CategoryList>(json);
    }
}
