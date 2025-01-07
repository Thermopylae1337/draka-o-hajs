using System;
using Unity.Netcode;

/// <summary>
/// Klasa służąca do przechowywania nagród uzyskanych przez drużynę.
/// </summary>
[Serializable]
public struct PrizeDataList : INetworkSerializable
{
    /// <summary>
    /// Zmienna przechowywująca listę nagród uzyskanych przez drużynę.
    /// </summary>
    public PrizeData[] prizes;

    /// <summary>
    /// Metoda służąca do serializacji klasy.
    /// </summary>
    /// <typeparam name="T">Parametr będący typem klasy realizującej interfejs IReaderWriter.</typeparam>
    /// <param name="serializer">Zmienna reprezentująca obiekt klasy BufferSerializer<T>.</param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = prizes?.Length ?? 0;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            prizes = new PrizeData[count];
        }

        for (int i = 0; i < count; i++)
        {
            prizes[i].NetworkSerialize(serializer);
        }
    }
}

