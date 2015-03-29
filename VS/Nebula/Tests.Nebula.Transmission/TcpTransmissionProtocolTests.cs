using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Nebula.Transmission;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Transmission
{
    [TestFixture]
    public class TcpTransmissionProtocolTests
    {
        private const string Ip = "127.0.0.1";
        private const int Port = 9999;

        private TcpTransmissionProtocol _protocol;
        private TcpListener _testSocket;

        [SetUp]
        public void Setup()
        {
            _testSocket = new TcpListener(IPAddress.Parse(Ip), Port);
            _testSocket.Start();

            _protocol = new TcpTransmissionProtocol(Ip, Port);
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

        private void SendMessage(Socket connectedSocket, string message)
        {
            var memoryStream = new MemoryStream();
            var writer = new BinaryWriter(memoryStream);
            writer.Write(message);
            writer.Flush();

            connectedSocket.Send(memoryStream.ToArray());
        }

        [Test]
        public void StartConnectsToSocket()
        {           
            _protocol.Start();

            Check.That(_testSocket.Pending()).IsTrue();
        }

        [Test]
        public void StartSecondTimeThrowsException()
        {
            _protocol.Start();

            Check.ThatCode(() => _protocol.Start())
                .Throws<InvalidOperationException>()
                .WithMessage("Connection has already started");
        }

        [Test]
        public void StopCloseConnection()
        {
            _protocol.Start();

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
        public void StopSecondTimeThrowsException()
        {
            _protocol.Start();
            _testSocket.AcceptSocket();
            _protocol.Stop();

            Check.ThatCode(() => _protocol.Stop())
                .Throws<InvalidOperationException>()
                .WithMessage("Connection has already stopped");
        }

        [Test]
        public void PacketIsCorrectlyRecived()
        {
            _protocol.Start();

            var connectedSocket = _testSocket.AcceptSocket();
            SendMessage(connectedSocket, "Test Message");

            Check.That(_protocol.GetPackets()).ContainsExactly("Test Message");
        }

        [Test]
        public void MultiplePacketsAreCorrectlyRecived()
        {
            _protocol.Start();

            var connectedSocket = _testSocket.AcceptSocket();
            SendMessage(connectedSocket, "Test Message 1");
            SendMessage(connectedSocket, "Test Message 2");
            SendMessage(connectedSocket, "Test Message 3");

            Check.That(_protocol.GetPackets())
                .ContainsExactly("Test Message 1", "Test Message 2", "Test Message 3");
        }

        [Test]
        public void BigPacketIsCorrectlyRecived()
        {
            _protocol.Start();

            var bigString = Enumerable.Range(0, 512)
                .Select(q => "a")
                .Aggregate((source, text) => source + text);

            var connectedSocket = _testSocket.AcceptSocket();
            SendMessage(connectedSocket, bigString);

            Check.That(_protocol.GetPackets()).ContainsExactly(bigString);
        }
    }
}
