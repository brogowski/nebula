using System;
using System.Globalization;
using Nebula.Connectivity;
using Nebula.Packets;
using UnityEngine;

namespace Nebula.Serialization
{
    public class MovePacketSerializer : CombinedPacketSerializer<MovePacket>
    {
        private readonly IPacketConverter<Guid> _guidSerializer;
        private readonly IPacketConverter<Vector3> _vectorSerializer;
        private readonly IPacketConverter<Quaternion> _quaternionSerializer;
        private readonly CultureInfo _floatFormat = CultureInfo.InvariantCulture;

        public MovePacketSerializer(
            IPacketConverter<Guid> guidSerializer,
            IPacketConverter<Vector3> vectorSerializer,
            IPacketConverter<Quaternion> quaternionSerializer)
        {
            _guidSerializer = guidSerializer;
            _vectorSerializer = vectorSerializer;
            _quaternionSerializer = quaternionSerializer;
        }

        protected override string[] SerializePacketToFragments(MovePacket packet)
        {
            return new[]
            {
                _guidSerializer.Serialize(packet.Id),
                _vectorSerializer.Serialize(packet.Move),
                _quaternionSerializer.Serialize(packet.Rotation),
                packet.Duration.ToString(_floatFormat)
            };
        }

        protected override MovePacket DeserializeFromFragments(string[] split)
        {
            return new MovePacket(
                _guidSerializer.Deserialize(split[0]),
                _vectorSerializer.Deserialize(split[1]),
                _quaternionSerializer.Deserialize(split[2]),
                float.Parse(split[3], _floatFormat));
        }
    }
}