using System;
using System.Collections.Generic;
using Nebula.Packets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    class GameObjectDirector
    {
        private readonly Dictionary<Guid, PhysicsObject> _gameObjects = new Dictionary<Guid, PhysicsObject>();
        private readonly Dictionary<string, GameObject> _prefabs;

        public GameObjectDirector(Dictionary<string, GameObject> prefabs)
        {
            _prefabs = new Dictionary<string, GameObject>(prefabs);
        }

        public void MoveObjects(float deltaTime)
        {
            foreach (var physicsObject in _gameObjects.Values)
            {
                var moves = physicsObject.Moves.Take(deltaTime);
                foreach (var move in moves)
                {                    
                    physicsObject.SaveOnlyStatefulGameObject.ApplyPositionDiff(move.Value);
                }
                var rotations = physicsObject.Rotations.Take(deltaTime);
                foreach (var rotation in rotations)
                {
                    physicsObject.SaveOnlyStatefulGameObject.ApplyRotationDiff(rotation.Value);
                }
            }
        }

        public void AddNewMovement(MovePacket packet)
        {
            var physicsObject = _gameObjects[packet.Id];
            physicsObject.Moves.Add(packet.Move, packet.Duration);
            physicsObject.Rotations.Add(packet.Rotation, packet.Duration);
        }

        public void CreateNewGameObject(SpawnPacket packet)
        {
            var prefab = _prefabs[packet.Type];
            var newGameObject = (GameObject)Object.Instantiate(prefab, packet.Position, packet.Rotation);
            Object.Destroy(newGameObject.GetComponent<Rigidbody>());
            _gameObjects.Add(packet.Id, new PhysicsObject(newGameObject, packet.Type));
        }

        public void DestroyObject(DestroyPacket packet)
        {
            var physicsObject = _gameObjects[packet.Id];
            Object.Destroy(physicsObject.GameObject);
            _gameObjects.Remove(packet.Id);
        }
    }
}