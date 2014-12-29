using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.TestScripts
{
    class RecordPhysicsTest : MonoBehaviour
    {
        public GameObject GameObjectContainer;

        private GameObject[] _gameObjects;

        private Vector3[] _startPositions;
        private Vector3[] _lastPositions;
        private List<Vector3>[] _diffsPosition;

        private Quaternion[] _startRotations;
        private Quaternion[] _lastRotations;
        private List<Quaternion>[] _diffsRotation; 

        private bool _recordPhysics = true;        

        void OnEnable()
        {
            _gameObjects = GameObjectContainer.transform.Cast<Transform>().Select(q => q.gameObject).ToArray();
            _startPositions = _gameObjects.Select(q => q.transform.position).ToArray();            
            _lastPositions = _startPositions.ToArray();
            _diffsPosition = _gameObjects.Select(q => new List<Vector3>()).ToArray();
            _startRotations = _gameObjects.Select(q => q.transform.rotation).ToArray();
            _lastRotations = _startRotations.ToArray();
            _diffsRotation = _gameObjects.Select(q => new List<Quaternion>()).ToArray();            
        }

        void Update()
        {
            var time = Time.deltaTime;
            if (_recordPhysics)
            {
                RecordPhysics(time);
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
                gameObject.SetActive(false);
                return;
            }

            for (int index = 0; index < _gameObjects.Length; index++)
            {
                _gameObjects[index].transform.position +=
                    _diffsPosition[index][0];

                _gameObjects[index].transform.rotation *=
                    _diffsRotation[index][0];                             

                _diffsPosition[index].RemoveAt(0);
                _diffsRotation[index].RemoveAt(0);
            }
        }

        private bool NoPhysicsToPlay()
        {
            return _diffsPosition.Any(q => !q.Any());
        }

        private void RecordPhysics(float time)
        {
            for (int index = 0; index < _gameObjects.Length; index++)
            {
                var position = _gameObjects[index].transform.position;
                var diffPos = position - _lastPositions[index];
                _diffsPosition[index].Add(diffPos);
                _lastPositions[index] = position;

                var rotation = _gameObjects[index].transform.rotation;
                var diffRot = Quaternion.Inverse(_lastRotations[index]) * rotation;                
                _diffsRotation[index].Add(diffRot);
                _lastRotations[index] = rotation;
            }
        }

        public void StartReplay()
        {
            _recordPhysics = false;

            for (int i = 0; i < _gameObjects.Length; i++)
            {
                _gameObjects[i].transform.position = _startPositions[i];
                Destroy(_gameObjects[i].rigidbody);
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

        private void Push(GameObject o, Vector3 direction)
        {
            o.rigidbody.AddForce(direction*300);
        }

    }
}
