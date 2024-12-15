using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class CategoryList
{
    [JsonProperty("listaKategorii")]
    private readonly List<Category> categoryList = new();

    public CategoryList(List<Category> list) => categoryList = list;

    //public List<Question> FindCategory(string name)
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

    public void AddCategory(Category k)
    {
        if (k == null)
        {
            throw new ArgumentNullException(nameof(k), "Kategoria nie może być null.");
        }

        categoryList.Add(k);
    }

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
