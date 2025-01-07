using System;
using Unity.Netcode;

/// <summary>
/// Klasa reprezentująca nagrodę uzyskaną przez drużynę.
/// </summary>
[Serializable]
public struct PrizeData : INetworkSerializable
{
    /// <summary>
    /// Zmienna przechowująca nazwę drużyny.
    /// </summary>
    public string teamName;
    /// <summary>
    /// Zmienna przechowująca pieniądze wylosowane przez drużynę.
    /// </summary>
    public int money;
    /// <summary>
    /// Zmienna przechowująca odznakę uzyskaną przez drużynę
    /// </summary>
    public string badge;

    /// <summary>
    /// Metoda służąca do serializacji obiektu.
    /// </summary>
    /// <typeparam name="T">Parametr będący typem klasy realizującej interfejs IReaderWriter.</typeparam>
    /// <param name="serializer">Zmienna reprezentująca obiekt klasy BufferSerializer<T>.</param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        teamName ??= string.Empty;
        badge ??= string.Empty;

        serializer.SerializeValue(ref teamName);
        serializer.SerializeValue(ref money);
        serializer.SerializeValue(ref badge);
    }
}

