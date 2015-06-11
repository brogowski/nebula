using Nebula;
using Nebula.Connectivity;
using Nebula.Input;
using Nebula.Packets;
using Nebula.Recording;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Nebula
{
    [TestFixture]
    public class NebulaServerTests
    {
        private NebulaServer _server;
        private AbstractSender<IPacket> _sender;
        private AbstractReciver<RecordedInput> _reciver;
        private float _time = 2.5f;
        
        [SetUp]
        public void Setup()
        {
            _sender = Substitute.For<AbstractSender<IPacket>>(Substitute.For<ISendTransmissionProtocol>(), Substitute.For<IPacketSerializer<IPacket>>());
            _reciver = Substitute.For<AbstractReciver<RecordedInput>>(Substitute.For<IReciveTransmissionProtocol>(), Substitute.For<IPacketDeserializer<RecordedInput>>());
            _server = new NebulaServer(_sender, _reciver);
        }

        [Test]
        public void GetInputForDuration_AddsInputFromReciver()
        {
            var expected = new RecordedInput(new RecordedInput.InputData[0], 0.75f);

            _reciver.Recive().Returns(new[] { expected });

            var result = _server.GetInputForDuration(2.0f);

            Check.That(result).ContainsExactly(expected);
        }

        [Test]
        public void AfterAddingGameObject_SpawnPacketIsCreated()
        {
            IPacket packet = null;
            _sender.WhenForAnyArgs(q => q.Send(null))
                .Do(q => packet = q.Arg<IPacket>());

            var stub = Substitute.For<ReadOnlyStatefulGameObject>(Substitute.For<IGameObject>());
            stub.GetPositionDiff().Returns(new Vector3(1,2,3));
            stub.GetRotationDiff().Returns(new Quaternion(1, 2, 3, 4));
            stub.Type.Returns("ABC");
            stub.IsDestroyed.Returns(false);

            _server.AddToRemotePhysics(stub);
            _server.UpdateRemotePhysics(_time);

            Check.That(packet).IsInstanceOf<SpawnPacket>();
            var spawnPacket = (SpawnPacket) packet;
            Check.That(spawnPacket.Position).IsEqualTo(new Vector3(1, 2, 3));
            Check.That(spawnPacket.Rotation).IsEqualTo(new Quaternion(1, 2, 3, 4));
            Check.That(spawnPacket.Type).IsEqualTo("ABC");
        }

        [Test]
        public void AfterAddingAlreadyDestroyedGameObject_NothingIsCreated()
        {
            var stub = Substitute.For<ReadOnlyStatefulGameObject>(Substitute.For<IGameObject>());
            stub.GetPositionDiff().Returns(new Vector3(1, 2, 3));
            stub.GetRotationDiff().Returns(new Quaternion(1, 2, 3, 4));
            stub.Type.Returns("ABC");
            stub.IsDestroyed.Returns(true);

            _server.AddToRemotePhysics(stub);
            _server.UpdateRemotePhysics(_time);

            _sender.DidNotReceiveWithAnyArgs().Send(null);
        }

        [Test]
        public void AfterGameObjectIsAddedAndUpdateWasCalledAtLeastOnceAndGameObjectIsNotDestroyed_MovePacketIsCreated()
        {
            var stub = Substitute.For<ReadOnlyStatefulGameObject>(Substitute.For<IGameObject>());
            stub.GetPositionDiff().Returns(new Vector3(1, 2, 3));
            stub.GetRotationDiff().Returns(new Quaternion(1, 2, 3, 4));
            stub.IsDestroyed.Returns(false);

            _server.AddToRemotePhysics(stub);
            _server.UpdateRemotePhysics(_time);

            IPacket packet = null;
            _sender.WhenForAnyArgs(q => q.Send(null))
                .Do(q => packet = q.Arg<IPacket>());

            _server.UpdateRemotePhysics(_time);

            Check.That(packet).IsInstanceOf<MovePacket>();
            var movePacket = (MovePacket)packet;
            Check.That(movePacket.Move).IsEqualTo(new Vector3(1, 2, 3));
            Check.That(movePacket.Rotation).IsEqualTo(new Quaternion(1, 2, 3, 4));
        }

        [Test]
        public void AfterGameObjectIsAddedAndUpdateWasCalledAtLeastOnceAndGameObjectIsDestroyed_DestroyPacketIsCreated()
        {
            var stub = Substitute.For<ReadOnlyStatefulGameObject>(Substitute.For<IGameObject>());
            stub.GetPositionDiff().Returns(new Vector3(1, 2, 3));
            stub.GetRotationDiff().Returns(new Quaternion(1, 2, 3, 4));
            stub.IsDestroyed.Returns(false);

            _server.AddToRemotePhysics(stub);
            _server.UpdateRemotePhysics(_time);

            IPacket packet = null;
            _sender.WhenForAnyArgs(q => q.Send(null))
                .Do(q => packet = q.Arg<IPacket>());
            stub.IsDestroyed.Returns(true);

            _server.UpdateRemotePhysics(_time);

            Check.That(packet).IsInstanceOf<DestroyPacket>();
        }

        [Test]
        public void AfterGameObjectIsDestroyed_NothingIsCreated()
        {
            var stub = Substitute.For<ReadOnlyStatefulGameObject>(Substitute.For<IGameObject>());
            stub.GetPositionDiff().Returns(new Vector3(1, 2, 3));
            stub.GetRotationDiff().Returns(new Quaternion(1, 2, 3, 4));
            stub.IsDestroyed.Returns(false);

            _server.AddToRemotePhysics(stub);
            _server.UpdateRemotePhysics(_time);

            stub.IsDestroyed.Returns(true);
            _server.UpdateRemotePhysics(_time);

            _sender.ClearReceivedCalls();
            _server.UpdateRemotePhysics(_time);

            _sender.DidNotReceiveWithAnyArgs().Send(null);
        }
    }
}
