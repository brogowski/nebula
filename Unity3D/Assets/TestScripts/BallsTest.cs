using Assets.Scripts.Generators;
using UnityEngine;

namespace Assets.TestScripts
{
    class BallsTest : MonoBehaviour
    {
        public GameObject BallPrefab;
        public GameObject BallContainer;
        public Vector3 MinPosition;
        public Vector3 MaxPosition;

        private BallGenerator _ballGenerator;

        void OnEnable()
        {
            _ballGenerator = new BallGenerator(MinPosition, MaxPosition,
                BallPrefab, BallContainer);
        }

        public void AddNewBall()
        {
            _ballGenerator.GenerateNewBall();
        }
    }
}
