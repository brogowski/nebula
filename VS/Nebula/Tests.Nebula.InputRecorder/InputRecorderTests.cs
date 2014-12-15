using Nebula.TimedList;
using NFluent;
using NUnit.Framework;
using Nebula.InputRecorder;

namespace Tests.Nebula.InputRecorder
{
    public class InputRecorderTests
    {
        private global::Nebula.InputRecorder.InputRecorder _recorder;

        [SetUp]
        public void SetUp()
        {
            _recorder = new global::Nebula.InputRecorder.InputRecorder();
        }

        [Test]
        public void CanGetInputAfterRecording()
        {
            var recordedInput = new RecordedInput("Horizontal", 1.0f, 0.01f);
            _recorder.RecordInput(recordedInput);

            Check.That(_recorder.GetInputForDuration(0.01f)).ContainsExactly(recordedInput);
        }
    }
}
