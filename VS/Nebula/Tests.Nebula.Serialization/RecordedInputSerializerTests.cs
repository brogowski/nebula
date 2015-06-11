using Nebula.Input;
using Nebula.Serialization;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class RecordedInputSerializerTests
    {
        private RecordedInputSerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = new RecordedInputSerializer();
        }

        [Test]
        public void CanSerializeEmptyRecordedInput()
        {
            var sample = new RecordedInput();

            var serializedPacket = _serializer.Serialize(sample);

            Check.That(serializedPacket).IsEqualTo("RecordedInput(0)");
        }

        [Test]
        public void CanSerializeNonZeroDurationRecorderInput()
        {
            var sample = new RecordedInput(new RecordedInput.InputData[0], .75f);

            var serializedPacket = _serializer.Serialize(sample);

            Check.That(serializedPacket).IsEqualTo("RecordedInput(0.75)");
        }

        [Test]
        public void CanSerializeRecorderInputWithNotEmptyInputData()
        {
            var sample = new RecordedInput(new []
            {
                new RecordedInput.InputData("Name", .77743f), 
                new RecordedInput.InputData("Name", .77743f), 
            }, .75f);

            var serializedPacket = _serializer.Serialize(sample);

            Check.That(serializedPacket).IsEqualTo(@"RecordedInput(0.75)
Name:0.77742999792099
Name:0.77742999792099");
        }

        [Test]
        public void CanDeserializeEmptyRecordedInput()
        {
            var deserializedPacket = _serializer.Deserialize("RecordedInput(0)");

            Check.That(deserializedPacket.Data).IsEmpty();
            Check.That(deserializedPacket.Duration).IsEqualTo(0);
        }

        [Test]
        public void CanDeserializeNonZeroDurationRecorderInput()
        {
            var expected = new RecordedInput(new RecordedInput.InputData[0], .75f);

            var deserializedPacket = _serializer.Deserialize("RecordedInput(0.75)");

            Check.That(deserializedPacket.Duration).IsEqualTo(expected.Duration);
            Check.That(deserializedPacket.Data).ContainsExactly(expected.Data);
        }

        [Test]
        public void CanDeserializeRecorderInputWithNotEmptyInputData()
        {
            var sample = new RecordedInput(new[]
            {
                new RecordedInput.InputData("Name", .77743f), 
                new RecordedInput.InputData("Name", .77743f), 
            }, .75f);

            var deserializedPacket = _serializer.Deserialize(@"RecordedInput(0.75)
Name:0.77742999792099
Name:0.77742999792099");

            Check.That(deserializedPacket.Data).ContainsExactly(sample.Data);
        }

    }
}
