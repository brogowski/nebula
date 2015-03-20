namespace Nebula.Connectivity
{
    public interface IPacketDeserializer<out T>
    {
        T Deserialize(string packet);
    }
}