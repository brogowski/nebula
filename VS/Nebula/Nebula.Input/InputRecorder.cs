using System.Collections.Generic;
using System.Linq;
using Nebula.TimedList;

namespace Nebula.Input
{
    public class InputRecorder
    {
        private readonly TimedList<RecordedInput.InputData[]> _timedList = new TimedList<RecordedInput.InputData[]>(
            (datas, f) => new []{datas,datas});

        public void RecordInput(RecordedInput recorded)
        {            
            _timedList.Add(recorded.Data, recorded.Duration);
        }

        public IEnumerable<RecordedInput> GetInputForDuration(float duration)
        {
            return _timedList.Take(duration).Select(q => new RecordedInput(q.Value, q.Duration));
        }

        
    }
}