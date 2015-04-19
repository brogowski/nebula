using System;
using System.Collections;
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
            _transmissionProtocol.BeenCloseValue = true;

            Check.ThatCode(() => _transmissionProtocol.GetPackets())
                .Throws<InvalidOperationException>().WithMessage("Connection has been closed.");
        }

        [Test]
        public void GetPacketsReturnsNotNull()
        {
            _transmissionProtocol.BeenOpenedValue = true;

            Check.That(_transmissionProtocol.GetPackets()).IsNotNull();
        }

        [Test]
        public void GetPacketsReturnsCorrectPackets()
        {
            _transmissionProtocol.BeenOpenedValue = true;
            _transmissionProtocol.RecivedPackets.Enqueue("A");
            _transmissionProtocol.RecivedPackets.Enqueue("B");
            _transmissionProtocol.RecivedPackets.Enqueue("C");

            Check.That(_transmissionProtocol.GetPackets()).ContainsExactly("A", "B", "C");
        }

        [Test]
        public void SendPacketAddPacketToQueue()
        {
            _transmissionProtocol.BeenOpenedValue = true;

            _transmissionProtocol.SendPacket("A");

            Check.That(_transmissionProtocol.PacketsToSend).ContainsExactly("A");
        }

        [Test]
        public void StartSecondTimeThrowsException()
        {
            _transmissionProtocol.BeenOpenedValue = true;

            Check.ThatCode(() => _transmissionProtocol.Start())
                .Throws<InvalidOperationException>()
                .WithMessage("Connection has already started");
        }

        [Test]
        public void StopSecondTimeThrowsException()
        {
            _transmissionProtocol.BeenCloseValue = true;

            Check.ThatCode(() => _transmissionProtocol.Stop())
                .Throws<InvalidOperationException>()
                .WithMessage("Connection has already stopped");
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

        protected override bool BeenOpened
        {
            get { return BeenOpenedValue; }
        }

        protected override bool BeenClosed
        {
            get { return BeenCloseValue; }
        }

        protected override Queue RecivedPacketsQueue
        {
            get { return RecivedPackets; }
        }

        protected override Queue PacketsToSendQueue
        {
            get { return PacketsToSend; }
        }

        public bool OpenConnectionInvoked;
        public bool CloseConnectionInvoked;
        public bool BeenOpenedValue;
        public bool BeenCloseValue;
        public Queue RecivedPackets = new Queue();
        public Queue PacketsToSend = new Queue();
    }
}
