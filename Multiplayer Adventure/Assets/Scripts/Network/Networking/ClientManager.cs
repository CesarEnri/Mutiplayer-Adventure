using System;
using System.Threading.Tasks;
using _00___Game_Data.Scripts.Core.Utils.Singleton;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Network.Networking
{
    public class ClientManager: Singleton<ClientManager>
    {
        
        public async Task StartClient(string joinCode)
        {
            JoinAllocation allocation;

            try
            {
                allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                
            }
            catch (Exception e)
            {
                Debug.Log($"Relay Get Join Code Request Failed");
                throw;
            }

            Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"host: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"client: {allocation.AllocationId}");
            
            var relayServerData = new RelayServerData(allocation, "dtls");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();
        }

    }
}