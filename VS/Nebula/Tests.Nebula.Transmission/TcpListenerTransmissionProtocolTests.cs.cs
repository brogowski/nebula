using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Nebula.Transmission;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Transmission
{
    [TestFixture]
    public class TcpListenerTransmissionProtocolTests
    {
        private const string Ip = "127.0.0.1";
        private const int Port = 9999;

        private TcpListenerTransmissionProtocol _protocol;
        private TcpClient _testSocket;
        private const string TestMessage = "Test Message";

        [SetUp]
        public void Setup()
        {
            _testSocket = new TcpClient {NoDelay = true};           
            _protocol = new TcpListenerTransmissionProtocol(Ip, Port);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                _protocol.Stop();
                _testSocket.Close();
            }
            catch
            {
            }
        }

        [Test]
        public void StartAllowsConnections()
        {           
            _protocol.Start();
            ConnectTestSocket();

            Check.That(_testSocket.Connected).IsTrue();
        }

        [Test]
        public void StopClosesConnections()
        {
            _protocol.Start();
            ConnectTestSocket();
            _protocol.Stop();            

            try
            {
                _testSocket.GetStream().Write(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6);
                _testSocket.GetStream().Write(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6);
                _testSocket.GetStream().Write(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6);
                _testSocket.GetStream().Flush();
            }
            catch (Exception e)
            {
                Check.That((e.InnerException as SocketException).SocketErrorCode).IsEqualTo(SocketError.ConnectionReset);
            }

            Thread.Sleep(25);

            Check.That(_testSocket.Connected).IsFalse();
        }

        [Test]
        public void PacketIsCorrectlySend()
        {
            _protocol.Start();
            ConnectTestSocket();

            Thread.Sleep(25);

            _protocol.SendPacket(TestMessage);

            Check.That(_testSocket.ReciveMessage()).IsEqualTo(TestMessage);
        }

        [Test]
        public void PacketIsCorrectlyRecieved()
        {
            _protocol.Start();
            ConnectTestSocket();

            _testSocket.SendMessage(TestMessage);

            Thread.Sleep(25);

            Check.That(_protocol.GetPackets()).ContainsExactly(TestMessage);
        }

        [Test]
        public void MultiplePacketsAreCorrectlyRecived()
        {
            _protocol.Start();
            ConnectTestSocket();
            _testSocket.SendMessage("Test Message 1");
            _testSocket.SendMessage("Test Message 2");
            _testSocket.SendMessage("Test Message 3");

            Thread.Sleep(25);

            Check.That(_protocol.GetPackets())
                .ContainsExactly("Test Message 1", "Test Message 2", "Test Message 3");
        }

        [Test]
        public void MultiplePacketsAreCorrectlySend()
        {
            var testMessages = new[] { "Test Message 1", "Test Message 2", "Test Message 3" };
            _protocol.Start();
            ConnectTestSocket();
            Thread.Sleep(25);
            foreach (var message in testMessages)
            {
                _protocol.SendPacket(message);
            }
            Thread.Sleep(25);

            var recivedMessages = new string[3];
            for (int i = 0; i < 3; i++)
            {
                recivedMessages[i] = _testSocket.ReciveMessage();
            }

            Check.That(recivedMessages).ContainsExactly(testMessages);
        }

        [Test]
        public void BigPacketIsCorrectlyRecived()
        {
            _protocol.Start();
            ConnectTestSocket();

            var bigString = Enumerable.Range(0, 512)
                .Select(q => "a")
                .Aggregate((source, text) => source + text);

            _testSocket.SendMessage(bigString);

            Thread.Sleep(100);

            Check.That(_protocol.GetPackets()).ContainsExactly(bigString);
        }

        [Test]
        public void SendAndReciveWorksTogether()
        {
            const string clientTestMessage = TestMessage + "C";
            const string serverTestMessage = TestMessage + "S";
            _protocol.Start();
            ConnectTestSocket();
            Thread.Sleep(25);
            
            _protocol.SendPacket(clientTestMessage);
            _testSocket.SendMessage(serverTestMessage);

            Thread.Sleep(25);

            Check.That(_protocol.GetPackets()).ContainsExactly(serverTestMessage);
            Check.That(_testSocket.ReciveMessage()).IsEqualTo(clientTestMessage);
        }

        [Test]
        public void MultipleClientsAreSupported()
        {
            var clients = new[]
            {
                new TcpClient {NoDelay = true},
                new TcpClient {NoDelay = true},
                new TcpClient {NoDelay = true},
                new TcpClient {NoDelay = true},
            };

            _protocol.Start();

            foreach (var client in clients)
            {
                client.Connect(Ip, Port);
            }

            Thread.Sleep(100);

            for (int index = 0; index < clients.Length; index++)
            {
                clients[index].SendMessage("Client: " + index);
            }           
 
            _protocol.SendPacket("Server");

            Thread.Sleep(100);

            foreach (var client in clients)
            {
                Check.That(client.ReciveMessage()).IsEqualTo("Server");
            }

            Check.That(_protocol.GetPackets()).Contains(
                Enumerable.Range(0, clients.Length)
                .Select(i => "Client: " + i));
        }

        [Test]
        public void ThreadsAreCorrectlyDisposed()
        {
            _protocol.Start();
            ConnectTestSocket();

            _protocol.Stop();
            _testSocket.Close();

            Thread.Sleep(25);

            Check.That(_protocol.RecieveThread.ThreadState).IsEqualTo(ThreadState.Stopped);
            Check.That(_protocol.SendThread.ThreadState).IsEqualTo(ThreadState.Stopped);
            Check.That(_protocol.SendThread.ThreadState).IsEqualTo(ThreadState.Stopped);
        }

        private void ConnectTestSocket()
        {
            _testSocket.Connect(Ip, Port);
        }
    }

    static class TcpClientHelper
    {
        public static void SendMessage(this TcpClient connectedSocket, string message)
        {
            var writer = new BinaryWriter(connectedSocket.GetStream());
            writer.Write(message);
            writer.Flush();
        }
        public static string ReciveMessage(this TcpClient connectedSocket)
        {
            string message = string.Empty;
            var thread = new Thread(() =>
            {
                var reader = new BinaryReader(connectedSocket.GetStream());
                message = reader.ReadString();
            });

            thread.Start();
            var success = thread.Join(TimeSpan.FromSeconds(1));
            if (!success)
                throw new TimeoutException();

            return message;
        }

    }
}
