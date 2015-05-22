using System.Collections.Generic;
using Nebula.Input;
using UnityEngine;

namespace Assets.TestScripts
{
    class NebulaTestGameLogic : MonoBehaviour
    {
        public NebulaServerTests NebulaServer;
        public GameObject[] Prefabs;
        public Vector3 SpawnPoint;
        public Quaternion SpawnRotation;

        public void CreateNewObject()
        {
            var newGameObject = (GameObject) Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], SpawnPoint, SpawnRotation);
            NebulaServer.AddNewGameObject(newGameObject);
        }

        public void ExecuteInputs(IEnumerable<RecordedInput> inputs)
        {
            throw new System.NotImplementedException();
        }
    }
}