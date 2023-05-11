using System.Collections.Generic;
using _00___Game_Data.Scripts.Core.Utils.Singleton;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Networking
{
    public class ServerManager: Singleton<ServerManager>
    {
        [SerializeField] private string characterSelectSceneName = "CharacterSelectScene";
        [SerializeField] private string gameplaySceneName = "Gameplay";
        
        private bool _gameHasStarted;
        
        public Dictionary<ulong, ClientData> ClientData { get; private set; }


        public void StartServer()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;
            
            ClientData = new Dictionary<ulong, ClientData>();
            
            NetworkManager.Singleton.StartServer();
        }
        
        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

            ClientData = new Dictionary<ulong, ClientData>();
            
            NetworkManager.Singleton.StartHost();
        }

        
        
        private void OnNetworkReady()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientDisconnect;
            
            NetworkManager.Singleton.SceneManager.LoadScene(characterSelectSceneName, LoadSceneMode.Single);
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (ClientData.ContainsKey(clientId))
            {
                if (ClientData.Remove(clientId))
                {
                    Debug.Log($"Removed client  {clientId}");
                }
            }
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (ClientData.Count >= 4 || _gameHasStarted)
            {
                response.Approved = false;
                return;
            }

            response.Approved = true;
            response.CreatePlayerObject = false;
            response.Pending = false;

            ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
            
            Debug.Log($"Added client  {request.ClientNetworkId}");

        }

        public void SetCharacter(ulong clientId, int characterId)
        {
            if (ClientData.TryGetValue(clientId, out ClientData data))
            {
                data.characterId = characterId;
            }
        }

        public void StartGame()
        {
            _gameHasStarted = true;

            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);

        }
    }
}