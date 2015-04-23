using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.Transmission;
namespace Assets.Scripts.Networking
{
    class NetworkController : IDisposable
    {
        private AbstractTransmissionProtocol _connection;

        public void ConnectAsClient(string ipAndPort)
        {
            string ip;
            int port;
            ParseConnectionArgument(ipAndPort, out ip, out port);

            _connection = new TcpClientTransmissionProtocol(ip, port);
            _connection.Start();
        }

        public void ConnectAsServer(string ipAndPort)
        {
            string ip;
            int port;
            ParseConnectionArgument(ipAndPort, out ip, out port);

            _connection = new TcpListenerTransmissionProtocol(ip, port);
            _connection.Start();
        }

        public void SendOnlineMessage(string message)
        {
            _connection.SendPacket(message);
        }

        public IEnumerable<string> GetOnlineMessages()
        {
            return _connection != null ? _connection.GetPackets() : Enumerable.Empty<string>();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Stop();
            }
        }

        private void ParseConnectionArgument(string ipAndPort, out string ip, out int port)
        {
            var data = ipAndPort.Split(':');
            ip = data[0];
            port = int.Parse(data[1]);
        }
    }
}
