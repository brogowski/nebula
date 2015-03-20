using System.Collections.Generic;
using System.Linq;

namespace Nebula.Connectivity
{
    public abstract class AbstractReciver<T>
    {
        private readonly IReciveTransmissionProtocol _transmissionProtocol;
        private readonly IPacketDeserializer<T> _deserializer;

        protected AbstractReciver(IReciveTransmissionProtocol transmissionProtocol, IPacketDeserializer<T> deserializer)
        {
            _transmissionProtocol = transmissionProtocol;
            _deserializer = deserializer;
        }

        public IEnumerable<T> Recive()
        {
            return _transmissionProtocol.GetPackets().Select(DeserializePacket);
        }

        private T DeserializePacket(string packet)
        {
            return _deserializer.Deserialize(packet);
        }
    }
}