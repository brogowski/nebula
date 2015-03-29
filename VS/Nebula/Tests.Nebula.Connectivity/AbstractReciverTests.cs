using Nebula.Connectivity;
using NFluent;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Nebula.Connectivity
{
    [TestFixture]
    public class AbstractReciverTests
    {
        private AbstractReciver<int> _reciver;
        private IReciveTransmissionProtocol _protocol;
        private IPacketDeserializer<int> _parser;

        [SetUp]
        public void Setup()
        {
            _protocol = Substitute.For<IReciveTransmissionProtocol>();
            _parser = Substitute.For<IPacketDeserializer<int>>();
            _reciver = new MockReciver(_protocol, _parser);
        }

        [Test]
        public void CanReciveMessages()
        {
            var messages = _reciver.Recive();

            Check.That(messages).IsNotNull();
        }

        [Test]
        public void WhenProtocolHaveMessages_MessagesAreCorrectlyReturned()
        {
            _protocol.GetPackets().Returns(new[] {"1", "2"});
            _parser.Deserialize("1").Returns(1);
            _parser.Deserialize("2").Returns(2);

            var messages = _reciver.Recive();

            Check.That(messages).ContainsExactly(1, 2);
        }
    }

    internal class MockReciver : AbstractReciver<int>
    {
        public MockReciver(IReciveTransmissionProtocol transmissionProtocol, IPacketDeserializer<int> deserializer)
            : base(transmissionProtocol, deserializer)
        {
        }
    }
}
