using System;
using Unity.Netcode;

namespace Network.Administration.Selection
{
    public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
    {
        public ulong ClientId;
        public int CharacterId;

        public CharacterSelectState(ulong clientId, int characterId = -1)
        {
            ClientId = clientId;
            CharacterId = characterId;
        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref CharacterId);
        }

        public bool Equals(CharacterSelectState other)
        {
            return ClientId == other.ClientId && CharacterId == other.CharacterId;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterSelectState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, CharacterId);
        }
    }
}