namespace Nebula.Connectivity
{
    public abstract class AbstractSender<T>
    {
        private readonly ISendTransmissionProtocol _protocol;
        private readonly IPacketSerializer<T> _serializer;

        protected AbstractSender(ISendTransmissionProtocol protocol, IPacketSerializer<T> serializer)
        {
            _protocol = protocol;
            _serializer = serializer;
        }

        public void Send(T message)
        {
            _protocol.SendPacket(_serializer.Serialize(message));
        }
    }
}