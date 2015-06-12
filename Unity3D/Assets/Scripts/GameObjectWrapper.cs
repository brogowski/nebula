using Nebula.Recording;
using UnityEngine;

namespace Assets.Scripts
{
    internal class GameObjectWrapper : IGameObject
    {
        private readonly GameObject _gameObject;
        private readonly string _type;

        public GameObjectWrapper(GameObject gameObject, string type)
        {
            _gameObject = gameObject;
            _type = type;
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
            get { return _gameObject == null; }
        }

        public string Type
        {
            get { return _type; }
        }
    }
}