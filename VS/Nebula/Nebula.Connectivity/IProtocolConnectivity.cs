using System;

namespace Nebula.Connectivity
{
    public interface IProtocolConnectivity : IDisposable
    {
        void Start();
        void Stop();
    }
}