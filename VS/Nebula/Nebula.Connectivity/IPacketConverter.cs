namespace Nebula.Connectivity
{
    public interface IPacketConverter<T> : IPacketSerializer<T>, IPacketDeserializer<T>
    {
         
    }
}