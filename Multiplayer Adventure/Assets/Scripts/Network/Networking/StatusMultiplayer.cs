using _00___Game_Data.Scripts.Core.Utils.Singleton;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Networking
{
    public class StatusMultiplayer : Singleton<StatusMultiplayer>
    {

        public void StartHost()
        {
            ServerManager.Instance.StartHost();
            
        }

        public void StartServer()
        {
            ServerManager.Instance.StartServer();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        public void StopAll()
        {
            
        }
    }
}
