using System;
using UnityEngine.Serialization;

namespace Network.Networking
{
    [Serializable]
    public class ClientData
    {
        public ulong clientId;

        public int characterId = -1;

        public ClientData(ulong clientId)
        {
            this.clientId = clientId;
        }
    }
}