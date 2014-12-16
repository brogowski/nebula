namespace Nebula.Input
{
    public struct RecordedInput
    {
        public RecordedInput(InputData[] inputData, float duration)
        {
            Data = inputData;
            Duration = duration;
        }

        public readonly float Duration;
        public InputData[] Data;

        public struct InputData
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