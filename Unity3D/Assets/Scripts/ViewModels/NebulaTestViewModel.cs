using Assets.Scripts.Helpers;
using Assets.TestScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ViewModels
{
    class NebulaTestViewModel : MonoBehaviour
    {
        public InputField IpInputField;
        public GameObject NetworkPanel; 
        public Button ClientButton;
        public Button HostButton;
        public NebulaServerTests NebulaServer;
        public NebulaClientTests NebulaClient;
        public CameraMover CameraMover;

        public void OnEnable()
        {            
            ClientButton.onClick.AddListener(ClientClick);
            HostButton.onClick.AddListener(HostClick);
        }        

        public void ClientClick()
        {
            NetworkPanel.SetActive(false);
            CameraMover.enabled = true;
            NebulaClient.Ip = IpInputField.text;
            NebulaClient.gameObject.SetActive(true);
        }

        public void HostClick()
        {
            NetworkPanel.SetActive(false);
            CameraMover.enabled = true;
            NebulaServer.Ip = IpInputField.text;
            NebulaServer.gameObject.SetActive(true);
        }
    }
}
