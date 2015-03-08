using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.Recording;
using Nebula.TimedList;
using UnityEngine;

namespace Assets.TestScripts
{
    class RecordPhysicsTest : MonoBehaviour
    {
        public GameObject GameObjectContainer;

        private ReadWriteGameObject[] _gameObjects;

        private Vector3[] _startPositions;
        private TimedList<Vector3>[] _diffsPosition;

        private Quaternion[] _startRotations;
        private TimedList<Quaternion>[] _diffsRotation; 

        private bool _recordPhysics = true;        

        void OnEnable()
        {                        
            _gameObjects = GameObjectContainer.transform.Cast<Transform>().Where(q => q.gameObject.activeSelf).Select(q => new ReadWriteGameObject(q.gameObject)).ToArray();

            _startPositions = GameObjectContainer.transform.Cast<Transform>().Select(q => q.transform.position).ToArray();            
            _startRotations = GameObjectContainer.transform.Cast<Transform>().Select(q => q.transform.rotation).ToArray();

            _diffsPosition = _gameObjects.Select(q => GetNewVectorList()).ToArray();
            _diffsRotation = _gameObjects.Select(q => GetNewQuaternionList()).ToArray();            
        }

        private TimedList<Vector3> GetNewVectorList()
        {
            return new TimedList<Vector3>((vector3, value) =>
            {
                return new[] {vector3*value, vector3*(1 - value)};
            });
        }

        private TimedList<Quaternion> GetNewQuaternionList()
        {
            return new TimedList<Quaternion>((originalQuaternion, value) =>
            {                            
                var firstQuaternion = Quaternion.Lerp(new Quaternion(), originalQuaternion, value);
                var secondQuaternion = originalQuaternion * Quaternion.Inverse(firstQuaternion);
                return new[] {firstQuaternion, secondQuaternion};
            });
        }

        void Update()
        {
            var deltaTime = Time.deltaTime;
            if (_recordPhysics)
            {
                RecordPhysics(deltaTime);
            }
            else
            {
                PlayPhysics(deltaTime);
            }
        }

        private void PlayPhysics(float time)
        {
            if (NoPhysicsToPlay())
            {
                this.gameObject.SetActive(false);
                return;
            }

            for (int index = 0; index < _gameObjects.Length; index++)
            {
                ApplyPositionDiffs(time, index);
                ApplyRotationDiffs(time, index);
            }
        }

        private void ApplyRotationDiffs(float time, int index)
        {
            _diffsRotation[index].Take(time).Select(rotation =>
            {
                _gameObjects[index].ApplyRotationDiff(rotation.Value);
                return true;
            }).ToArray();
            
        }

        private void ApplyPositionDiffs(float time, int index)
        {
            _diffsPosition[index].Take(time).Select(position =>
            {
                _gameObjects[index].ApplyPositionDiff(position.Value);
                return true;
            }).ToArray();
        }

        private bool NoPhysicsToPlay()
        {
            return _diffsPosition.Any(q => q.Count <= 0);
        }

        private void RecordPhysics(float time)
        {
            for (int index = 0; index < _gameObjects.Length; index++)
            {
                _diffsPosition[index].Add(_gameObjects[index].GetPositionDiff(), time);

                _diffsRotation[index].Add(_gameObjects[index].GetRotationDiff(), time);
            }
        }

        public void StartReplay()
        {
            _recordPhysics = false;

            for (int i = 0; i < _gameObjects.Length; i++)
            {
                _gameObjects[i].GameObject.transform.position = _startPositions[i];
                _gameObjects[i].GameObject.transform.rotation = _startRotations[i];
                Destroy(_gameObjects[i].GameObject.transform.GetComponent<Rigidbody>());
            }
        }

        public void PushLeft()
        {
            Array.ForEach(_gameObjects, q => Push(q, Vector3.left));
        }

        public void PushRight()
        {
            Array.ForEach(_gameObjects, q => Push(q, Vector3.right));
        }

        public void PushForward()
        {
            Array.ForEach(_gameObjects, q => Push(q, Vector3.forward));
        }

        public void PushBack()
        {
            Array.ForEach(_gameObjects, q => Push(q, Vector3.back));
        }

        public void PushUp()
        {
            Array.ForEach(_gameObjects, q => Push(q, Vector3.up));
        }

        private void Push(ReadWriteGameObject o, Vector3 direction)
        {
            o.GameObject.GetComponent<Rigidbody>().AddForce(direction*300);
        }

        internal class ReadWriteGameObject
        {
            internal GameObject GameObject;

            private readonly ReadOnlyStatefulGameObject _readGameObject;
            private readonly SaveOnlyStatefulGameObject _saveGameObject;

            public ReadWriteGameObject(GameObject gameObject)
            {
                GameObject = gameObject;
                var monoBehaviourWrapper = new GameObjectWrapper(gameObject);
                _readGameObject = new ReadOnlyStatefulGameObject(monoBehaviourWrapper);
                _saveGameObject = new SaveOnlyStatefulGameObject(monoBehaviourWrapper);
            }

            public void ApplyPositionDiff(Vector3 vector3)
            {
                _saveGameObject.ApplyPositionDiff(vector3);
            }

            public void ApplyRotationDiff(Quaternion quaternion)
            {
                _saveGameObject.ApplyRotationDiff(quaternion);
            }

            public Vector3 GetPositionDiff()
            {
                return _readGameObject.GetPositionDiff();
            }

            public Quaternion GetRotationDiff()
            {
                return _readGameObject.GetRotationDiff();
            }
        }
    }


}
