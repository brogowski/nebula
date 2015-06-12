using System.Collections.Generic;
using Assets.Scripts.Generators;
using Nebula.Input;
using UnityEngine;

namespace Assets.TestScripts
{
    class NebulaTestGameLogic : MonoBehaviour
    {
        public NebulaServerTests NebulaServer;
        public GameObject Prefab;
        public int TowerHeight = 100;
        public Vector3 SpawnPoint;

        private JengaTowerGenerator _generator = new JengaTowerGenerator();

        private void CreateNewTower()
        {
            var newObjects = _generator.GenerateTower(SpawnPoint, TowerHeight, Prefab);
            foreach (var jengaBlock in newObjects)
            {
                var newGameObject = (GameObject)Instantiate(jengaBlock.Prefab, jengaBlock.Position, jengaBlock.Rotation);
                NebulaServer.AddNewGameObject(newGameObject);
            }
        }

        public void ExecuteInputs(IEnumerable<RecordedInput> inputs)
        {
            foreach (var recordedInput in inputs)
            {
                foreach (var inputData in recordedInput.Data)
                {
                    if (inputData.Name == "Generate" && inputData.Value == 1.00f)
                    {
                        CreateNewTower();
                    }
                }
            }
        }
    }
}