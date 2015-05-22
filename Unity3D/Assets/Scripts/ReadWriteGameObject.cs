using Nebula.Recording;
using UnityEngine;

namespace Assets.Scripts
{
    class ReadWriteGameObject
    {
        private readonly ReadOnlyStatefulGameObject _readGameObject;
        private readonly SaveOnlyStatefulGameObject _saveGameObject;

        public ReadWriteGameObject(GameObject gameObject)
        {
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