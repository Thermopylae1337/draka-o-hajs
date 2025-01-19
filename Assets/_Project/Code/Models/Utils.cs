using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

/// <summary>
/// Klasa zawierająca pomocnicze metody oraz stałe używane w grze, takie jak ustawienia początkowe, limity rund, oraz metody do serializacji i deserializacji danych.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Stała przechowująca ilość możliwych do wystąpienia pytań w grze.
    /// </summary>
    public const int QUESTIONS_AMOUNT = 8;
    /// <summary>
    /// Stała przechowująca ilość początkowej kasy podczas rozpoczęcia gry.
    /// Reprezentuje domyślną kwotę, z jaką drużyna zaczyna grę
    /// </summary>
    public const int START_MONEY = 10000;
    /// <summary>
    /// Stała przechowująca domyślną nazwę drużyny.
    /// </summary>
    public const string TEAM_DEFAULT_NAME = "New Team";
    /// <summary>
    /// Stała przechowująca ilość możliwych rund, które mogą wystąpić w grze.
    /// Określa maksymalną liczbę rund, które mogą zostać rozegrane w danej grze.
    /// </summary>
    public const int ROUNDS_LIMIT = 7;

    // Serialization helpers
    /// <summary>
    /// Serializuje i deserializuje listę obiektów do/z formatu JSON w zależności od trybu serializacji (czytania/zapisu).
    /// </summary>
    /// <typeparam name="T">Typ, który implementuje interjes <see cref="IReaderWriter">.</typeparam>
    /// <typeparam name="Y">Typ obiektów w liście, które mają zostać zserializowane.</typeparam>
    /// <param name="serializer">Obiekt odpowiedzialny za serializację danych.</param>
    /// <param name="list">Lista obiektów typu <typeparamref name="Y"/>, którą chcemy zserializować.</param>
    /// <returns>W przypadku trybu zapisu, deserializuje listę i zwraca ją po zakończeniu serializacji. W przypadku trybu odczytu zwraca null.</returns>
    public static List<Y> NetworkSerializeList<T, Y>(BufferSerializer<T> serializer, List<Y> list) where T : IReaderWriter
    {
        string listSerialized = serializer.IsReader ? "" : JsonConvert.SerializeObject(list);

        serializer.SerializeValue(ref listSerialized);

        return serializer.IsWriter ? JsonConvert.DeserializeObject<List<Y>>(listSerialized) : null;
    }

    /// <summary>
    /// Serializuje i deserializuje słownik obiektów do/z formatu JSON w zależności od trybu serializacji (czytania/zapisu).
    /// </summary>
    /// <typeparam name="T">Typ, który implementuje interfejs <see cref="IReaderWriter"/>.</typeparam>
    /// <typeparam name="Y">Typ kluczy w słowniku.</typeparam>
    /// <typeparam name="Z">Typ wartości w słowniku.</typeparam>
    /// <param name="serializer">Obiekt odpowiedzialny za serializację danych.</param>
    /// <param name="dict">Słownik obiektów typu <typeparamref name="Y"/> (klucze) i <typeparamref name="Z"/> (wartości), który ma zostać zserializowany.</param>
    /// <returns>W przypadku trybu zapisu, deserializuje słownik i zwraca go po zakończeniu serializacji. W przypadku trybu odczytu zwraca null.</returns>
    public static Dictionary<Y, Z> NetworkSerializeDictionary<T, Y, Z>(BufferSerializer<T> serializer, Dictionary<Y, Z> dict) where T : IReaderWriter
    {
        string dictSerialized = serializer.IsReader ? "" : JsonConvert.SerializeObject(dict);

        serializer.SerializeValue(ref dictSerialized);

        return serializer.IsWriter ? JsonConvert.DeserializeObject<Dictionary<Y, Z>>(dictSerialized) : null;
    }

    /// <summary>
    /// Tablica przechowująca typy każdej z kategorii występującej w grze. 
    /// </summary>
    public static readonly string[] categoryNames = new string[]
    {
        "Czarna skrzynka",
        "Anatomia i medycyna",
        "Astronomia",
        "Biologia",
        "Chemia",
        "Czasy współczesne",
        "Film",
        "Filozofia i religie",
        "Fizyka",
        "Geografia",
        "Historia",
        "Język polski",
        "Kulinaria",
        "Literatura",
        "Matematyka",
        "Podpowiedź",
        "Motoryzacja",
        "Muzyka klasyczna",
        "Muzyka rozrywkowa",
        "Piłka nożna",
        "Polityka i gospodarka",
        "Popkultura",
        "Przysłowia i cytaty",
        "Rozmaitości",
        "Seriale",
        "Sport",
        "Sztuka",
        "Technologie",
        "Wędkarstwo",
    };
}