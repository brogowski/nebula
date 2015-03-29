using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TestScripts;
using Nebula.TimedList;
using UnityEngine;

namespace Assets.Scripts.Recording
{
    class PhysicsRecorder : IDisposable
    {
        protected readonly List<ReadWriteGameObject> GameObjects
            = new List<ReadWriteGameObject>();

        private readonly List<TimedList<Vector3>> _diffsPosition
            = new List<TimedList<Vector3>>();

        private readonly List<TimedList<Quaternion>> _diffsRotation
            = new List<TimedList<Quaternion>>();

        public void AddGameObjectToRecording(GameObject gameObject)
        {
            GameObjects.Add(new ReadWriteGameObject(gameObject));
            _diffsPosition.Add(GetNewVectorList());
            _diffsRotation.Add(GetNewQuaternionList());           
        }

        public virtual void PlayPhysics(float time)
        {
            if (NoPhysicsToPlay())
            {
                return;
            }

            for (int index = 0; index < GameObjects.Count; index++)
            {
                ApplyPositionDiffs(time, index);
                ApplyRotationDiffs(time, index);
            }
        }

        public virtual void RecordPhysics(float time)
        {
            for (int index = 0; index < GameObjects.Count; index++)
            {
                _diffsPosition[index].Add(GameObjects[index].GetPositionDiff(), time);

                _diffsRotation[index].Add(GameObjects[index].GetRotationDiff(), time);
            }
        }

        private TimedList<Vector3> GetNewVectorList()
        {
            return new TimedList<Vector3>((vector3, value) =>
                new[] { vector3 * value, vector3 * (1 - value) });
        }

        private TimedList<Quaternion> GetNewQuaternionList()
        {
            return new TimedList<Quaternion>((originalQuaternion, value) =>
            {
                var firstQuaternion = Quaternion.Lerp(new Quaternion(), originalQuaternion, value);
                var secondQuaternion = originalQuaternion * Quaternion.Inverse(firstQuaternion);
                return new[] { firstQuaternion, secondQuaternion };
            });
        }

        private void ApplyRotationDiffs(float time, int index)
        {
            var rotations = _diffsRotation[index].Take(time).ToArray();
            Array.ForEach(rotations, rotation =>
                GameObjects[index].ApplyRotationDiff(rotation.Value));
        }

        private void ApplyPositionDiffs(float time, int index)
        {
            var positions = _diffsPosition[index].Take(time).ToArray();
            Array.ForEach(positions, position =>
                GameObjects[index].ApplyPositionDiff(position.Value));
        }

        private bool NoPhysicsToPlay()
        {
            return _diffsPosition.Any(q => q.Count <= 0);
        }

        public virtual void Dispose()
        {
            
        }
    }
}
