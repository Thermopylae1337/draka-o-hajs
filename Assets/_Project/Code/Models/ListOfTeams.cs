using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// Klasa do przechowywania listy użytkowników w danej drużynie.
/// Dziedziczy po TeamManager i implementuje interfejsy <see cref="INetworkSerializable"/> oraz <see cref="IEquatable{TeamListModel}"/>.
/// </summary>
public class TeamListModel : List<TeamManager>, INetworkSerializable, IEquatable<TeamListModel>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Konstruktor inicjalizujący listę użytkowników drużyny
    /// </summary>
    /// <param name="lot">Lista użytkowników drużyny typu <see cref="TeamManager"/>.</param>
    public TeamListModel(List<TeamManager> lot) : base(lot)
    {
    }

    public TeamListModel()
    {
    }

    /// <summary>
    /// Serializuje dane obiektu do sieciowego formatu przy użyciu dostarczonego serializera
    /// </summary>
    /// <typeparam name="T">Typ, który implementuje interfejs <see cref="IReaderWriter"/>. Określa sposób serializacji i deserializacji danych.</typeparam>
    /// <param name="serializer">Obiekt odpowiedzialny za serializację danych w formacie sieciowym.</param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        Utils.NetworkSerializeList(serializer, this);
    }

    /// <summary>
    ///  Porównuje bieżący obiekt z innym obiektem na podstawie zawartości
    /// </summary>
    /// <param name="other">Obiekt typu <see cref="TeamListModel"/>, z którym ma zostać porównany bieżący obiekt.</param>
    /// <returns>True, jeśli bieżący obiekt ma taką sama zawartość jak parametr (other); w przeciwnym razie false.</returns>
    public bool Equals(TeamListModel other) => this.SequenceEqual(other);
}
