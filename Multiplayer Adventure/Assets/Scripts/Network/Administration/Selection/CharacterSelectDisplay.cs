using System.Collections.Generic;
using Network.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Network.Administration.Selection
{
    public class CharacterSelectDisplay : NetworkBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;
        [SerializeField] private Transform charactersHolder;
        [SerializeField] private CharacterSelectButton selectButtonPrefab;
        [SerializeField] private PlayerCard[] playerCards;


        [SerializeField] private GameObject characterInfoPanel;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private Transform introSpawnPoint;
        [SerializeField] private Button lockInButton;

        private GameObject _introInstance;
        private List<CharacterSelectButton> _characterSelectButtons = new();


        private NetworkList<CharacterSelectState> _players;

        private void Awake()
        {
            _players = new NetworkList<CharacterSelectState>();
        }


        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                var allCharacters = characterDatabase.GetAllCharacters();

                foreach (var character in allCharacters)
                {
                    var selectButtonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                    selectButtonInstance.SetCharacter(this, character);
                    _characterSelectButtons.Add(selectButtonInstance);
                }

                _players.OnListChanged += HandlePlayersStateChanged;
            }

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
            if (IsClient)
            {
                _players.OnListChanged -= HandlePlayersStateChanged;
            }

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
            }

            base.OnNetworkDespawn();
        }

        private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
        {
            for (int i = 0; i < playerCards.Length; i++)
            {
                if (_players.Count > i)
                {
                    playerCards[i].UpdateDisplay(_players[i]);
                }
                else
                {
                    playerCards[i].DisableDisplay();
                }
            }

            foreach (var button in _characterSelectButtons)
            {
                if (button.IsDisabled) { continue; }

                if (IsCharacterTaken(button.Character.Id, false))
                {
                    button.SetDisabled();
                }
            }

            foreach (var player in _players)
            {
                if (player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

                if (player.IsLockedIn)
                {
                    lockInButton.interactable = false;
                    break;
                }

                if (IsCharacterTaken(player.CharacterId, false))
                {
                    lockInButton.interactable = false;
                    break;
                }

                lockInButton.interactable = true;
                
                
                
                break;
            }
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
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId)
                {
                    continue;
                }

                if (_players[i].IsLockedIn)
                {
                    return;
                }

                if (_players[i].CharacterId == character.Id)
                {
                    return;
                }

                if (IsCharacterTaken(character.Id, false))
                {
                    return;
                }
            }

            characterNameText.text = character.DisplayName;

            characterInfoPanel.SetActive(true);

            if (_introInstance != null)
            {
                Destroy(_introInstance);
            }

            _introInstance = Instantiate(character.IntroPrefab, introSpawnPoint);

            SelectServerRpc(character.Id);
        }


        [ServerRpc(RequireOwnership = false)]
        private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    continue;
                }

                if (!characterDatabase.IsValidCharacterId(_players[i].CharacterId))
                {
                    return;
                }

                if (IsCharacterTaken(characterId, true))
                {
                    return;
                }

                _players[i] = new CharacterSelectState(_players[i].ClientId, characterId, _players[i].IsLockedIn);

            }
        }

        public void LockIn()
        {
            LockInServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    continue;
                }

                if (!characterDatabase.IsValidCharacterId(_players[i].CharacterId))
                {
                    return;
                }

                if (IsCharacterTaken(_players[i].CharacterId, true))
                {
                    return;
                }

                _players[i] = new CharacterSelectState(_players[i].ClientId, _players[i].CharacterId, true);

            }

            foreach (var player in _players)
            {
                if (!player.IsLockedIn)
                {
                    return;
                }
            }

            foreach (var player in _players)
            {
                ServerManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
            }
            
            ServerManager.Instance.StartGame();
            
        }


        private bool IsCharacterTaken(int characterId, bool checkAll)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (!checkAll)
                {
                    if (_players[i].ClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        continue;
                    }

                }

                if (_players[i].IsLockedIn && _players[i].CharacterId == characterId)
                {
                    return true;
                }

            }

            return false;
        }
    }
}