using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Nebula.Transmission
{
    public class TcpListenerTransmissionProtocol : AbstractTransmissionProtocol
    {
        private readonly SynchronizedCollection<TcpClientTransmissionProtocol> _clients;
        private readonly TcpListener _listener;
        private readonly Queue _synchronizedReciveQueue;
        private readonly Queue _synchronizedSendQueue;

        private bool _beenOpened;
        private bool _beenClosed;

        private volatile bool _acceptThreadLoopCondition = true;
        private volatile bool _reciveThreadLoopCondition = true;
        private volatile bool _sendThreadLoopCondition = true;

        internal Thread AcceptThread { get; private set; }
        internal Thread SendThread { get; private set; }
        internal Thread RecieveThread { get; private set; }

        public TcpListenerTransmissionProtocol(string ip, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ip), port);     
       
            _synchronizedReciveQueue = Queue.Synchronized(new Queue());
            _synchronizedSendQueue = Queue.Synchronized(new Queue());
            _clients = new SynchronizedCollection<TcpClientTransmissionProtocol>();

        }

        protected override void OpenConnection()
        {
            _listener.Start();
            SetupThreads();
            _beenOpened = true;
        }
        protected override void CloseConnection()
        {
            _acceptThreadLoopCondition = false;
            _reciveThreadLoopCondition = false;
            _sendThreadLoopCondition = false;
            _listener.Stop();
            foreach (var client in _clients)
            {
                client.Stop();
            }
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

        private void AcceptSocket(TcpListener listener)
        {
            try
            {
                KeepAcceptingClients(listener);
            }
            finally
            {                
                AcceptThread.Abort();
            }
        }
        private void SendMessage()
        {
            try
            {
                KeepSendingMessages();
            }
            finally
            {
                SendThread.Abort();
            }
        }
        private void RecieveMessage()
        {
            try
            {
                KeepRecievingMessages();
            }
            finally
            {
                SendThread.Abort();
            }
        }

        private void KeepSendingMessages()
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
                    SendMessageToAllClients(message);
                }
            }
        }
        private void KeepAcceptingClients(TcpListener listener)
        {
            while (_acceptThreadLoopCondition)
            {
                if (listener.Pending())
                {
                    SetupClient(listener.AcceptTcpClient());
                }
                Thread.Sleep(10);
            }
        }
        private void KeepRecievingMessages()
        {
            while (_reciveThreadLoopCondition)
            {
                foreach (var message in GetMessagesFromClients())
                {
                    _synchronizedReciveQueue.Enqueue(message);
                }
            }
        }

        private IEnumerable<string> GetMessagesFromClients()
        {
            return _clients.SelectMany(q => q.GetPackets());
        }
        private void SendMessageToAllClients(string message)
        {
            foreach (var client in _clients)
            {
                client.SendPacket(message);
            }
        }

        private void SetupClient(TcpClient client)
        {
            _clients.Add(new TcpClientTransmissionProtocol(client));
        }
        private void SetupThreads()
        {
            AcceptThread = new Thread(() => AcceptSocket(_listener)) { IsBackground = true };
            SendThread = new Thread(SendMessage) { IsBackground = true };
            RecieveThread = new Thread(RecieveMessage) { IsBackground = true };

            AcceptThread.Start();
            SendThread.Start();
            RecieveThread.Start();
        }

    }
}
