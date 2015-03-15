using System;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class PlaneController : MonoBehaviour
    {
        public GameObject Plane;
        public float MaxAngle;
        public float Speed;

        public PlaneController(GameObject plane, float maxAngle, float speed)
        {
            Plane = plane;
            MaxAngle = maxAngle;
            Speed = speed;
        }

        public void RotateLeft()
        {
            if (GetSignedAngleBetweenVectors(Vector3.up, Plane.transform.up, -Plane.transform.forward) < MaxAngle)
                Plane.transform.Rotate(Plane.transform.forward, Speed * Time.deltaTime);
        }

        public void RotateRight()
        {
            if (GetSignedAngleBetweenVectors(Vector3.up, Plane.transform.up, -Plane.transform.forward) > -MaxAngle)
                Plane.transform.Rotate(-Plane.transform.forward, Speed * Time.deltaTime);
        }

        public void RotateUp()
        {
            if (GetSignedAngleBetweenVectors(Vector3.forward, Plane.transform.forward, Plane.transform.right) > -MaxAngle)
                Plane.transform.Rotate(Plane.transform.right, Speed * Time.deltaTime);
        }

        public void RotateDown()
        {
            if (GetSignedAngleBetweenVectors(Vector3.forward, Plane.transform.forward, Plane.transform.right) < MaxAngle)
                Plane.transform.Rotate(-Plane.transform.right, Speed * Time.deltaTime);
        }

        private double GetSignedAngleBetweenVectors(Vector3 a, Vector3 b, Vector3 normal)
        {
            var angle = Math.Acos(Vector3.Dot(a.normalized, b.normalized));
            var cross = Vector3.Cross(a, b);
            if (Vector3.Dot(normal, cross) > 0)
            {
                angle = -angle;
            }
            angle *= (180.0 / Math.PI);
            return angle;
        }

        void Update()
        {
            if(UnityEngine.Input.GetAxis(EditorSettings.InputManager.HorizontalAxis) < 0)
                RotateLeft();
            if (UnityEngine.Input.GetAxis(EditorSettings.InputManager.HorizontalAxis) > 0)
                RotateRight();
            if (UnityEngine.Input.GetAxis(EditorSettings.InputManager.VerticalAxis) < 0)
                RotateDown();
            if (UnityEngine.Input.GetAxis(EditorSettings.InputManager.VerticalAxis) > 0)
                RotateUp();
        }
    }
}
