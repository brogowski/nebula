using System;
using System.Collections.Generic;
using Nebula.Connectivity;

namespace Nebula.Transmission
{
    public abstract class AbstractTransmissionProtocol : 
        IReciveTransmissionProtocol, ISendTransmissionProtocol, IProtocolConnectivity
    {
        protected abstract void OpenConnection();
        protected abstract void CloseConnection();
        protected abstract bool IsOpen { get; }
        protected abstract bool IsClosed { get; }
        protected abstract Queue<string> RecivedPacketsQueue { get; }
        protected abstract Queue<string> PacketsToSendQueue { get; } 

        public IEnumerable<string> GetPackets()
        {
            if(IsClosed)
                throw new InvalidOperationException("Connection has been closed.");
            if(!IsOpen)
                throw new InvalidOperationException("Connection has not been opened.");

            var returnList = new List<string>();
            var count = RecivedPacketsQueue.Count;
            for (int i = 0; i < count; i++)
            {
                returnList.Add(RecivedPacketsQueue.Dequeue());
            }
            return returnList;
        }

        public void SendPacket(string packet)
        {
            PacketsToSendQueue.Enqueue(packet);
        }

        public void Start()
        {
            OpenConnection();
        }

        public void Stop()
        {
            CloseConnection();
        }

        public abstract void Dispose();
    }
}