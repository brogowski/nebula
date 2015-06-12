using System;
using System.Linq;
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
        public GameObject StatisticsPanel;
        public Text StatisticsText;

        public void OnEnable()
        {
            ClientButton.onClick.AddListener(ClientClick);
            HostButton.onClick.AddListener(HostClick);
            CheckIfCommandLine();
        }

        private void CheckIfCommandLine()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length <= 1)
                return;

            for (int index = 0; index < commandLineArgs.Length; index++)
            {
                if (commandLineArgs[index] == "-framerate")
                {
                    Application.targetFrameRate = int.Parse(commandLineArgs[index + 1]);
                }

                if (commandLineArgs[index] == "-ip")
                {
                    IpInputField.text = commandLineArgs[index + 1];                    
                }
            }
            HostClick();
        }

        public void ClientClick()
        {
            NetworkPanel.SetActive(false);
            CameraMover.enabled = true;
            NebulaClient.Ip = IpInputField.text;
            NebulaClient.gameObject.SetActive(true);
            StatisticsPanel.SetActive(true);
            WindowTitleChanger.ChangeTitleName("Nebula", "Nebula - Client");
        }

        public void HostClick()
        {
            NetworkPanel.SetActive(false);
            CameraMover.enabled = true;
            NebulaServer.Ip = IpInputField.text;
            NebulaServer.gameObject.SetActive(true);
            WindowTitleChanger.ChangeTitleName("Nebula", "Nebula - Server");
        }

        public void UpdateStatisticsPanel(string text)
        {
            StatisticsText.text = text;
        }

        public static void RunHeadLessServer()
        {
        
        }
    }
}
