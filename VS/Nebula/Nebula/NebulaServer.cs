using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.Connectivity;
using Nebula.Input;
using Nebula.Packets;
using Nebula.Recording;

namespace Nebula
{
    public class NebulaServer
    {
        private readonly ICollection<ObservableGameObject> _observables = new List<ObservableGameObject>();
        private readonly InputRecorder _inputRecorder = new InputRecorder();
        private readonly AbstractSender<IPacket> _packetSender;
        private readonly AbstractReciver<RecordedInput> _inputReciver;

        public NebulaServer(AbstractSender<IPacket> packetSender, AbstractReciver<RecordedInput> inputReciver)
        {
            _packetSender = packetSender;
            _inputReciver = inputReciver;
        }

        public void AddToRemotePhysics(ReadOnlyStatefulGameObject gameObject)
        {
            if(!gameObject.IsDestroyed)
                _observables.Add(new ObservableGameObject(gameObject));
        }

        public void UpdateRemotePhysics(float time)
        {
            var packets = BuildPackets(time);
            ChangeNewObjectsToOld();
            RemoveDestroyedObjects();
            SendPackets(packets);
        }

        public IEnumerable<RecordedInput> GetInputForDuration(float duration)
        {
            foreach (var recordedInput in _inputReciver.Recive())
            {                
                _inputRecorder.RecordInput(recordedInput);
            }
            return _inputRecorder.GetInputForDuration(duration);
        }

        private void RemoveDestroyedObjects()
        {
            var toDestroy = _observables.Where(q => q.GameObject.IsDestroyed).ToArray();
            foreach (var toDestroyObject in toDestroy)
            {
                _observables.Remove(toDestroyObject);
            }
        }

        private void SendPackets(IEnumerable<IPacket> packets)
        {
            foreach (var packet in packets)
            {
                _packetSender.Send(packet);
            }
        }

        private void ChangeNewObjectsToOld()
        {
            foreach (var gameObject in _observables)
            {
                gameObject.IsNew = false;
            }
        }

        private IEnumerable<IPacket> BuildPackets(float time)
        {
            var packets = new List<IPacket>();
            foreach (var observableGameObject in _observables)
            {
                if (observableGameObject.IsNew)
                {
                    packets.Add(BuildSpawnPacket(observableGameObject));
                }
                else if (observableGameObject.GameObject.IsDestroyed)
                {
                    packets.Add(BuildDestroyPacket(observableGameObject));
                }
                else
                {
                    packets.Add(BuildMovePacket(observableGameObject, time));
                }
            }
            return packets;
        }

        private IPacket BuildMovePacket(ObservableGameObject observableGameObject, float time)
        {
            return new MovePacket(observableGameObject.Id,
                observableGameObject.GameObject.GetPositionDiff(),
                observableGameObject.GameObject.GetRotationDiff(),
                time);
        }

        private IPacket BuildDestroyPacket(ObservableGameObject observableGameObject)
        {
            return new DestroyPacket(observableGameObject.Id);
        }

        private IPacket BuildSpawnPacket(ObservableGameObject observableGameObject)
        {
            return new SpawnPacket(observableGameObject.Id,
                observableGameObject.GameObject.GetCurrentPosition(),
                observableGameObject.GameObject.GetCurrentRotation(),
                observableGameObject.GameObject.Type);
        }

        private class ObservableGameObject
        {
            private readonly ReadOnlyStatefulGameObject _gameObject;

            public ObservableGameObject(ReadOnlyStatefulGameObject gameObject)
            {
                _gameObject = gameObject;
                IsNew = true;
                Id = Guid.NewGuid();
            }

            public Guid Id { get; private set; }

            public ReadOnlyStatefulGameObject GameObject
            {
                get { return _gameObject; }
            }

            public bool IsNew { get; set; }
        }
    }
}