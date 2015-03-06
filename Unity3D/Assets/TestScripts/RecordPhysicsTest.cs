using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.Recording;
using UnityEngine;

namespace Assets.TestScripts
{
    class RecordPhysicsTest : MonoBehaviour
    {
        public GameObject GameObjectContainer;

        private ReadWriteGameObject[] _gameObjects;

        private Vector3[] _startPositions;
        private List<Vector3>[] _diffsPosition;

        private Quaternion[] _startRotations;
        private List<Quaternion>[] _diffsRotation; 

        private bool _recordPhysics = true;        

        void OnEnable()
        {
            _gameObjects = GameObjectContainer.transform.Cast<Transform>().Select(q => new ReadWriteGameObject(q.gameObject)).ToArray();

            _startPositions = GameObjectContainer.transform.Cast<Transform>().Select(q => q.transform.position).ToArray();            
            _startRotations = GameObjectContainer.transform.Cast<Transform>().Select(q => q.transform.rotation).ToArray();

            _diffsPosition = _gameObjects.Select(q => new List<Vector3>()).ToArray();
            _diffsRotation = _gameObjects.Select(q => new List<Quaternion>()).ToArray();            
        }

        void Update()
        {
            if (_recordPhysics)
            {
                RecordPhysics();
            }
            else
            {
                PlayPhysics();
            }
        }

        private void PlayPhysics()
        {
            if (NoPhysicsToPlay())
            {
                this.gameObject.SetActive(false);
                return;
            }

            for (int index = 0; index < _gameObjects.Length; index++)
            {
                _gameObjects[index].ApplyPositionDiff(_diffsPosition[index][0]);
                _gameObjects[index].ApplyRotationDiff(_diffsRotation[index][0]);                         

                _diffsPosition[index].RemoveAt(0);
                _diffsRotation[index].RemoveAt(0);
            }
        }

        private bool NoPhysicsToPlay()
        {
            return _diffsPosition.Any(q => !q.Any());
        }

        private void RecordPhysics()
        {
            for (int index = 0; index < _gameObjects.Length; index++)
            {
                var diffPos = _gameObjects[index].GetPositionDiff();
                _diffsPosition[index].Add(diffPos);

                var diffRot = _gameObjects[index].GetRotationDiff();           
                _diffsRotation[index].Add(diffRot);
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
