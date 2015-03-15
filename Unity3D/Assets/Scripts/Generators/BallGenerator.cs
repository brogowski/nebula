using UnityEngine;

namespace Assets.Scripts.Generators
{
    class BallGenerator
    {
        private readonly GameObject _prefab;
        private readonly Vector3 _minPosition;
        private readonly Vector3 _maxPosition;
        private readonly GameObject _container;

        public BallGenerator(Vector3 minPosition, Vector3 maxPosition,
            GameObject prefab, GameObject container)
        {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
            _prefab = prefab;
            _container = container;
        }

        public void GenerateNewBall()
        {
            var position = GetRandomPosition();
            var ball = Object.Instantiate(_prefab);
            ball.GetComponent<MeshRenderer>().material.color
                = new Color(GetRandom(0, 1), GetRandom(0, 1), GetRandom(0, 1));
            ball.transform.parent = _container.transform;
            ball.transform.position += position;
        }

        private Vector3 GetRandomPosition()
        {
            var x = GetRandom(_minPosition.x, _maxPosition.x);
            var y = GetRandom(_minPosition.y, _maxPosition.y);
            var z = GetRandom(_minPosition.z, _maxPosition.z);
            return new Vector3(x,y,z);
        }

        private float GetRandom(float min, float max)
        {
            return Random.Range(min, max);
        }
    }
}
