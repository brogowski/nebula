using Nebula.Recording;
using Nebula.TimedList;
using UnityEngine;

namespace Assets.Scripts
{
    class PhysicsObject
    {
        public PhysicsObject(GameObject gameObject, string type)
        {
            GameObject = gameObject;
            SaveOnlyStatefulGameObject = new SaveOnlyStatefulGameObject(new GameObjectWrapper(gameObject, type));
            Moves = GetNewVectorList();
            Rotations = GetNewQuaternionList();
        }

        public GameObject GameObject { get; private set; }
        public SaveOnlyStatefulGameObject SaveOnlyStatefulGameObject { get; private set; }
        public TimedList<Vector3> Moves { get; private set; }
        public TimedList<Quaternion> Rotations { get; private set; }

        private TimedList<Vector3> GetNewVectorList()
        {
            return new TimedList<Vector3>((vector3, value) =>
                new[] { vector3 * value, vector3 * (1 - value) });
        }
        private TimedList<Quaternion> GetNewQuaternionList()
        {
            return new TimedList<Quaternion>((originalQuaternion, value) =>
            {
                var firstQuaternion = Quaternion.Lerp(new Quaternion(), originalQuaternion, value);
                var secondQuaternion = originalQuaternion * Quaternion.Inverse(firstQuaternion);
                return new[] { firstQuaternion, secondQuaternion };
            });
        }
    }
}