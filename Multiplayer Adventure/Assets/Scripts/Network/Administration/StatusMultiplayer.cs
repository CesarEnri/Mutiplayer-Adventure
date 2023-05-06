using _00___Game_Data.Scripts.Core.Utils.Singleton;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Administration
{
    public class StatusMultiplayer : Singleton<StatusMultiplayer>
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }

        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
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
