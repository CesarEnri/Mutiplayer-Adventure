using Network.Administration.Selection;
using Unity.Netcode;
using UnityEngine;

namespace Network.Networking
{
    public class CharacterSpawner: NetworkBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;
        
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            foreach (var client in ServerManager.Instance.ClientData)
            {
                var character = characterDatabase.GetCharacterById(client.Value.characterId);

                if (character != null)
                {
                    var spawnPos = new Vector3(Random.Range(-3, 3), 0f, Random.Range(-3f, 3));
                    var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                    characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                }
            }
            
        }
    }
}