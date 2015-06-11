using Assets.Scripts.Generators;
using UnityEngine;

namespace Assets.TestScripts
{
    class JengaTest : MonoBehaviour
    {
        public int Height = 8;
        public int Force = 10000;
        public GameObject CubePrefab;
        public GameObject BallPrefab;
        public Vector3 Position;

        private JengaTowerGenerator _generator = new JengaTowerGenerator();

        public void BuildTower()
        {
            _generator.GenerateTower(Position, Height, CubePrefab);
        }

        void Update()
        {
            if(Input.GetMouseButtonUp(0))
                BuildTower();

        }

    }
}
