using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
public class TeamListModel : List<Team>, INetworkSerializable, IEquatable<TeamListModel>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TeamListModel(List<Team> lot) : base(lot)
    {
    }

    public TeamListModel()
    {
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        Utils.NetworkSerializeList(serializer, this);
    }

    public bool Equals(TeamListModel other) => this.SequenceEqual(other);
}
