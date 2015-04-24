using System;
using Nebula.Connectivity;

namespace Nebula.Serialization
{
    public class GuidSerializer : IPacketConverter<Guid>
    {
        public string Serialize(Guid arg)
        {
            return arg.ToString("N");
        }

        public Guid Deserialize(string packet)
        {
            return new Guid(packet);
        }
    }
}