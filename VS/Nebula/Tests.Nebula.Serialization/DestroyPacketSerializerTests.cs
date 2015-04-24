using System;
using Nebula.Connectivity;
using Nebula.Packets;
using Nebula.Serialization;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class DestroyPacketSerializerTests
    {
        private IPacketConverter<DestroyPacket> _serializer;
        private DestroyPacket _packet;

        [SetUp]
        public void Setup()
        {
            _packet = new DestroyPacket(Guid.NewGuid());

            _serializer = new DestroyPacketSerializer();
        }

        [Test]
        public void CanSerializeMovePacket()
        {
            Check.That(_serializer.Serialize(_packet))
                .IsEqualTo(_packet.Id.ToString("N"));
        }

        [Test]
        public void CanDeserializeMovePacket()
        {
            Check.That(_serializer.Deserialize(_packet.Id.ToString("N")))
                .IsEqualTo(_packet);
        }
    }
}
