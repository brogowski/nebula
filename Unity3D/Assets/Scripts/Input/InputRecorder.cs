using Assets.Scripts.EditorSettings;
using Nebula.Input;
using UnityEngine;

namespace Assets.Scripts.Input
{
    public class InputRecorder : MonoBehaviour
    {
        public readonly Nebula.Input.InputRecorder Recorder
            = new Nebula.Input.InputRecorder();

        void Update()
        {
            var duration = Time.deltaTime;
            var horizontalValue = UnityEngine.Input.GetAxis(InputManager.HorizontalAxis);
            var verticalValue = UnityEngine.Input.GetAxis(InputManager.VerticalAxis);

            var inputData = new[]
            {
                GetInputData(InputManager.HorizontalAxis, horizontalValue),
                GetInputData(InputManager.VerticalAxis, verticalValue),
            };

            Record(inputData, duration);
        }

        private void Record(RecordedInput.InputData[] inputData, float duration)
        {
            Recorder.RecordInput(new RecordedInput(inputData, duration));
        }

        private RecordedInput.InputData GetInputData(string inputName, float value)
        {
            return new RecordedInput.InputData(inputName, value);
        }
    }
}
