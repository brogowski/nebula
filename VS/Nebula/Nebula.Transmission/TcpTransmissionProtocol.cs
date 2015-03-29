using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Nebula.Transmission
{
    public class TcpTransmissionProtocol : AbstractTransmissionProtocol
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly Queue _synchronizedReciveQueue;

        private TcpClient _tcpClient;
        private bool _isClosed;
        private bool _isOpen;
        private Thread _recieveThread;
        private volatile bool _reciveThreadLoopCondition = true;

        public TcpTransmissionProtocol(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _synchronizedReciveQueue = Queue.Synchronized(new Queue());
        }

        protected override void OpenConnection()
        {
            if(IsOpen)
                throw new InvalidOperationException("Connection has already started");

            _tcpClient = new TcpClient(AddressFamily.InterNetwork)
            {
                NoDelay = true
            };
            _tcpClient.Connect(_ip, _port);
            _isOpen = true;
            _recieveThread = new Thread(ReciveMessages);
            _recieveThread.Start();
        }

        protected override void CloseConnection()
        {
            if (IsClosed)
                throw new InvalidOperationException("Connection has already stopped");

            _tcpClient.Close();
            _reciveThreadLoopCondition = false;
            _isClosed = true;
        }

        protected override bool IsOpen
        {
            get { return _isOpen; }
        }

        protected override bool IsClosed
        {
            get { return _isClosed; }
        }

        protected override Queue RecivedPacketsQueue
        {
            get { return _synchronizedReciveQueue; }
        }

        protected override Queue PacketsToSendQueue
        {
            get { throw new NotImplementedException(); }
        }

        private void ReciveMessages()
        {
            var networkStream = _tcpClient.GetStream();
            var reader = new BinaryReader(networkStream);
            while (_reciveThreadLoopCondition)
            {
                var message = reader.ReadString();
                _synchronizedReciveQueue.Enqueue(message);
            }
            _recieveThread.Abort();
        }
    }
}