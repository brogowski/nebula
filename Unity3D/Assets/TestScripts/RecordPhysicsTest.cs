using System;
using System.Linq;
using Assets.Scripts.Recording;
using UnityEngine;

namespace Assets.TestScripts
{
    class RecordPhysicsTest : MonoBehaviour
    {
        public GameObject GameObjectContainer;

        private PhysicsRecorder _physicsRecorder;
        private bool _recordPhysics = true;
        private Vector3[] _startPositions;
        private Quaternion[] _startRotations;
        private GameObject[] _gameObjects;

        void OnEnable()
        {
            _physicsRecorder = new PhysicsRecorder();

            _gameObjects = GameObjectContainer.transform.Cast<Transform>()
                .Where(q => q.gameObject.activeSelf).Select(q => q.gameObject).ToArray();

            foreach (var obj in _gameObjects)
            {
                _physicsRecorder.AddGameObjectToRecording(obj);
            }

            _startPositions = GameObjectContainer.transform.Cast<Transform>().Select(q => q.transform.position).ToArray();            
            _startRotations = GameObjectContainer.transform.Cast<Transform>().Select(q => q.transform.rotation).ToArray();       
        }

        void OnDestroy()
        {
            _physicsRecorder.Dispose();
        }


        void Update()
        {
            if (!_recordPhysics)
            {
                PlayPhysics(Time.deltaTime);
            }
        }

        void FixedUpdate()
        {
            if (_recordPhysics)
            {
                RecordPhysics(Time.deltaTime);
            }
        }

        private void PlayPhysics(float time)
        {
            _physicsRecorder.PlayPhysics(time);
        }

        private void RecordPhysics(float time)
        {
            _physicsRecorder.RecordPhysics(time);
        }

        public void StartReplay()
        {
            _recordPhysics = false;

            for (int i = 0; i < _gameObjects.Length; i++)
            {
                _gameObjects[i].transform.position = _startPositions[i];
                _gameObjects[i].transform.rotation = _startRotations[i];
                Destroy(_gameObjects[i].transform.GetComponent<Rigidbody>());
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
            o.GetComponent<Rigidbody>().AddForce(direction*300);
        }
    }
}
