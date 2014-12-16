using Assets.Scripts.EditorSettings;
using Nebula.PlayerMovement;
using UnityEngine;

namespace Assets.Scripts.PlayerMovement
{
    public class PlayerObject : MonoBehaviour, IPlayerObject
    {
        private PlayerController _playerController;
        private CharacterController _characterController;
        private const float _gravity = 9.81f;

        void OnEnable()
        {
            _playerController = new PlayerController
            {
                PlayerObject = this,                
                Speed = 1f
            };
            _characterController = GetComponent<CharacterController>();            
        }

        void Update()
        {
            _playerController.MoveHorizontally(UnityEngine.Input.GetAxis(InputManager.HorizontalAxis) * Time.deltaTime * 100f);
            _playerController.MoveForward(UnityEngine.Input.GetAxis(InputManager.VerticalAxis) * Time.deltaTime * 100f);
            _playerController.ApplyGravity(_gravity * Time.deltaTime);            
        }        

        public void MoveHorizontally(float value)
        {
            _characterController.Move(Vector3.right * value);
        }

        public void MoveVertically(float value)
        {
            _characterController.Move(Vector3.up * value);
        }

        public void MoveForward(float value)
        {
            _characterController.Move(Vector3.forward * value);
        }
    }
}
