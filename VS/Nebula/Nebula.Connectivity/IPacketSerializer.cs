namespace Nebula.Connectivity
{
    public interface IPacketSerializer<in T>
    {
        string Serialize(T arg);
    }
}