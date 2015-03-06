using UnityEngine;

namespace Nebula.Recording
{
    public class SaveOnlyStatefulGameObject
    {
        private readonly IGameObject _gameObject;

        public SaveOnlyStatefulGameObject(IGameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void ApplyPositionDiff(Vector3 vector3)
        {
            _gameObject.Position += vector3;
        }

        public void ApplyRotationDiff(Quaternion quaternion)
        {
            _gameObject.Rotation *= quaternion;
        }
    }
}