using Nebula.Connectivity;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Nebula.Connectivity
{
    [TestFixture]
    public class AbstractSenderTests
    {
        private MockSender _sender;
        private ISendTransmissionProtocol _protocol;
        private IPacketSerializer<int> _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = Substitute.For<IPacketSerializer<int>>();
            _protocol = Substitute.For<ISendTransmissionProtocol>();
            _sender = new MockSender(_protocol, _serializer);
        }

        [Test]
        public void MessageIsSerializedBeforeSend()
        {
            _serializer.Serialize(20).Returns("2000");

            _sender.Send(20);

            _protocol.Received().SendPacket("2000");
        }
    }

    internal class MockSender : AbstractSender<int>
    {
        public MockSender(ISendTransmissionProtocol protocol, IPacketSerializer<int> serializer) : base(protocol, serializer)
        {
        }
    }
}
