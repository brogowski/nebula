using System;
using System.Collections.Generic;
using Assets.Scripts.TextControl;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    class Network : MonoBehaviour
    {
        public TextController TextController;
        public Button SendButton;
        public Button ClientButton;
        public Button HostButton;
        public InputField ConnectInputField;
        public InputField MessageInputField;

        private NetworkController _networking;

        void Start()
        {
            _networking = new NetworkController();

            SendButton.onClick.AddListener(() =>
            {
                var message = MessageInputField.text;
                MessageInputField.text = string.Empty;
                if (!string.IsNullOrEmpty(message))
                {
                    _networking.SendOnlineMessage(message);
                }
            });

            ClientButton.onClick.AddListener(() =>
            {
                _networking.ConnectAsClient(ConnectInputField.text);
            });

            HostButton.onClick.AddListener(() =>
            {
                _networking.ConnectAsServer(ConnectInputField.text);
            });
        }

        public void Update()
        {
            if (_networking != null)
            {
                foreach (var message in _networking.GetOnlineMessages())
                {
                    TextController.AddMessage(message);
                }
            }
        }

        void OnDestroy()
        {
            if(_networking != null)
                _networking.Dispose();
        }
    }
}
