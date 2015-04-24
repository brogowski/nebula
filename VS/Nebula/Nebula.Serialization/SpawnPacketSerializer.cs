using System;
using Nebula.Connectivity;
using Nebula.Packets;
using UnityEngine;

namespace Nebula.Serialization
{
    public class SpawnPacketSerializer : CombinedPacketSerializer<SpawnPacket>
    {
        private readonly IPacketConverter<Guid> _guidSerializer;
        private readonly IPacketConverter<Vector3> _vectorSerializer;
        private readonly IPacketConverter<Quaternion> _quaternionSerializer;

        public SpawnPacketSerializer(
            IPacketConverter<Guid> guidSerializer,
            IPacketConverter<Vector3> vectorSerializer,
            IPacketConverter<Quaternion> quaternionSerializer)
        {
            _guidSerializer = guidSerializer;
            _vectorSerializer = vectorSerializer;
            _quaternionSerializer = quaternionSerializer;            
        }

        protected override string[] SerializePacketToFragments(SpawnPacket packet)
        {
            return new[]
            {
                _guidSerializer.Serialize(packet.Id),
                _vectorSerializer.Serialize(packet.Position),
                _quaternionSerializer.Serialize(packet.Rotation),
                packet.Type
            };
        }

        protected override SpawnPacket DeserializeFromFragments(string[] split)
        {
            return new SpawnPacket(
                _guidSerializer.Deserialize(split[0]),
                _vectorSerializer.Deserialize(split[1]),
                _quaternionSerializer.Deserialize(split[2]),
                split[3]);
        }
    }
}