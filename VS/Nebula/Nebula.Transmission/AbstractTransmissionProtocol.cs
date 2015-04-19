using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.Connectivity;

namespace Nebula.Transmission
{
    public abstract class AbstractTransmissionProtocol : 
        IReciveTransmissionProtocol, ISendTransmissionProtocol, IProtocolConnectivity
    {
        protected abstract void OpenConnection();
        protected abstract void CloseConnection();
        protected abstract bool BeenOpened { get; }
        protected abstract bool BeenClosed { get; }
        protected abstract Queue RecivedPacketsQueue { get; }
        protected abstract Queue PacketsToSendQueue { get; } 

        public IEnumerable<string> GetPackets()
        {
            ValidateSocket();

            var returnList = new List<string>();
            var count = RecivedPacketsQueue.Count;
            for (int i = 0; i < count; i++)
            {
                var message = RecivedPacketsQueue.Dequeue() as string;
                if (message != null)
                {
                    returnList.Add(message);
                }
            }
            return returnList;
        }

        public void SendPacket(string packet)
        {
            ValidateSocket();

            PacketsToSendQueue.Enqueue(packet);
        }

        public void Start()
        {
            if (BeenOpened)
                throw new InvalidOperationException("Connection has already started");

            OpenConnection();
        }

        public void Stop()
        {
            if (BeenClosed)
                throw new InvalidOperationException("Connection has already stopped");

            CloseConnection();
        }

        private void ValidateSocket()
        {
            if (BeenClosed)
                throw new InvalidOperationException("Connection has been closed.");
            if (!BeenOpened)
                throw new InvalidOperationException("Connection has not been opened.");
        }
    }
}