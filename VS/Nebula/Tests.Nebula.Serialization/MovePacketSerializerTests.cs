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
    [TestFixture]
    public class MovePacketSerializerTests
    {
        private MovePacketSerializer _serializer;
        private IPacketConverter<Guid> _mockGuidSerializer;
        private IPacketConverter<Vector3> _mockVectorSerializer;
        private IPacketConverter<Quaternion> _mockQuaternionSerializer;
        private MovePacket _packet;

        [SetUp]
        public void Setup()
        {
            _packet = new MovePacket(Guid.NewGuid(), new Vector3(), new Quaternion());

            _mockGuidSerializer = Substitute.For<IPacketConverter<Guid>>();
            _mockVectorSerializer = Substitute.For<IPacketConverter<Vector3>>();
            _mockQuaternionSerializer = Substitute.For<IPacketConverter<Quaternion>>();

            _serializer = new MovePacketSerializer(
                _mockGuidSerializer,
                _mockVectorSerializer,
                _mockQuaternionSerializer);
        }

        [Test]
        public void CanSerializeMovePacket()
        {
            _mockGuidSerializer.Serialize(_packet.Id).Returns("A");
            _mockVectorSerializer.Serialize(_packet.Move).Returns("B");
            _mockQuaternionSerializer.Serialize(_packet.Rotation).Returns("C");

            Check.That(_serializer.Serialize(_packet)).IsEqualTo(
                string.Format("A{0}B{0}C", Environment.NewLine));
        }

        [Test]
        public void CanDeserializeMovePacket()
        {
            _mockGuidSerializer.Deserialize("A").Returns(_packet.Id);
            _mockVectorSerializer.Deserialize("B").Returns(_packet.Move);
            _mockQuaternionSerializer.Deserialize("C").Returns(_packet.Rotation);

            Check.That(_serializer.Deserialize(string.Format("A{0}B{0}C", Environment.NewLine)))
                .IsEqualTo(_packet);
        }
    }
}
