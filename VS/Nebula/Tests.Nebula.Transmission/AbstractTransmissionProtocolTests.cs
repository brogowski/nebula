using System;
using System.Collections.Generic;
using Nebula.Transmission;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Transmission
{
    [TestFixture]
    public class AbstractTransmissionProtocolTests
    {
        private MockTransmissionProtocol _transmissionProtocol;

        [SetUp]
        public void Setup()
        {
            _transmissionProtocol = new MockTransmissionProtocol();
        }

        [Test]
        public void StartOpensConnection()
        {
            _transmissionProtocol.Start();

            Check.That(_transmissionProtocol.OpenConnectionInvoked).IsTrue();
        }

        [Test]
        public void StopClosesConnection()
        {
            _transmissionProtocol.Stop();

            Check.That(_transmissionProtocol.CloseConnectionInvoked).IsTrue();
        }

        [Test]
        public void WhenConnectionIsNotOpenGetPacketsThrowsException()
        {            
            Check.ThatCode(() => _transmissionProtocol.GetPackets())
                .Throws<InvalidOperationException>().WithMessage("Connection has not been opened.");
        }

        [Test]
        public void WhenConnectionIsClosedGetPacketsThrowsException()
        {
            _transmissionProtocol.IsCloseValue = true;

            Check.ThatCode(() => _transmissionProtocol.GetPackets())
                .Throws<InvalidOperationException>().WithMessage("Connection has been closed.");
        }

        [Test]
        public void GetPacketsReturnsNotNull()
        {
            _transmissionProtocol.IsOpenValue = true;

            Check.That(_transmissionProtocol.GetPackets()).IsNotNull();
        }

        [Test]
        public void GetPacketsReturnsCorrectPackets()
        {
            _transmissionProtocol.IsOpenValue = true;
            _transmissionProtocol.RecivedPackets.Enqueue("A");
            _transmissionProtocol.RecivedPackets.Enqueue("B");
            _transmissionProtocol.RecivedPackets.Enqueue("C");

            Check.That(_transmissionProtocol.GetPackets()).ContainsExactly("A", "B", "C");
        }

        [Test]
        public void SendPacketAddPacketToQueue()
        {
            _transmissionProtocol.IsOpenValue = true;

            _transmissionProtocol.SendPacket("A");

            Check.That(_transmissionProtocol.PacketsToSend).ContainsExactly("A");
        }
    }

    public class MockTransmissionProtocol : AbstractTransmissionProtocol
    {
        protected override void OpenConnection()
        {
            OpenConnectionInvoked = true;
        }
        
        protected override void CloseConnection()
        {
            CloseConnectionInvoked = true;
        }

        protected override bool IsOpen
        {
            get { return IsOpenValue; }
        }

        protected override bool IsClosed
        {
            get { return IsCloseValue; }
        }

        protected override Queue<string> RecivedPacketsQueue
        {
            get { return RecivedPackets; }
        }

        protected override Queue<string> PacketsToSendQueue
        {
            get { return PacketsToSend; }
        }

        public override void Dispose()
        {
            
        }

        public bool OpenConnectionInvoked;
        public bool CloseConnectionInvoked;
        public bool IsOpenValue;
        public bool IsCloseValue;
        public Queue<string> RecivedPackets = new Queue<string>();
        public Queue<string> PacketsToSend = new Queue<string>();
    }
}
