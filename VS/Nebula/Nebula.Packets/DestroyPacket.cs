using System;

namespace Nebula.Packets
{
    public struct DestroyPacket
    {
        public Guid Id;

        public DestroyPacket(Guid id)
        {
            Id = id;
        }
    }
}
