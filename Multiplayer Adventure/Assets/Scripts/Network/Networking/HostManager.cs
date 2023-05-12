using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _00___Game_Data.Scripts.Core.Utils.Singleton;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Networking
{
    public class HostManager : Singleton<HostManager>
    {
        [SerializeField] private int maxConnections = 4;

        [SerializeField] private string characterSelectSceneName = "CharacterSelectScene";
        [SerializeField] private string gameplaySceneName = "Gameplay";

        private bool _gameHasStarted;

        public Dictionary<ulong, ClientData> ClientData { get; private set; }

        public string JoinCode { get; private set; }

        public void StartServer()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;
            
            ClientData = new Dictionary<ulong, ClientData>();
            
            NetworkManager.Singleton.StartServer();
        }
        
        public async Task StartHost()
        {
            Allocation allocation;

            try
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            }
            catch (Exception e)
            {
                Debug.Log($"Relay create allocation request failed {e.Message}");
                throw;
            }
            
            Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"server: {allocation.AllocationId}");

            try
            {
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            }
            catch (Exception e)
            {
                Debug.Log($"Relay get join code request failed {e.Message}");
                throw;
            }

            var relayServerData = new RelayServerData(allocation, "dtls");
            
           
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
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
            if (ClientData.Count >= maxConnections || _gameHasStarted)
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