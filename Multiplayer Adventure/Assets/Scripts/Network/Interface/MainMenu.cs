using System;
using System.Threading.Tasks;
using _00___Game_Data.Scripts.Core.Utils.Singleton;
using Network.Networking;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Network.Interface
{
    public class MainMenu: Singleton<MainMenu>
    {
        [SerializeField] private GameObject connectingPanel;
        
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private TMP_InputField joinCodeInputField;
        
        
        private async void Start()
        {
            connectingPanel.SetActive(true);
            
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
            }
            catch (Exception e)
            {
                Debug.Log($"Error {e.Message}");
                throw;
            }
            
            
            connectingPanel.SetActive(false);
            menuPanel.SetActive(true);
        }


        public void StartHost()
        {
             HostManager.Instance.StartHost();
        }

        public void StartClient()
        {
            ClientManager.Instance.StartClient(joinCodeInputField.text);
        }

    }
}