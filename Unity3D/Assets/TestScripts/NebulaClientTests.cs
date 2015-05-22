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
        public GameObject[] Prefabs;

        private GameObjectDirector _gameObjectDirector;
        private NebulaClient _nebula;

        void OnEnable()
        {
            _gameObjectDirector = new GameObjectDirector(Prefabs.ToDictionary(q => q.name, q => q));

            var tcpClient = new TcpClientTransmissionProtocol(Ip, Port);
            tcpClient.Start();

            _nebula = new NebulaClient(new RecorderInputSender(tcpClient, new RecordedInputSerializer()),
                new PacketReciver(tcpClient, new PacketSerializer()), new InputRecorder());
        }

        void Update()
        {
            UpdateRemoteInput(Time.deltaTime);
            UpdateLocalPhysics(Time.deltaTime);
        }

        private void UpdateRemoteInput(float deltaTime)
        {
            var horizontalValue = Input.GetAxis(InputManager.HorizontalAxis);
            var verticalValue = Input.GetAxis(InputManager.VerticalAxis);

            var inputData = new[]
            {
                new RecordedInput.InputData(InputManager.HorizontalAxis, horizontalValue),
                new RecordedInput.InputData(InputManager.VerticalAxis, verticalValue),
            };

            _nebula.RecordInput(new RecordedInput(inputData, deltaTime));
            _nebula.UpdateRemoteInputs(deltaTime);
        }

        private void UpdateLocalPhysics(float deltaTime)
        {
            foreach (var packet in _nebula.GetPhysicsPackets())
            {
                if (packet is MovePacket)
                {
                    _gameObjectDirector.AddNewMovement((MovePacket)packet);
                }
                if (packet is SpawnPacket)
                {
                    _gameObjectDirector.CreateNewGameObject((SpawnPacket)packet);
                }
                if (packet is DestroyPacket)
                {
                    _gameObjectDirector.DestroyObject((DestroyPacket)packet);
                }
            }
            _gameObjectDirector.MoveObjects(deltaTime);
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
