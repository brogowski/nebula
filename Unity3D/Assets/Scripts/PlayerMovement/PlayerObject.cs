using Assets.Scripts.EditorSettings;
using Nebula.PlayerMovement;
using UnityEngine;

namespace Assets.Scripts.PlayerMovement
{
    public class PlayerObject : MonoBehaviour, IPlayerObject
    {
        private PlayerController _playerController;

        void OnEnable()
        {
            _playerController = new PlayerController(25f)
            {
                PlayerObject = this
            };
        }

        void Update()
        {
            _playerController.MoveHorizontally(Input.GetAxis(InputManager.HorizontalAxis));
            _playerController.MoveVertically(Input.GetAxis(InputManager.VerticalAxis));
        }

        public void MoveHorizontally(float value)
        {
            gameObject.rigidbody.velocity = new Vector3(value, gameObject.rigidbody.velocity.y, gameObject.rigidbody.velocity.z);                        
        }
        public void MoveVertically(float value)
        {
            gameObject.rigidbody.velocity = new Vector3(gameObject.rigidbody.velocity.x, gameObject.rigidbody.velocity.y, value);     
        }
    }
}
