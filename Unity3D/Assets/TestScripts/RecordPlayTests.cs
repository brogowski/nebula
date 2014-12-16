using Assets.Scripts.Input;
using Assets.Scripts.PlayerMovement;
using Nebula.PlayerMovement;
using UnityEngine;

namespace Assets.TestScripts
{
    public class RecordPlayTests : MonoBehaviour
    {
        public InputPlayer InputPlayer;
        public InputRecorder InputRecorder;
        public PlayerObject PlayerObject;
        private PlayerController _playerController;

        void OnEnable()
        {
            _playerController = new PlayerController
            {
                PlayerObject = PlayerObject,
                Speed = 1f
            };
            InputPlayer.PlayerController = _playerController;
            InputPlayer.Recorder = InputRecorder.Recorder;
        }

        public void StartReplay()
        {
            Debug.Log("Start play.");            
            InputRecorder.gameObject.SetActive(false);
            ResetPlayerPosition();     
            InputPlayer.gameObject.SetActive(true);
        }

        public void StartRecording()
        {
            Debug.Log("Start rec.");
            InputPlayer.gameObject.SetActive(false);
            ResetPlayerPosition();
            InputRecorder.gameObject.SetActive(true);
        }

        private void ResetPlayerPosition()
        {
            PlayerObject.transform.position = new Vector3(0, 2, 0);
        }
    }
}
