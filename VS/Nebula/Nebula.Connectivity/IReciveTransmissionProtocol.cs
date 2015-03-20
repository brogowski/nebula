using System.Collections.Generic;

namespace Nebula.Connectivity
{
    public interface IReciveTransmissionProtocol
    {
        IEnumerable<string> GetPackets();
    }
}