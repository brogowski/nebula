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
        public GameObject Prefab;
        public NebulaTestGameLogic GameLogic;

        private TcpListenerTransmissionProtocol _transmissionProtocol;
        private NebulaServer _nebula;

        void OnEnable()
        {
            _transmissionProtocol = new TcpListenerTransmissionProtocol(Ip, Port);
            _transmissionProtocol.Start();

            _nebula = new NebulaServer(new PacketSender(_transmissionProtocol, SetupPacketSerializer()),
                new RecordedInputReciver(_transmissionProtocol, new RecordedInputSerializer()));
        }

        private PacketSerializer SetupPacketSerializer()
        {
            var packetSerializer = new PacketSerializer();
            var movePacketSerializer = new MovePacketSerializer(new GuidSerializer(), new Vector3Serializer(),
                new QuaternionSerializer());
            var spawnPacketSerializer = new SpawnPacketSerializer(new GuidSerializer(), new Vector3Serializer(),
                new QuaternionSerializer());
            var destroyPacketSerializer = new DestroyPacketSerializer();
            packetSerializer.Serializers.Add(typeof (MovePacket), packet => movePacketSerializer.Serialize((MovePacket) packet));
            packetSerializer.Serializers.Add(typeof (SpawnPacket), packet => spawnPacketSerializer.Serialize((SpawnPacket) packet));
            packetSerializer.Serializers.Add(typeof (DestroyPacket), packet => destroyPacketSerializer.Serialize(((DestroyPacket) packet).Id));
            return packetSerializer;
        }

        void FixedUpdate()
        {
            _nebula.UpdateRemotePhysics(Time.deltaTime);
            var inputs = _nebula.GetInputForDuration(Time.deltaTime);
            GameLogic.ExecuteInputs(inputs);
        }

        void OnDestroy()
        {
            _transmissionProtocol.Stop();
        }

        public void AddNewGameObject(GameObject newGameObject)
        {
            _nebula.AddToRemotePhysics(new ReadOnlyStatefulGameObject(new GameObjectWrapper(newGameObject, Prefab.name)));
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
