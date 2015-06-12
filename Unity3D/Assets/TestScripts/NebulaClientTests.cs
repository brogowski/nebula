using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.EditorSettings;
using Nebula;
using Nebula.Connectivity;
using Nebula.Input;
using Nebula.Packets;
using Nebula.Serialization;
using Nebula.Transmission;
using UnityEngine;

namespace Assets.TestScripts
{
    class NebulaClientTests : MonoBehaviour
    {
        public string Ip = "127.0.0.1";
        public int Port = 9999;
        public GameObject Prefab;


        private TcpClientTransmissionProtocol _transmissionProtocol;
        private GameObjectDirector _gameObjectDirector;
        private NebulaClient _nebula;

        void OnEnable()
        {
            _gameObjectDirector = new GameObjectDirector(new Dictionary<string, GameObject>{{Prefab.name, Prefab}});

            _transmissionProtocol = new TcpClientTransmissionProtocol(Ip, Port);
            _transmissionProtocol.Start();

            _nebula = new NebulaClient(new RecorderInputSender(_transmissionProtocol, new RecordedInputSerializer()),
                new PacketReciver(_transmissionProtocol, SetupPacketSerializer()), new InputRecorder());
        }

        private PacketSerializer SetupPacketSerializer()
        {
            var packetSerializer = new PacketSerializer();
            var movePacketSerializer = new MovePacketSerializer(new GuidSerializer(), new Vector3Serializer(),
                new QuaternionSerializer());
            var spawnPacketSerializer = new SpawnPacketSerializer(new GuidSerializer(), new Vector3Serializer(),
                new QuaternionSerializer());
            var destroyPacketSerializer = new DestroyPacketSerializer();
            packetSerializer.Deserializers.Add(typeof(MovePacket), packet => movePacketSerializer.Deserialize(packet));
            packetSerializer.Deserializers.Add(typeof(SpawnPacket), packet => spawnPacketSerializer.Deserialize(packet));
            packetSerializer.Deserializers.Add(typeof(DestroyPacket), packet => new DestroyPacket(destroyPacketSerializer.Deserialize(packet)));
            return packetSerializer;
        }

        void Update()
        {
            UpdateRemoteInput(Time.deltaTime);
            UpdateLocalPhysics(Time.deltaTime);
        }

        void OnDestroy()
        {            
            _transmissionProtocol.Stop();
        }


        private void UpdateRemoteInput(float deltaTime)
        {
            var generateValue = Input.GetButtonUp("Generate");
            var pauseValue = Input.GetButtonUp("Pause");


            var inputData = new[]
            {
                new RecordedInput.InputData("Generate", generateValue ? 1.00f : 0f),
                new RecordedInput.InputData("Pause", pauseValue ? 1.00f : 0f)
            };

            _nebula.RecordInput(new RecordedInput(inputData, deltaTime));
            _nebula.UpdateRemoteInputs(deltaTime);
            
        }

        private void UpdateLocalPhysics(float deltaTime)
        {
            var packets = _nebula.GetPhysicsPackets().OrderBy(q => q.GetType(), new PacketComparer());
            foreach (var packet in packets)
            {
                if (packet is SpawnPacket)
                {
                    _gameObjectDirector.CreateNewGameObject((SpawnPacket)packet);
                }
                if (packet is MovePacket)
                {
                    _gameObjectDirector.AddNewMovement((MovePacket)packet);
                }
                if (packet is DestroyPacket)
                {
                    _gameObjectDirector.DestroyObject((DestroyPacket)packet);
                }
            }
            _gameObjectDirector.MoveObjects(deltaTime);
        }
    }

    internal class PacketComparer : IComparer<Type>
    {
        private readonly Dictionary<Type, int> _order = new Dictionary<Type, int>
        {
            {typeof(SpawnPacket), 1},
            {typeof(MovePacket), 2},
            {typeof(DestroyPacket), 3},
        }; 
        public int Compare(Type x, Type y)
        {
            if(!_order.ContainsKey(x) || !_order.ContainsKey(y))
                throw new NotSupportedException();

            return _order[x].CompareTo(_order[y]);
        }
    }

    class RecorderInputSender : AbstractSender<RecordedInput>
    {
        public RecorderInputSender(ISendTransmissionProtocol protocol, IPacketSerializer<RecordedInput> serializer) : base(protocol, serializer)
        {
        }
    }

    class PacketReciver : AbstractReciver<IPacket>
    {
        public PacketReciver(IReciveTransmissionProtocol transmissionProtocol, IPacketDeserializer<IPacket> deserializer) : base(transmissionProtocol, deserializer)
        {
        }
    }
}
