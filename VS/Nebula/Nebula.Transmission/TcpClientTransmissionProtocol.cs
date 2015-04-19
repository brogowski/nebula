using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Nebula.Transmission
{
    public class TcpClientTransmissionProtocol : AbstractTransmissionProtocol
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly Queue _synchronizedReciveQueue;
        private readonly Queue _synchronizedSendQueue;

        private TcpClient _tcpClient;
        private bool _beenClosed;
        private bool _beenOpened;

        private volatile bool _reciveThreadLoopCondition = true;
        private volatile bool _sendThreadLoopCondition = true;

        internal Thread RecieveThread { get; private set; }
        internal Thread SendThread { get; private set; }

        public TcpClientTransmissionProtocol(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _synchronizedReciveQueue = Queue.Synchronized(new Queue());
            _synchronizedSendQueue = Queue.Synchronized(new Queue());
        }

        public TcpClientTransmissionProtocol(TcpClient client)
        {
            ValidateClient(client);

            _tcpClient = client;
            _ip = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
            _port = (client.Client.RemoteEndPoint as IPEndPoint).Port;

            _synchronizedReciveQueue = Queue.Synchronized(new Queue());
            _synchronizedSendQueue = Queue.Synchronized(new Queue());

            SetupThreads();

            _beenOpened = true;
        }

        protected override void OpenConnection()
        {
            _tcpClient = new TcpClient(AddressFamily.InterNetwork)
            {
                NoDelay = true
            };
            _tcpClient.Connect(_ip, _port);
            _beenOpened = true;            
            
            SetupThreads();
        }
        protected override void CloseConnection()
        {
            _reciveThreadLoopCondition = false;
            _sendThreadLoopCondition = false;            
            _tcpClient.Close();
            _beenClosed = true;
        }

        protected override bool BeenOpened
        {
            get { return _beenOpened; }
        }
        protected override bool BeenClosed
        {
            get { return _beenClosed; }
        }

        protected override Queue RecivedPacketsQueue
        {
            get { return _synchronizedReciveQueue; }
        }
        protected override Queue PacketsToSendQueue
        {
            get { return _synchronizedSendQueue; }
        }

        private void ReciveMessages(Stream networkStream)
        {
            var reader = new BinaryReader(networkStream);
            try
            {
                KeepReceivingMessages(reader);
            }
            finally
            {
                RecieveThread.Abort();                
            }
        }
        private void SendMessages(Stream networkStream)
        {
            var writer = new BinaryWriter(networkStream);
            try
            {
                KeepSendingMessages(writer);
            }
            finally
            {
                SendThread.Abort();
            }
        }

        private void KeepReceivingMessages(BinaryReader reader)
        {
            while (_reciveThreadLoopCondition)
            {
                var message = reader.ReadString();
                _synchronizedReciveQueue.Enqueue(message);
            }
        }
        private void KeepSendingMessages(BinaryWriter reader)
        {
            while (_sendThreadLoopCondition)
            {
                if (_synchronizedSendQueue.Count < 1)
                {
                    Thread.Sleep(10);
                    continue;
                }

                var message = _synchronizedSendQueue.Dequeue() as string;
                if (message != null)
                {
                    reader.Write(message);
                }
            }
        }

        private void ValidateClient(TcpClient client)
        {
            if (!client.Connected)
                throw new NotSupportedException("Client must be connected!");

            var endpoint = client.Client.RemoteEndPoint as IPEndPoint;
            if (endpoint == null)
            {
                throw new NotSupportedException("Only IP connections are supported");
            }
        }
        private void SetupThreads()
        {
            var networkStream = _tcpClient.GetStream();

            RecieveThread = new Thread(() => ReciveMessages(networkStream)) { IsBackground = true };
            SendThread = new Thread(() => SendMessages(networkStream)) { IsBackground = true };
            RecieveThread.Start();
            SendThread.Start();
        }
    }
}