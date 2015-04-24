using System;
using Nebula.Connectivity;

namespace Nebula.Serialization
{
    public abstract class CombinedPacketSerializer<T> : IPacketConverter<T>
    {
        private readonly string _separator = Environment.NewLine;

        public string Serialize(T arg)
        {
            var fragments = SerializePacketToFragments(arg);
            return string.Join(_separator, fragments);
        }
        public T Deserialize(string packet)
        {
            return DeserializeFromFragments(packet.Split(new [] { _separator },
                StringSplitOptions.RemoveEmptyEntries));
        }

        protected abstract string[] SerializePacketToFragments(T packet);
        protected abstract T DeserializeFromFragments(string[] split);
    }
}