using UnityEngine;

namespace Nebula.Recording
{
    public class ReadOnlyStatefulGameObject
    {
        private readonly IGameObject _gameObject;
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        public ReadOnlyStatefulGameObject(IGameObject gameObject)
        {
            _gameObject = gameObject;
            _lastPosition = gameObject.Position;
            _lastRotation = gameObject.Rotation;
        }

        public virtual bool IsDestroyed
        {
            get { return _gameObject.IsDestroyed; }
        }

        public virtual string Type
        {
            get { return _gameObject.Type; }
        }

        public virtual Vector3 GetPositionDiff()
        {
            var diff = _gameObject.Position - _lastPosition;
            _lastPosition = _gameObject.Position;
            return diff;
        }

        public virtual Quaternion GetRotationDiff()
        {
            var diff = Quaternion.Inverse(_lastRotation)*_gameObject.Rotation;
            _lastRotation = _gameObject.Rotation;
            return diff;
        }

        public Vector3 GetCurrentPosition()
        {
            return _gameObject.Position;
        }

        public Quaternion GetCurrentRotation()
        {
            return _gameObject.Rotation;
        }
    }
}