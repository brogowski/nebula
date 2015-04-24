using Nebula.Connectivity;
using Nebula.Packets;

namespace Nebula.Serialization
{
    public class DestroyPacketSerializer : GuidSerializer, IPacketConverter<DestroyPacket>
    {
        string IPacketSerializer<DestroyPacket>.Serialize(DestroyPacket arg)
        {
            return Serialize(arg.Id);
        }

        DestroyPacket IPacketDeserializer<DestroyPacket>.Deserialize(string packet)
        {
            return new DestroyPacket(Deserialize(packet));
        }
    }
}