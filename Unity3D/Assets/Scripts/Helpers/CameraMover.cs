﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Helpers
{            
    class CameraMover : MonoBehaviour
    {
        public float LookSensitivity = .25f;
        public float LookSmoothDamp = 0.1f;
        public float MoveSpeed = 1.5f;

        private float _yRotation;
        private float _xrotation;
        private float _currentyrotation;
        private float _currentXRotation;
        private float _yRotationV;
        private float _xRotationV;

        private float _moveX;
        private float _moveY;
        private float _moveZ;

        void Update()
        {
            _moveX = GetAxis("Horizontal") * MoveSpeed;
            _moveY = GetAxis("Height") * MoveSpeed;
            _moveZ = GetAxis("Vertical") * MoveSpeed;

            _yRotation += CrossPlatformInputManager.GetAxis("Mouse X") * LookSensitivity;
            _xrotation -= CrossPlatformInputManager.GetAxis("Mouse Y") * LookSensitivity;

            _xrotation = Mathf.Clamp(_xrotation, -90, 90);
            _currentXRotation = Mathf.SmoothDamp(_currentXRotation, _xrotation, ref _xRotationV, LookSmoothDamp);
            _currentyrotation = Mathf.SmoothDamp(_currentyrotation, _yRotation, ref _yRotationV, LookSmoothDamp);

            if (Camera.main != null)
            {
                Camera.main.transform.Translate(_moveX, _moveY, _moveZ);
                Camera.main.transform.rotation = Quaternion.Euler(_xrotation, _yRotation, 0);
            }

        }

        private static float GetAxis(string name)
        {
            if (Time.timeScale == 0)
                return UnityEngine.Input.GetAxisRaw(name);

            return CrossPlatformInputManager.GetAxis(name);
        }
    }
}
