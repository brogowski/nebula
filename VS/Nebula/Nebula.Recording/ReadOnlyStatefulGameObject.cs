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

        public Vector3 GetPositionDiff()
        {
            var diff = _gameObject.Position - _lastPosition;
            _lastPosition = _gameObject.Position;
            return diff;
        }

        public Quaternion GetRotationDiff()
        {
            var diff = Quaternion.Inverse(_lastRotation)*_gameObject.Rotation;
            _lastRotation = _gameObject.Rotation;
            return diff;
        }
    }
}