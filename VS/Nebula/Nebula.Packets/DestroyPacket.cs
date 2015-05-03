using System;

namespace Nebula.Packets
{
    public struct DestroyPacket : IPacket
    {
        public Guid Id;

        public DestroyPacket(Guid id)
        {
            Id = id;
        }
    }
}
