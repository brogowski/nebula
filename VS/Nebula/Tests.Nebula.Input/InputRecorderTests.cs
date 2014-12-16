using Nebula.Input;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Input
{
    public class InputRecorderTests
    {
        private InputRecorder _recorder;

        [SetUp]
        public void SetUp()
        {
            _recorder = new InputRecorder();
        }

        [Test]
        public void CanGetInputAfterRecording()
        {
            var recordedInput = new RecordedInput(new []{new RecordedInput.InputData("Horizontal", 1.0f)}, 0.01f);
            _recorder.RecordInput(recordedInput);

            Check.That(_recorder.GetInputForDuration(0.01f)).ContainsExactly(recordedInput);
        }
    }
}
