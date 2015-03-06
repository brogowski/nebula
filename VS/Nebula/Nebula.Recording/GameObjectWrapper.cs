using UnityEngine;

namespace Nebula.Recording
{
    public class GameObjectWrapper : IGameObject
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
    }
}
