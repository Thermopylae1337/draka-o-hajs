using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
//klasa stworzona �eby m�c przetestowa� licytacj� ale powinna by� u�yteczna po prostu do serializacji list dru�yn
public class ListOfTeams : INetworkSerializable
{
    public List<Team> list;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ListOfTeams(List<Team> lot)
    {
        this.list = lot;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        Utils.NetworkSerializeList(serializer, list);
    }
    // Update is called once per frame

}
