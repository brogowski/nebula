using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Nebula.Transmission;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Transmission
{
    [TestFixture]
    public class TcpClientTransmissionProtocolTests
    {
        private const string Ip = "127.0.0.1";
        private const int Port = 9999;
        private const string TestMessage = "Test Message";

        private TcpClientTransmissionProtocol _protocol;
        private TcpListener _testSocket;

        [SetUp]
        public void Setup()
        {
            _testSocket = new TcpListener(IPAddress.Parse(Ip), Port);
            _testSocket.Start();
        }

        private void CreateConnectedProtocolFromTcpClient()
        {
            var client = new TcpClient {NoDelay = true};
            client.Connect(Ip, Port);
            _protocol = new TcpClientTransmissionProtocol(client);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                _testSocket.Stop();
                _protocol.Stop();
            }
            catch
            {
            }
        }

        [Test]
        public void StartConnectsToSocket()
        {
            _protocol = new TcpClientTransmissionProtocol(Ip, Port);
            _protocol.Start();

            Check.That(_testSocket.Pending()).IsTrue();
        }

        [Test]
        public void StopCloseConnection()
        {
            CreateConnectedProtocolFromTcpClient();

            var connectedSocket = _testSocket.AcceptSocket();

            _protocol.Stop();

            try
            {
                connectedSocket.Send(new byte[] { 1, 2, 3, 4, 5, 6 });
                connectedSocket.Send(new byte[] { 1, 2, 3, 4, 5, 6 });
            }
            catch (SocketException e)
            {
                Check.That(e.SocketErrorCode).IsEqualTo(SocketError.ConnectionAborted);
            }

            Check.That(connectedSocket.Connected).IsFalse();
        }

        [Test]
        public void PacketIsCorrectlyRecived()
        {
            CreateConnectedProtocolFromTcpClient();

            var connectedSocket = _testSocket.AcceptSocket();
            connectedSocket.SendMessage(TestMessage);

            Thread.Sleep(25);

            Check.That(_protocol.GetPackets()).ContainsExactly(TestMessage);
        }

        [Test]
        public void PacketIsCorrectlySend()
        {
            CreateConnectedProtocolFromTcpClient();
            var connectedSocket = _testSocket.AcceptSocket();

            _protocol.SendPacket(TestMessage);

            Thread.Sleep(10);

            Check.That(connectedSocket.ReciveMessage(TestMessage.Length)).IsEqualTo(TestMessage);
        }

        [Test]
        public void MultiplePacketsAreCorrectlyRecived()
        {
            CreateConnectedProtocolFromTcpClient();

            var connectedSocket = _testSocket.AcceptSocket();
            connectedSocket.SendMessage("Test Message 1");
            connectedSocket.SendMessage("Test Message 2");
            connectedSocket.SendMessage("Test Message 3");

            Thread.Sleep(25);

            Check.That(_protocol.GetPackets())
                .ContainsExactly("Test Message 1", "Test Message 2", "Test Message 3");
        }

        [Test]
        public void MultiplePacketsAreCorrectlySend()
        {
            var testMessages = new[] {"Test Message 1", "Test Message 2", "Test Message 3"};
            CreateConnectedProtocolFromTcpClient();
            var connectedSocket = _testSocket.AcceptSocket();

            foreach (var message in testMessages)
            {
                _protocol.SendPacket(message);
            }

            Thread.Sleep(25);

            var recivedMessages = new string[3];
            for (int i = 0; i < 3; i++)
            {
                recivedMessages[i] = connectedSocket.ReciveMessage("Test Message X".Length);
            }

            Check.That(recivedMessages).ContainsExactly(testMessages);
        }

        [Test]
        public void BigPacketIsCorrectlyRecived()
        {
            CreateConnectedProtocolFromTcpClient();

            var bigString = Enumerable.Range(0, 512)
                .Select(q => "a")
                .Aggregate((source, text) => source + text);

            var connectedSocket = _testSocket.AcceptSocket();
            connectedSocket.SendMessage(bigString);

            Check.That(_protocol.GetPackets()).ContainsExactly(bigString);
        }

        [Test]
        public void SendAndReciveWorksTogether()
        {
            const string clientTestMessage = TestMessage + "C";
            const string serverTestMessage = TestMessage + "S";
            CreateConnectedProtocolFromTcpClient();
            var connectedSocket = _testSocket.AcceptSocket();

            _protocol.SendPacket(clientTestMessage);
            connectedSocket.SendMessage(serverTestMessage);

            Thread.Sleep(10);

            Check.That(_protocol.GetPackets()).ContainsExactly(serverTestMessage);
            Check.That(connectedSocket.ReciveMessage(clientTestMessage.Length))
                .IsEqualTo(clientTestMessage);
        }

        [Test]
        public void ThreadsAreCorrectlyDisposed()
        {
            CreateConnectedProtocolFromTcpClient();
            var connectedSocket = _testSocket.AcceptSocket();

            _protocol.Stop();
            connectedSocket.Disconnect(false);

            Thread.Sleep(10);

            Check.That(_protocol.RecieveThread.ThreadState).IsEqualTo(ThreadState.Stopped);
            Check.That(_protocol.SendThread.ThreadState).IsEqualTo(ThreadState.Stopped);
        }
    }

    static class TcpListenerHelper
    {
        public static void SendMessage(this Socket connectedSocket, string message)
        {
            var memoryStream = new MemoryStream();
            var writer = new BinaryWriter(memoryStream);
            writer.Write(message);
            writer.Flush();

            connectedSocket.Send(memoryStream.ToArray());
        }
        public static string ReciveMessage(this Socket connectedSocket, int messageLength)
        {
            string message = string.Empty;
            var thread = new Thread(() =>
            {
                var buffer = new byte[messageLength + 1];
                connectedSocket.Receive(buffer);
                var reader = new BinaryReader(new MemoryStream(buffer));
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
