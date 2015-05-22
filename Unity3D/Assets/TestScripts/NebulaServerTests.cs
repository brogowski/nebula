using System.Linq;
using Assets.Scripts;
using Nebula;
using Nebula.Connectivity;
using Nebula.Input;
using Nebula.Packets;
using Nebula.Recording;
using Nebula.Serialization;
using Nebula.Transmission;
using UnityEngine;

namespace Assets.TestScripts
{
    class NebulaServerTests : MonoBehaviour
    {
        public string Ip = "127.0.0.1";
        public int Port = 9999;
        public GameObject[] Prefabs;
        public NebulaTestGameLogic GameLogic;

        private NebulaServer _nebula;

        void OnEnable()
        {
            var tcpServer = new TcpListenerTransmissionProtocol(Ip, Port);
            tcpServer.Start();

            _nebula = new NebulaServer(new PacketSender(tcpServer, new PacketSerializer()),
                new RecordedInputReciver(tcpServer, new RecordedInputSerializer()));
        }

        void FixedUpdate()
        {
            _nebula.UpdateRemotePhysics(Time.deltaTime);
            var inputs = _nebula.GetInputForDuration(Time.deltaTime);
            GameLogic.ExecuteInputs(inputs);
        }

        public void AddNewGameObject(GameObject newGameObject)
        {
            _nebula.AddToRemotePhysics(new ReadOnlyStatefulGameObject(new GameObjectWrapper(newGameObject)));
        }
    }

    class PacketSender : AbstractSender<IPacket>
    {
        public PacketSender(ISendTransmissionProtocol protocol, IPacketSerializer<IPacket> serializer) : base(protocol, serializer)
        {
        }
    }

    class RecordedInputReciver : AbstractReciver<RecordedInput>
    {
        public RecordedInputReciver(IReciveTransmissionProtocol transmissionProtocol, IPacketDeserializer<RecordedInput> deserializer) : base(transmissionProtocol, deserializer)
        {
        }
    }
}
