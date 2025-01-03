using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Assets._Project.Code.Models
{
    [Serializable]
    public struct PrizeData : INetworkSerializable
    {
        public string teamName;
        public int money;
        public string badge;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            teamName ??= string.Empty;
            badge ??= string.Empty;

            serializer.SerializeValue(ref teamName);
            serializer.SerializeValue(ref money);
            serializer.SerializeValue(ref badge);
        }
    }
}
