using System;
using Nebula.Serialization;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class GuidSerializerTests
    {
        private GuidSerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = new GuidSerializer();
        }

        [Test]
        public void CanSerializeGuid()
        {
            var id = Guid.NewGuid();
            var result = _serializer.Serialize(id);
            Check.That(result).IsEqualTo(id.ToString("N"));
        }

        [Test]
        public void CanDeserializeGuid()
        {
            var id = Guid.NewGuid();
            var result = _serializer.Deserialize(id.ToString("N"));
            Check.That(result).IsEqualTo(id);
        }
    }
}
