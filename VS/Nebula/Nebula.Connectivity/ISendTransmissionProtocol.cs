namespace Nebula.Connectivity
{
    public interface ISendTransmissionProtocol
    {
        void SendPacket(string packet);
    }
}