using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Network.Administration.Selection
{
    public class CharacterSelectDisplay: NetworkBehaviour
    {
        [SerializeField] private GameObject characterInfoPanel;
        [SerializeField] private TMP_Text characterNameText;

        private NetworkList<CharacterSelectState> _players;


        private void Awake()
        {
            _players = new NetworkList<CharacterSelectState>();
        }


        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;


                foreach (var clients in NetworkManager.Singleton.ConnectedClientsList)
                {
                    HandleClientConnected(clients.ClientId);
                }
            }

            base.OnNetworkSpawn();
        }
        

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
            }
            
            base.OnNetworkDespawn();
        }
        
        private void HandleClientConnected(ulong clientId)
        {
            _players.Add(new CharacterSelectState(clientId));
        }
        
        private void HandleClientDisconnected(ulong clientId)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    _players.RemoveAt(i);
                    break;
                }
            }
        }


        public void Select(Character character)
        {
            characterNameText.text = character.DisplayName;

            characterInfoPanel.SetActive(true);
            
            SelectServerRpc(character.Id);
        }

        
        [ServerRpc(RequireOwnership = false)]
        private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams=default)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    _players[i] = new CharacterSelectState(_players[i].ClientId, characterId);
                }
            }
        }
    }
}