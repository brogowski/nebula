namespace Nebula.InputRecorder
{
    public struct RecordedInput
    {
        public RecordedInput(string name, float value, float duration)
        {
            Value = value;
            Name = name;
            Duration = duration;
        }

        public readonly float Duration;
        public readonly float Value;
        public readonly string Name;
    }
}