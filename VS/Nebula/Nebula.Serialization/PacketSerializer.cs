using System;
using System.Collections.Generic;
using Nebula.Connectivity;
using Nebula.Packets;

namespace Nebula.Serialization
{
    public class PacketSerializer : IPacketConverter<IPacket>
    {
        private const char Separator = '|';
        public IDictionary<Type, Func<string, IPacket>> Deserializers { get; private set; }
        public IDictionary<Type, Func<IPacket, string>> Serializers { get; private set; }

        public PacketSerializer()
        {
            Deserializers = new Dictionary<Type, Func<string, IPacket>>();
            Serializers = new Dictionary<Type, Func<IPacket, string>>();
        }

        public string Serialize(IPacket arg)
        {
            var packetType = arg.GetType();
            if(!Serializers.ContainsKey(packetType))
                throw new NotSupportedException();

            return FormatSerializedPacket(Serializers[packetType].Invoke(arg), packetType);
        }

        public IPacket Deserialize(string packet)
        {
            var packetType = GetPacketType(packet);
            if (!Deserializers.ContainsKey(packetType))
                throw new NotSupportedException();

            return Deserializers[packetType].Invoke(packet.Split(Separator)[1]);
        }

        private string FormatSerializedPacket(string serializedPacket, Type packetType)
        {
            return string.Format("{0}{1}{2}", packetType.AssemblyQualifiedName, Separator, serializedPacket);
        }

        private Type GetPacketType(string packet)
        {
            var type = packet.Split(Separator)[0];
            return Type.GetType(type) ?? typeof (void);
        }
    }
}