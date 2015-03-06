using System.Linq;
using Assets.Scripts.EditorSettings;
using Nebula.Input;
using Nebula.PlayerMovement;
using UnityEngine;

namespace Assets.Scripts.Input
{
    public class InputPlayer : MonoBehaviour
    {
        public Nebula.Input.InputRecorder Recorder;
        public PlayerController PlayerController;

        void Update()
        {
            var duration = Time.deltaTime;
            var commands = Recorder.GetInputForDuration(duration).ToArray();

            if (commands.Length <= 0)
                return;

            ExecuteCommands(commands);
        }

        private void ExecuteCommands(RecordedInput[] commands)
        {
            foreach (RecordedInput command in commands)
            {
                ExecuteInput(command);
            }
        }

        private void ExecuteInput(RecordedInput input)
        {
            for (int i = 0; i < input.Data.Length; i++)
            {

                switch (input.Data[i].Name)
                {
                    case InputManager.HorizontalAxis:
                        PlayerController.MoveHorizontally(input.Data[i].Value * input.Duration * 100f);
                        break;
                    case InputManager.VerticalAxis:                        
                        PlayerController.MoveForward(input.Data[i].Value * input.Duration * 100f);
                        break;
                    case "Gravity":
                        PlayerController.ApplyGravity(input.Data[i].Value * input.Duration * 100f);
                        break;
                }
            }
        }
    }
}
