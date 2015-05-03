using System;
using Nebula.Connectivity;
using Nebula.Packets;
using Nebula.Serialization;
using NFluent;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class PacketSerializerTests
    {
        private PacketSerializer _serializer;
        private IPacketConverter<MockPacket> _mockConverter;
        private MockPacket _mockPacket;
        private string _serializatedMockPacket;
        private string _packetHeader;

        [SetUp]
        public void Setup()
        {
            _mockPacket = new MockPacket();
            _packetHeader = typeof (MockPacket).AssemblyQualifiedName;
            _serializatedMockPacket = "Dummy";
            _mockConverter = Substitute.For<IPacketConverter<MockPacket>>();
            _mockConverter.Serialize(_mockPacket).Returns(_serializatedMockPacket);
            _mockConverter.Deserialize(_serializatedMockPacket).Returns(_mockPacket);
            _serializer = new PacketSerializer();
        }

        [Test]
        public void WhenTypeIsNotSupported_Serialization_ThrowsNotSupportedException()
        {
            Check.ThatCode(() => _serializer.Serialize(_mockPacket)).Throws<NotSupportedException>();
        }

        [Test]
        public void WhenTypeIsSupported_SerializationIsSuccessful()
        {
            _serializer.Serializers.Add(typeof(MockPacket), packet => _mockConverter.Serialize((MockPacket)packet));

            Check.That(_serializer.Serialize(_mockPacket)).IsEqualTo(_packetHeader + "|" + _serializatedMockPacket);
        }

        [Test]
        public void WhenTypeIsNotSupported_Deserialization_ThrowsNotSupportedException()
        {
            Check.ThatCode(() => _serializer.Deserialize(_serializatedMockPacket)).Throws<NotSupportedException>();
        }

        [Test]
        public void WhenTypeIsSupported_DeserializationIsSuccessful()
        {
            _serializer.Deserializers.Add(typeof(MockPacket), serializedPacket => _mockConverter.Deserialize(serializedPacket));

            Check.That(_serializer.Deserialize(_packetHeader + "|" + _serializatedMockPacket)).IsEqualTo(_mockPacket);
        }
    }

    public class MockPacket : IPacket
    {
        
    }
}
