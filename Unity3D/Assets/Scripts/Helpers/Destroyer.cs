using Assets.TestScripts;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    class Destroyer : MonoBehaviour
    {

        public void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
            JengaTest.ObjectCount--;
        }
    }
}
