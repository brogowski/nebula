using System;
using Nebula.Connectivity;
using Nebula.Packets;
using Nebula.Serialization;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Nebula.Serialization
{
    public class SpawnPacketSerializerTests
    {
        private SpawnPacketSerializer _serializer;
        private IPacketConverter<Guid> _mockGuidSerializer;
        private IPacketConverter<Vector3> _mockVectorSerializer;
        private IPacketConverter<Quaternion> _mockQuaternionSerializer;
        private SpawnPacket _packet;

        [SetUp]
        public void Setup()
        {
            _packet = new SpawnPacket(Guid.NewGuid(), new Vector3(), new Quaternion(), "Cat");

            _mockGuidSerializer = Substitute.For<IPacketConverter<Guid>>();
            _mockVectorSerializer = Substitute.For<IPacketConverter<Vector3>>();
            _mockQuaternionSerializer = Substitute.For<IPacketConverter<Quaternion>>();

            _serializer = new SpawnPacketSerializer(
                _mockGuidSerializer,
                _mockVectorSerializer,
                _mockQuaternionSerializer);
        }

        [Test]
        public void CanSerializeSpawnPacket()
        {
            _mockGuidSerializer.Serialize(_packet.Id).Returns("A");
            _mockVectorSerializer.Serialize(_packet.Position).Returns("B");
            _mockQuaternionSerializer.Serialize(_packet.Rotation).Returns("C");

            Check.That(_serializer.Serialize(_packet)).IsEqualTo(
                string.Format("A{0}B{0}C{0}Cat", Environment.NewLine));
        }

        [Test]
        public void CanDeserializeSpawnPacket()
        {
            _mockGuidSerializer.Deserialize("A").Returns(_packet.Id);
            _mockVectorSerializer.Deserialize("B").Returns(_packet.Position);
            _mockQuaternionSerializer.Deserialize("C").Returns(_packet.Rotation);

            Check.That(_serializer.Deserialize(string.Format("A{0}B{0}C{0}Cat", Environment.NewLine)))
                .IsEqualTo(_packet);
        }
    }
}
