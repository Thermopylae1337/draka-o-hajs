using System;
using Unity.Netcode;

namespace Assets._Project.Code.Models
{
    [Serializable]
    public struct PrizeDataList : INetworkSerializable
    {
        public PrizeData[] prizes;

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
}
