using Assets.Scripts.Generators;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.TestScripts
{
    public class JengaTest : MonoBehaviour
    {
        public static int ObjectCount = 0;

        public int Height = 8;
        public int Force = 10000;
        public GameObject CubePrefab;
        public GameObject BallPrefab;
        public Vector3 Position;
        public Text ObjectCountText;

        private JengaTowerGenerator _generator = new JengaTowerGenerator();

        public void BuildTower()
        {
            var blocks = _generator.GenerateTower(Position, Height, CubePrefab);
            foreach (var block in blocks)
            {
                Instantiate(block.Prefab, block.Position, block.Rotation);
                ObjectCount++;
            }
        }

        void Update()
        {
            if (CrossPlatformInputManager.GetButtonUp("Generate"))
                BuildTower();
            if (CrossPlatformInputManager.GetButtonUp("Pause"))
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;

            ObjectCountText.text = ObjectCount.ToString();
        }

    }
}
