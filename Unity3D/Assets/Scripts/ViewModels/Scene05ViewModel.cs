using System;
using System.Collections.Generic;
using Assets.Scripts.Networking;
using Nebula.Connectivity;
using Nebula.Packets;
using Nebula.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ViewModels
{
    class Scene05ViewModel : MonoBehaviour
    {
        public Button ClientButton;
        public Button HostButton;
        public GameObject SpawnPrefab;
        public string AddressPort = "127.0.0.1:9000";

        private NetworkController _networking;
        private IPacketConverter<IPacket> _serializer;
        private Action _updateAction = null;
        private Action _spawnClickAction = null;
        private readonly Dictionary<Guid, GameObject> _gameObjects
            = new Dictionary<Guid, GameObject>();

        void Start()
        {
            _networking = new NetworkController();
            _serializer = GetPacketSerializer();
        }

        private PacketSerializer GetPacketSerializer()
        {
            var serializer = new PacketSerializer();
            var spawnConverter = new SpawnPacketSerializer(
                new GuidSerializer(),
                new Vector3Serializer(),
                new QuaternionSerializer());
            var moveConverter = new MovePacketSerializer(
                new GuidSerializer(),
                new Vector3Serializer(),
                new QuaternionSerializer());
            IPacketConverter<DestroyPacket> destroyConverter = new DestroyPacketSerializer();

            serializer.Deserializers.Add(typeof(SpawnPacket), s => spawnConverter.Deserialize(s));
            serializer.Deserializers.Add(typeof(MovePacket), s => moveConverter.Deserialize(s));
            serializer.Deserializers.Add(typeof(DestroyPacket), s => destroyConverter.Deserialize(s));

            serializer.Serializers.Add(typeof(SpawnPacket), packet => spawnConverter.Serialize((SpawnPacket)packet));
            serializer.Serializers.Add(typeof(MovePacket), packet => moveConverter.Serialize((MovePacket)packet));
            serializer.Serializers.Add(typeof(DestroyPacket), packet => destroyConverter.Serialize((DestroyPacket)packet));

            return serializer;
        }

        public void ConnectAsClient()
        {
            _networking.ConnectAsClient(AddressPort);
            _updateAction = UpdateClient;
            _spawnClickAction = () =>
            {
                var packet = new SpawnPacket(Guid.NewGuid(), new Vector3(0, 5), new Quaternion(), "X");
                var serializedPacket = _serializer.Serialize(packet);
                _networking.SendOnlineMessage(serializedPacket);
            };
        }

        private void UpdateClient()
        {
            var messages = _networking.GetOnlineMessages();
            foreach (var message in messages)
            {
                var packet = _serializer.Deserialize(message);
                if (packet is SpawnPacket)
                {
                    var spawnPacket = (SpawnPacket)packet;
                    CreatePrefab(spawnPacket.Id, spawnPacket.Position, spawnPacket.Rotation);
                }
            }
        }

        public void ConnectAsServer()
        {
            _networking.ConnectAsServer(AddressPort);
            _updateAction = UpdateServer;
        }

        private void UpdateServer()
        {
            var messages = _networking.GetOnlineMessages();
            foreach (var message in messages)
            {
                var packet = _serializer.Deserialize(message);
                if (packet is SpawnPacket)
                {
                    var spawnPacket = (SpawnPacket)packet;
                    CreatePrefab(spawnPacket.Id, spawnPacket.Position, spawnPacket.Rotation);
                    _networking.SendOnlineMessage(message);
                }
            }
        }

        public void SpawnClick()
        {
            if(_spawnClickAction != null)
                _spawnClickAction.Invoke();
        }

        void CreatePrefab(Guid id, Vector3 position, Quaternion rotation)
        {
            var obj = Instantiate(SpawnPrefab, position, rotation) as GameObject;
            _gameObjects.Add(id, obj);
        }

        void Update()
        {
            if(_updateAction != null)
                _updateAction.Invoke();
        }

        void OnDestroy()
        {
            _networking.Dispose();
        }        
    }
}
