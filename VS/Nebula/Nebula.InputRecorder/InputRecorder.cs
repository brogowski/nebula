using System.Collections.Generic;
using System.Linq;
using Nebula.TimedList;

namespace Nebula.InputRecorder
{
    public class InputRecorder
    {
        private readonly TimedList<InputData> _timedList = new TimedList<InputData>();

        public void RecordInput(RecordedInput recorded)
        {
            _timedList.Add(new InputData(recorded.Name, recorded.Value), recorded.Duration);
        }

        public IEnumerable<RecordedInput> GetInputForDuration(float duration)
        {
            return _timedList.Take(duration).Select(q => new RecordedInput(q.Value.Name, q.Value.Value, q.Duration));
        }

        private struct InputData
        {
            public InputData(string name, float value)
            {
                Value = value;
                Name = name;
            }

            public readonly float Value;
            public readonly string Name;
        }
    }
}