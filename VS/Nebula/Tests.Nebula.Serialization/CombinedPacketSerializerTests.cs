using System;
using Nebula.Serialization;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class CombinedPacketSerializerTests
    {
        private CombinedPacketSerializer<string> _packetSerializer;

        [SetUp]
        public void Setup()
        {
            _packetSerializer = new DummyPacketSerializer();
        }

        [Test]
        public void CanSerializePacket()
        {            
            var result = _packetSerializer.Serialize(@"Test Test Test");
            Check.That(result).IsEqualTo(string.Format("Test{0}Test{0}Test", Environment.NewLine));
        }

        [Test]
        public void CanDeSerializePacket()
        {
            var result = _packetSerializer.Deserialize(string.Format("Test{0}Test{0}Test", Environment.NewLine));
            Check.That(result).IsEqualTo("TestTestTest");
        }
    }

    public class DummyPacketSerializer : CombinedPacketSerializer<string>
    {
        protected override string[] SerializePacketToFragments(string packet)
        {
            return packet.Split(' ');
        }

        protected override string DeserializeFromFragments(string[] split)
        {
            return string.Join("", split);
        }
    }
}
