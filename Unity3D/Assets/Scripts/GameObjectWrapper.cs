using Nebula.Recording;
using UnityEngine;

namespace Assets.Scripts
{
    internal class GameObjectWrapper : IGameObject
    {
        private readonly GameObject _gameObject;

        public GameObjectWrapper(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public Vector3 Position
        {
            get { return _gameObject.transform.position; }
            set { _gameObject.transform.position = value; }
        }

        public Quaternion Rotation
        {
            get { return _gameObject.transform.rotation; }
            set { _gameObject.transform.rotation = value; }
        }

        public bool IsDestroyed
        {
            get { return false; }
        }

        public string Type
        {
            get { return _gameObject.name; }
        }
    }
}